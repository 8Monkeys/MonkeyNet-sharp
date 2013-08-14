#region License

#endregion

namespace EightMonkeys.MonkeyEmpire.MonkeyNet
{
    using NUnit.Framework;
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
            socket = new PeerSocket(new IPEndpoint(IPAddress.IPv6None, 5500));
            Assert.IsNotNull( socket );
        }
    }
}
