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
    using System.Collections.Generic;

    public class MessageRouter
    {
        private PeerSocket _socket;
        private List<uint> _registeredProtocolIDs; // TODO: Maybe a hashmap of ints and delegates is best to use here?

        public MessageRouter()
            : this(new IPEndPoint(IPAddress.IPv6Any, 42337)) { }

        public MessageRouter(IPEndPoint localEndPoint) {
            _socket = new PeerSocket(localEndPoint, 10);
            if (!_socket.IsBound)
                throw new Exception("The underlying peer socket can't be initialized");
            _socket.MessageReceived += OnSocketMessageReceived;
            _socket.MessageSent += OnSocketMessageSent;
            _socket.ReceiveMessages();

            _registeredProtocolIDs = new List<uint>();
        }

        public void RegisterNewProtocolID(uint protocolID) {
            _registeredProtocolIDs.Add(protocolID);
        }

        void OnSocketMessageSent(object sender, SocketMessage e) {
            throw new NotImplementedException();
        }

        void OnSocketMessageReceived(object sender, SocketMessage e) {
            uint protocolID = BitConverter.ToUInt32(e.MessagePayload, 0);
            if (_registeredProtocolIDs.Contains(protocolID) && (sender as PeerSocket) == _socket) {
                // TODO: Fire a message to the application registered with this protocol
                Console.WriteLine("Bang!");
            }
        }
    }
}
