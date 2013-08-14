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
        public void PeerConstructorTests()
        {
            PeerSocket socket = new PeerSocket();
            Assert.IsNotNull( socket );
            socket = new PeerSocket( 5500 );
            Assert.IsNotNull( socket );
            socket = new PeerSocket(new IPEndPoint( IPAddress.IPv6Any, 5500));
            Assert.IsNotNull( socket );
        }

        [Test]
        public void ListeningTest()
        {
            PeerSocket socket = new PeerSocket();
            Assert.IsTrue( socket.Open() );
        }
    }
}
