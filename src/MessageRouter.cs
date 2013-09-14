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

    /// <summary>
    /// This class is used to find out of packets sent to the local machine are intended for a 
    /// listening application. It checks the first four byte of an incoming message to retrieve a
    /// protocol code that is intended to identify an application. If one or more such applications
    /// are listening for incoming messages using this MessageRouter, their OnMessageReceived 
    /// method is called with the full message (including the message ID, at least for the moment).
    /// 
    /// In order to now about incoming messages, they have to register themselves by calling 
    /// RegisterApplication with an object implementing the IApplication interface.
    /// </summary>
    public class MessageRouter
    {
        /// <summary>
        /// the underlying socket to be used for communication
        /// </summary>
        private PeerSocket _socket;
        /// <summary>
        /// a list of registered applications
        /// </summary>
        private List<IApplication> _connectedApplications;

        /// <summary>
        /// An empty constructor. This will set up the internal socket to listen to IPv6Any on port
        /// 42337.
        /// </summary>
        public MessageRouter()
            : this(new IPEndPoint(IPAddress.IPv6Any, 42337)) { }

        /// <summary>
        /// Constructor to specify the local endpoint that the underlying socket is listening to.
        /// </summary>
        /// <param name="localEndPoint">The local endpoint to listen to</param>
        /// <exception cref="System.Exception">is thrown when the underlying socket could not bind 
        /// to the specified EndPoint</exception>
        public MessageRouter(IPEndPoint localEndPoint) {
            _socket = new PeerSocket(localEndPoint);
            if (!_socket.IsBound)
                throw new Exception("The underlying peer socket can't be initialized");
            _socket.MessageReceived += OnSocketMessageReceived;
            _socket.MessageSent += OnSocketMessageSent;
            _socket.ReceiveMessages();

            _connectedApplications = new List<IApplication>();
        }

        /// <summary>
        /// Subscribes an application to the MessageRouter in order to get notified when messages 
        /// are coming in for the specified protocolID
        /// </summary>
        /// <param name="app">the object that is to be notified at incoming messages</param>
        public void RegisterApplication(IApplication app) {
            _connectedApplications.Add(app);
        }

        void OnSocketMessageSent(object sender, SocketMessage e) {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The callback that is informed about incoming messages. It parses the first 4 byte of 
        /// the messages, retrieving an integer value from it that represents the protocolID of the
        /// application that this message is for. If one or more apllications are registered for 
        /// this protocol ID the are notified.
        /// </summary>
        /// <param name="sender">The <see cref="PeerSocket"/>that received the message</param>
        /// <param name="e"></param>
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
