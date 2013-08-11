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
            Assert.IsNotNull(socket);
        }
    }
}
