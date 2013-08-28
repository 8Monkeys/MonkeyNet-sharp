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

    /// <summary>
    /// An EventArgs class that wraps up the information about the sender of a message and the 
    /// message itself. A SocketMessage is used to inform users about received data and is also 
    /// specifying where the data wrapped is to be sent to.
    /// </summary>
    class SocketMessage: EventArgs // TODO: reduce to internal after the upper layers allow it
    {
        /// <summary>
        /// Since the SocketMessage is read-only once it is created, the constructor is passed the
        /// required information to fill internal fields.
        /// </summary>
        /// <param name="peer">The sender/ receiver of a message as IPEndPoint</param>
        /// <param name="payload">The raw message</param>
        public SocketMessage(EndPoint peer, byte[] payload) {
            MessagePeer = peer;
            MessagePayload = payload;
        }
        /// <summary>
        /// The sender/ receiver of a message as IPEndPoint
        /// </summary>
        public EndPoint MessagePeer { get; private set; }
        /// <summary>
        /// The raw message
        /// </summary>
        public byte[] MessagePayload { get; private set; }
    }
}
