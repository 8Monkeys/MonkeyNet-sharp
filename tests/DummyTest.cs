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
            PeerSocket socket = new PeerSocket(1337);
            Assert.IsNotNull(socket);
        }
    }
}
