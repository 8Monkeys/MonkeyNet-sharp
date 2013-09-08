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
    // TODO: Rename to something like IMessageInput and write classes that accept scripted input as well. Then, make the Message Router class have a list of inputs that can be initialized with more than one IMessageInput instance. Then we can listen to either input type or both at once.
    using System;

    interface IMessageReceiver
    {
        event EventHandler<SocketMessage> MessageReceived;
        void ReceiveMessages();
    }
}
