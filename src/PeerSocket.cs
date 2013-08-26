#region License
/*
    MonkeyNet is a real-time networking library for CLRs
    Copyright (C) 2013  8monkeys.de

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program. If not, see {http://www.gnu.org/licenses/}.
*/
#endregion

namespace EightMonkeys.MonkeyEmpire.MonkeyNet
{
    using System;
    using System.Net;
    using System.Net.Sockets;

    public class PeerSocket: IDisposable, IMessageReceiver, IMessageSender // TODO: reduce to internal after the upper layers allow it
    {
        #region Events
        public event EventHandler<SocketMessage> MessageReceived;
        public event EventHandler<SocketMessage> MessageSent;
        #endregion

        #region public Properties
        public bool IsBound { get { return _udpSocket.IsBound; } }
        public uint IOObjectCount { get; private set; }
        #endregion

        #region private Fields
        Socket _udpSocket = new Socket(AddressFamily.InterNetworkV6,
                                       SocketType.Dgram,
                                       ProtocolType.Udp);
        System.Collections.Generic.Queue<SocketAsyncEventArgs> _receivingSAEAs;
        System.Collections.Generic.Queue<SocketAsyncEventArgs> _sendingSAEAs;
        bool _isReceiving = false;
        #endregion

        #region Constructors
        /// <summary>
        /// 
        /// </summary>
        /// <param name="localEndpoint"></param>
        /// <exception cref="ArgumentException">if localEnPoint was null</exception>
        /// <exception cref="SecurityException">if the caller is not allowed to open a socket</exception>
        public PeerSocket(EndPoint localEndpoint, uint ioObjectCount) {
            try {
                IOObjectCount = ioObjectCount;
                _udpSocket.Blocking = false;
                _udpSocket.Bind(localEndpoint);
                fillWithSendingSAEAobjects(ref _sendingSAEAs);
            }
            catch (SocketException e) {
                System.Diagnostics.Debug.WriteLine("Failed to bind to local Endpoint. Exception was:");
                System.Diagnostics.Debug.WriteLine(e.ErrorCode + e.Message);
            }
            catch (ObjectDisposedException e) {
                System.Diagnostics.Debug.WriteLine("The socket seems to have been deleted somewhere"
                    + "outside");
            }
        }
        #endregion

        #region public Methods
        public void Dispose() {
            if (_isReceiving && _udpSocket != null) {
                _udpSocket.Dispose();
                // when the socket is disposed, SAEAs return with their completed event. The flag prevents rescheduling a listener object.
                _isReceiving = false;
                foreach (var SAEAobject in _sendingSAEAs) {
                    SAEAobject.Dispose();
                }
                foreach (var SAEAobject in _receivingSAEAs) {
                    SAEAobject.Dispose();
                }

                _udpSocket = null;
                MessageReceived = null;
                MessageSent = null;
            }
        }

        public void ReceiveMessages() {
            _isReceiving = true;
            fillWithReadingSAEAobjects(ref _receivingSAEAs);
            startListening();
        }

        public void SendMessage(SocketMessage message) {
            _udpSocket.SendToAsync(_sendingSAEAs.Dequeue());
        }
        #endregion

        #region private Methods
        private void fillWithReadingSAEAobjects(ref System.Collections.Generic.Queue<SocketAsyncEventArgs> SAEAQueue) {
            SAEAQueue = new System.Collections.Generic.Queue<SocketAsyncEventArgs>((int)IOObjectCount);
            for (int i=0; i < IOObjectCount; ++i) {
                var SAEAobject = new SocketAsyncEventArgs();
                SAEAobject.RemoteEndPoint = _udpSocket.LocalEndPoint;
                SAEAobject.SocketFlags = SocketFlags.None;
                SAEAobject.SetBuffer(new byte[512], 0, 512);
                SAEAobject.Completed += OnReadingMessageComplete;
                SAEAQueue.Enqueue(SAEAobject);
            }
        }

        private void fillWithSendingSAEAobjects(ref System.Collections.Generic.Queue<SocketAsyncEventArgs> SAEAQueue) {
            SAEAQueue = new System.Collections.Generic.Queue<SocketAsyncEventArgs>((int)IOObjectCount);
            for (int i=0; i < IOObjectCount; ++i) {
                var SAEAobject = new SocketAsyncEventArgs();
                SAEAobject.SocketFlags = SocketFlags.None;
                SAEAobject.Completed += OnSendingMessageComplete;
                SAEAQueue.Enqueue(SAEAobject);
            }
        }

        private void OnSendingMessageComplete(object sender, SocketAsyncEventArgs e) {
            MessageSent(this, null);
        }

        void OnReadingMessageComplete(object sender, SocketAsyncEventArgs e) {
            startListening();
            MessageReceived(this, new SocketMessage(e.RemoteEndPoint, e.Buffer));
            _receivingSAEAs.Enqueue(e);
        }

        private void startListening() {
            if (_isReceiving)
                _udpSocket.ReceiveFromAsync(_receivingSAEAs.Dequeue());
        }
        #endregion
    }
}
