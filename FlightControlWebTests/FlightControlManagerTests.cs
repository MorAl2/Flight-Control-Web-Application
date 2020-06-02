using Microsoft.VisualStudio.TestTools.UnitTesting;
using FlightControlWeb.Models;

namespace FlightControlWebTests
{
    [TestClass]
    public class FlightControlManagerTests
    {
        [TestMethod]
        //test the the get and remove methonds, using the add method during the test to confirm.
        //the function is inserting legal flight to the server, ask for it again and delete it.
        //to make sure the remove method worked as expected - we check for number of items.
        //in this scenario - we should find 0 item.
        public void Add_Get_RemoveServer_LegalInput_ReturnValidAnswers()
        {
            var actualLen = 0;
            var expectedLen = 0;
            var flightCtrlManager = new FlightControlManager();
            var dummyServer = new Server
            {
                ServerId = "testName",
                ServerURL = "https://www.testUrl.com"
            };
            flightCtrlManager.AddServer(dummyServer);
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
        // test the the get and remove methonds, using the add method during the test to confirm.
        // the function is inserting illegal flight to the server, ask for it again and delete it.
        // to make sure the remove method worked as expected - we check for number of items.
        // in this scenario - we should find 1 item.
        public void Add_RemoveServer_ElegalInput_ReturnsOneItem()
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
            flightCtrlManager.AddServer(dummyServer);
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
