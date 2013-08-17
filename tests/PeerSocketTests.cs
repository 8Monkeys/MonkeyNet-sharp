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
        public void PeerConstructorTests() {
            PeerSocket socket = new PeerSocket();
            Assert.IsNotNull(socket);
            Assert.True(socket.Bound, "Parameterless constructor was not able to bind to socket");
            socket = new PeerSocket(5500);
            Assert.IsNotNull(socket);
            Assert.True(socket.Bound, "Constructor was not able to bind to port 5500");
            socket = new PeerSocket(new IPEndPoint(IPAddress.IPv6Any, 5500));
            Assert.IsNotNull(socket);
            Assert.True(socket.Bound, "Constructor was not able to bind to IPEndPoint IPv6Any, port 5500");
            socket = new PeerSocket(new IPEndPoint(0, 0));
            Assert.False(socket.Bound);
        }

        [Test]
        public void ListeningTest() {
            PeerSocket socket = new PeerSocket();
            Assert.IsTrue(socket.Open());
        }
    }
}
