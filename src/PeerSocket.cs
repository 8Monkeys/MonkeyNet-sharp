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
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see {http://www.gnu.org/licenses/}.
*/
#endregion

namespace EightMonkeys.MonkeyEmpire.MonkeyNet
{
    using System;
    using System.Net;
    using System.Net.Sockets;

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
    public sealed class PeerSocket: Socket, IDisposable
    {
        /// <summary>
        /// Initializes a new object of the PeerSocket bound to the specified port.
        /// </summary>
        public PeerSocket( int port )
            : base( AddressFamily.InterNetworkV6, SocketType.Dgram, ProtocolType.Udp )
        {
            try
            {
                IPEndPoint boundEndPoint = new IPEndPoint( IPAddress.IPv6Any, port );
                base.Bind( boundEndPoint );
                base.Blocking = false;
            }
            catch ( SocketException ESocket )
            {
                Console.WriteLine( ESocket.Message );
                Dispose();
            }
        }

        public void Open()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        void IDisposable.Dispose()
        {
            base.Dispose();
        }
    }
}
