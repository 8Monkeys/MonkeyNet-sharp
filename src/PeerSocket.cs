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
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Sockets;
    using System.Net.NetworkInformation;
    using System.Security;

    /// <summary>
    /// A PeerSocket is a single socket instance that handle all communication with other peers
    /// through a single port.
    /// </summary>
    /// <remarks>
    /// Create only one PeerSocket per port on a machine. This instance in then used to send and 
    /// receive messages from other peers. Each incoming message is forwarded to the apllication 
    /// registry that is routing the message to an application that is able to handle it, or drops
    /// the packet. 
    /// 
    /// Communication is working UDP-based and is completely asynchrnous.
    /// </remarks>
    public sealed class PeerSocket: IDisposable
    {
        #region Events
        public EventHandler<PeerMessage> newMessage;
        #endregion

        #region public Properties
        public bool Bound { get; private set; }
        public EndPoint LocalEndPoint { get; private set; }
        public int MTU { get; private set; }
        #endregion

        #region private Properties
        private Socket _udpSocket { get; set; }
        private Queue<SocketAsyncEventArgs> _stateobjects { get; set; }
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new object of the PeerSocket bound to the specified port. The socket 
        /// assumes IPv6 is available on the system.
        /// </summary>
        public PeerSocket(int port)
            : this(new IPEndPoint(IPAddress.IPv6Any, port)) { }

        /// <summary>
        /// Initializes a new object of the PeerSocket bound to the specified Endpoint.
        /// </summary>
        /// <remarks>
        /// A new UDP socket is initialized that is using IPv6 for address lookup. The underlying 
        /// OS has to support that in order to run with this correctly. Refer to 
        /// http://msdn.microsoft.com/en-us/library/ms172319(v=vs.100).aspx for more details.
        /// 
        /// If the underlying socket is failing to bind to the specified EndPoint an exception is
        /// caught in this class and the Bound property is set to false.
        /// </remarks>
        /// <param name="localEndpoint">Listening address and port to listen to</param>
        public PeerSocket(EndPoint localEndpoint) {
            _udpSocket = new Socket(AddressFamily.InterNetworkV6, SocketType.Dgram, ProtocolType.Udp);
            LocalEndPoint = localEndpoint;
            Bound = _udpSocket.IsBound;
            try {
                _udpSocket.Blocking = false;
                _udpSocket.Bind(LocalEndPoint);
                Bound = _udpSocket.IsBound;
            }
            catch (SocketException e) {
                Console.WriteLine(e.Message);
                Bound = false;
            }
            catch (ArgumentNullException e) {
                Console.WriteLine(e.Message);
                Bound = false;
            }
            catch (SecurityException e) {
                Console.WriteLine(e.Message);
                Bound = false;
            }
        }

        /// <summary>
        /// Initializes a new PeerSocket object that binds to port 42337
        /// </summary>
        public PeerSocket()
            : this(42337) { }
        #endregion

        #region public Methods
        /// <summary>
        /// "Opens" the socket and waits for incoming messages
        /// </summary>
        /// <returns>true if the server has started listening</returns>
        public bool Open() {
            if (Bound && _udpSocket.IsBound) {
                findMaxMTU();
                _stateobjects = new Queue<SocketAsyncEventArgs>(10);
                for (int i=0; i <= 9; i++) {
                    var aNewStateObject = new SocketAsyncEventArgs();
                    aNewStateObject.Completed += OnMessageReceived;
                    _stateobjects.Enqueue(aNewStateObject);
                }
                return true;
            }
            else
                return false;
        }

        /// <summary>
        /// Disposes the PeerSocket object and releases the underlying socket.
        /// </summary>
        public void Dispose() {
            foreach (var argsObject in _stateobjects) {
                argsObject.Dispose();
            }
            _udpSocket.Close();
            _udpSocket.Dispose();
        }
        #endregion

        #region private Methods
        private void OnMessageReceived(object sender, SocketAsyncEventArgs e) {
            throw new NotImplementedException();
        }

        private void findMaxMTU() {
            foreach (var Interface in NetworkInterface.GetAllNetworkInterfaces()) {
                int interfaceMTU = Interface.GetIPProperties().GetIPv6Properties().Mtu;
                if (interfaceMTU > MTU)
                    MTU = interfaceMTU;
            }
        }
        #endregion
    }
}
