using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Autofac.Extras.Moq;
using Dapper;
using FlightControlWeb;
using FlightControlWeb.Controllers;
using FlightControlWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Nito.AsyncEx;
using Xunit;

namespace FlightControlWeb.Tests
{
    public class FlightPlanControllerTest
    {
        public FlightPlan GetFlightPlan()
        {
            return new FlightPlan() { company_name = "sd" }; ;
        }
        [Fact]
        public void ShouldGetFlightPlan()
        {
            using (var mock = AutoMock.GetLoose())
            {
                FlightPlan flightPlan = new FlightPlan() { company_name = "sd" };
                
                Task<FlightPlan> task1 = Task<FlightPlan>.FromResult(flightPlan);
                
                var x = new Mock<IFlightPlanManager>();

                x.Setup(x => x.GetFlightPlanByID("123")).Returns(task1);

                var controller = new FlightPlanController(x.Object);

                var actionResult = controller.GetFlightPlanById("123").Result;
                Microsoft.AspNetCore.Mvc.OkObjectResult okObjectResult = (Microsoft.AspNetCore.Mvc.OkObjectResult)actionResult;
                var temp = okObjectResult.Value;
                FlightPlan actual = (FlightPlan)temp;
                
                var expected = new FlightPlan() { company_name = "sd" };
                Assert.True(actual != null);
                Assert.Equal(expected.company_name, actual.company_name);
            }
        }
        [Fact]
        public void ShouldAddFlightPlan()
        {
            using (var mock = AutoMock.GetLoose())
            {
                FlightPlan flightPlan = new FlightPlan() { company_name = "bla" };
                var x = new Mock<IFlightPlanManager>();
                x.Setup(x => x.AddFlightPlan(flightPlan)).Returns(flightPlan);

                FlightPlanController f = new FlightPlanController(x.Object);
                var actionResult = f.AddFlightPlan(flightPlan).Result;
                Microsoft.AspNetCore.Mvc.OkObjectResult okObjectResult = (Microsoft.AspNetCore.Mvc.OkObjectResult)actionResult;
                var temp = okObjectResult.Value;
                FlightPlan actual = (FlightPlan)temp;
                var expected = flightPlan;
                Assert.True(actual != null);
                Assert.Equal(expected.company_name, actual.company_name);
            }
        }
    }
}
