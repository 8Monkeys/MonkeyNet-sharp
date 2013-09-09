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
        private List<IApplication> _connectedApplications;

        public MessageRouter()
            : this(new IPEndPoint(IPAddress.IPv6Any, 42337)) { }

        public MessageRouter(IPEndPoint localEndPoint) {
            _socket = new PeerSocket(localEndPoint);
            if (!_socket.IsBound)
                throw new Exception("The underlying peer socket can't be initialized");
            _socket.MessageReceived += OnSocketMessageReceived;
            _socket.MessageSent += OnSocketMessageSent;
            _socket.ReceiveMessages();

            _connectedApplications = new List<IApplication>();
        }

        public void RegisterApplication(IApplication app) {
            _connectedApplications.Add(app);
        }

        void OnSocketMessageSent(object sender, SocketMessage e) {
            throw new NotImplementedException();
        }

        void OnSocketMessageReceived(object sender, SocketMessage e) {
            // System.BitConverter is not CLS-compliant, neither is uint (= System.UInt32)
            int protocolID = e.MessagePayload[3];
            protocolID = (protocolID << 8) + e.MessagePayload[2];
            protocolID = (protocolID << 8) + e.MessagePayload[1];
            protocolID = (protocolID << 8) + e.MessagePayload[0];
            var receivers = _connectedApplications.FindAll(x => x.ProtocolID == protocolID);
            while (receivers.Count > 0) {
                receivers[receivers.Count - 1].OnMessageReceived(e.MessagePeer, e.MessagePayload);
                receivers.RemoveAt(receivers.Count - 1);
            }
        }
    }
}
