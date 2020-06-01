using Microsoft.VisualStudio.TestTools.UnitTesting;
using FlightControlWeb.Models;

namespace FlightControlWebTests
{
    [TestClass]
    public class FlightControlManagerTests
    {
        [TestMethod]
        public void Add_Get_EemoveServer_LeagalInput_ReturnValidAnswers()
        {
            var actualLen = 0;
            var expectedLen = 0;
            var flightCtrlManager = new FlightControlManager();
            var dummyServer = new Server
            {
                ServerId = "testName",
                ServerURL = "https://www.testUrl.com"
            };
            flightCtrlManager.addServer(dummyServer);
            var result = flightCtrlManager.GetServers();
            foreach (var item in result)
            {
                Assert.AreEqual(dummyServer.ServerId, item.ServerId);
                Assert.AreEqual(dummyServer.ServerURL, item.ServerURL);
            }
            flightCtrlManager.RemoveServer(dummyServer.ServerId);
            result = flightCtrlManager.GetServers();
            foreach (var item in result)
            {
                actualLen++;
            }
            Assert.AreEqual(expectedLen, actualLen);
        }

        [TestMethod]
        public void Add_RemoveServer_EleagalInput_ReturnsOneItem()
        {
            var actualLen = 0;
            var expectedLen = 1;
            var wrongID = "wringID";
            var flightCtrlManager = new FlightControlManager();
            var dummyServer = new Server
            {
                ServerId = "testName",
                ServerURL = "https://www.testUrl.com"
            };
            flightCtrlManager.addServer(dummyServer);
            var result = flightCtrlManager.GetServers();
            flightCtrlManager.RemoveServer(wrongID);
            result = flightCtrlManager.GetServers();
            foreach (var item in result)
            {
                actualLen++;
            }
            Assert.AreEqual(expectedLen, actualLen);
        }
    }
}
