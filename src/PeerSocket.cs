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
        /// <summary>
        /// This event is fired every time the socket receives a datagram. A new 
        /// <see cref="EightMonkeys.MonkeyEmpire.MonkeyNet.SocketMessage"/> is created that contains
        /// information about the sender of the datagram and the raw payload delivered with it. 
        /// The sender object of the event is always an instance of this class, not the socket that
        /// received the data.
        /// </summary>
        public event EventHandler<SocketMessage> MessageReceived;
        /// <summary>
        /// This event is fired every time the socket has sent a datagram. The 
        /// <see cref="EightMonkeys.MonkeyEmpire.MonkeyNet.SocketMessage"/> can be checked if the 
        /// message sent is the same as the argument passed to the SendMessage method of this class.
        /// </summary>
        public event EventHandler<SocketMessage> MessageSent;
        #endregion

        #region public Properties
        /// <summary>
        /// A flag that returns a boolean value indicating if the underlying socket is bound to a 
        /// port. You should consider the PeerSocket object inoperable if this returns false.
        /// </summary>
        public bool IsBound { get { return _udpSocket.IsBound; } }
        /// <summary>
        /// The number of <see cref="System.Net.Sockets.SocketAsyncEventArgs"/> that are used on the socket.
        /// </summary>
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
                    + "outside: " + e.Message);
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
            var saea = _sendingSAEAs.Dequeue();
            saea.RemoteEndPoint = message.MessagePeer;
            saea.SetBuffer(message.MessagePayload, 0, message.MessagePayload.Length);
            if (!_udpSocket.SendToAsync(saea))
                OnSendingMessageComplete(this, saea);
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
            byte[] message = new byte[e.BytesTransferred];
            Buffer.BlockCopy(e.Buffer, 0, message, 0, e.BytesTransferred);
            MessageSent(this, new SocketMessage(e.RemoteEndPoint, message));
            _sendingSAEAs.Enqueue(e);
        }

        void OnReadingMessageComplete(object sender, SocketAsyncEventArgs e) {
            startListening();
            byte[] message = new byte[e.BytesTransferred];
            Buffer.BlockCopy(e.Buffer, 0, message, 0, e.BytesTransferred);
            MessageReceived(this, new SocketMessage(e.RemoteEndPoint, message));
            _receivingSAEAs.Enqueue(e);
        }

        private void startListening() {
            if (_isReceiving)
                _udpSocket.ReceiveFromAsync(_receivingSAEAs.Dequeue());
        }
        #endregion
    }
}
