using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac.Extras.Moq;
using Castle.Core;
using Dapper;
using FlightControlWeb;
using FlightControlWeb.Controllers;
using FlightControlWeb.Models;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
namespace FlightControlWeb.Tests
{
    public class ServersControllerTest
    {
        [Fact]
        public void ShouldAddServer()
        {
            // Arrange
            Server server = new Server { ServerID = "test", ServerURL = "test" };
            var x = new Mock<IServerManager>();
            x.Setup(x => x.AddServer(server)).Returns(server);

            var httpContext = new DefaultHttpContext();

            var controllerContext = new ControllerContext()
            {
                HttpContext = httpContext,
            };
            //assign context to controller
            ServersController controller = new ServersController(x.Object)
            {
                ControllerContext = controllerContext,
            };

            //Act
            var actionResult = controller.Post(server).Result;
            Microsoft.AspNetCore.Mvc.OkObjectResult okObjectResult = (Microsoft.AspNetCore.Mvc.OkObjectResult)actionResult;
            var temp = okObjectResult.Value;
            Server actual = (Server)temp;
            var excpected = server;
            // Assert
            Assert.True(actual != null);

            Assert.Equal(excpected.ServerID, actual.ServerID);
            Assert.Equal(excpected.ServerURL, actual.ServerURL);
        }

        public List<Server> getSampleServers()
        {
            return new List<Server>() { new Server() {  ServerID="test", ServerURL="test" } };
        }
    }
}
