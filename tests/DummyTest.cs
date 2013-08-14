#region License

#endregion

namespace EightMonkeys.MonkeyEmpire.MonkeyNet
{
    using NUnit.Framework;

    [TestFixture]
    public class DummyTester
    {
        [Test]
        public void initialTest()
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
