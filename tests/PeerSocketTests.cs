#region License

#endregion

namespace EightMonkeys.MonkeyEmpire.MonkeyNet
{
    using NUnit.Framework;
    using System;
    using System.Net;
    using System.Net.Sockets;

    [TestFixture]
    public class PeerSocketTests
    {
        [Test]
        public void ParameterlessPeerConstructorTest() {
            var socket = new PeerSocket();
            Assert.IsNotNull(socket, "Parameterless constructor failed to create an object");
            Assert.True(socket.Bound, "Parameterless constructor was not able to bind to socket");
            Assert.AreEqual(new IPEndPoint(IPAddress.IPv6Any, 42337), socket.LocalEndPoint);
            socket.Dispose();
            Assert.False(socket.Bound, "The socket is bound longer than expected");
        }

        [Test]
        public void PortConstructorTest() {
            var socket = new PeerSocket(104);
            Assert.IsNotNull(socket, "Portparameter constructor failed to create an object");
            Assert.False(socket.Bound, "Socket is connected to a restricted port without elevated rights");
            Assert.False(socket.Open(), "Socket is listening on a restricted port");
            socket.Dispose();
            Assert.False(socket.Bound, "The socket is bound longer than expected");
        }

        [Test]
        public void EndPointConstructorTest() {
            var socket = new PeerSocket(new IPEndPoint(IPAddress.IPv6None, 12345));
            Assert.IsNotNull(socket, "EndPoint constructor failed to create an object");
            Assert.True(socket.Bound, "Constructor was not able to bind to socket");
            socket.Dispose();
            Assert.False(socket.Bound, "The socket is bound longer than expected");
        }
    }
}
