using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests
{
    [TestClass]
    public class ExampleTests
    {
        [TestMethod]
        public void MainTest() =>
            new Example.RaytracingGPU().Run();
    }
}
