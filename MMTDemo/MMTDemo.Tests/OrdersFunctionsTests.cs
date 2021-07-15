using AutoFixture;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MMTDemo.Models.ApiModels;
using MMTDemo.Models.FunctionModels;
using MMTDemo.Service;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using System.IO;
using System.Threading.Tasks;

namespace MMTDemo.Tests
{
    public class OrdersFunctionsTests
    {
        private Mock<IOrdersRepository> _mockOrdersRepository;
        private Mock<ICustomerApi> _mockCustomerApi;
        private Mock<ILogger> _mockILogger;
        private Fixture _fixture;


        [SetUp]
        public void Setup()
        {
            _mockOrdersRepository = new Mock<IOrdersRepository>();
            _mockCustomerApi = new Mock<ICustomerApi>();
            _mockILogger = new Mock<ILogger>();
            _fixture = new Fixture();
        }

        [Test]
        public void GetCustomerOrder_InvalidBody_ReturnsBadRequest()
        {
            //  Arrange
            var ordersFunction = new OrdersFunction(_mockOrdersRepository.Object, _mockCustomerApi.Object);
            var request = CreateMockRequest("");

            //  Act
            var response = ordersFunction.GetCustomerOrder(request.Object, _mockILogger.Object).GetAwaiter().GetResult();

            //  Assert
            Assert.IsInstanceOf<BadRequestObjectResult>(response);
            Assert.AreEqual("Invalid request body", GetBadResponseValue(response));
        }

        [Test]
        public void GetCustomerOrder_CustomerNotFound_ReturnsBadRequest()
        {
            //  Arrange
            var ordersFunction = new OrdersFunction(_mockOrdersRepository.Object, _mockCustomerApi.Object);
            var request = CreateMockRequest(new GetCustomerOrdersRequestModel
            {
                customerId = "someId",
                user = "someUser"
            });

            CustomerApiResponse customerApiResponse = null;
            _mockCustomerApi.Setup(m => m.GetCustomer(It.IsAny<string>())).Returns(Task.FromResult(customerApiResponse));

            //  Act
            var response = ordersFunction.GetCustomerOrder(request.Object, _mockILogger.Object).GetAwaiter().GetResult();

            //  Assert
            Assert.IsInstanceOf<BadRequestObjectResult>(response);
            Assert.AreEqual("Customer not found", GetBadResponseValue(response));
        }
        
        [Test]
        public void GetCustomerOrder_OrderNotFound_ReturnsBadRequest()
        {
            //  Arrange
            var ordersFunction = new OrdersFunction(_mockOrdersRepository.Object, _mockCustomerApi.Object);
            var request = CreateMockRequest(new GetCustomerOrdersRequestModel
            {
                customerId = "someId",
                user = "someUser"
            });

            var customerApiResponse = _fixture.Create<CustomerApiResponse>();
            Order order = null;

            _mockCustomerApi.Setup(m => m.GetCustomer(It.IsAny<string>())).Returns(Task.FromResult(customerApiResponse));
            _mockOrdersRepository.Setup(m=>m.GetOrder(It.IsAny<string>())).Returns(Task.FromResult(order));

            //  Act
            var response = ordersFunction.GetCustomerOrder(request.Object, _mockILogger.Object).GetAwaiter().GetResult();

            //  Assert
            Assert.IsInstanceOf<BadRequestObjectResult>(response);
            Assert.AreEqual("Order for Customer someId not found", GetBadResponseValue(response));
        }

        [Test]
        public void GetCustomerOrder_ReturnsOkObjectResult()
        {
            //  Arrange
            var ordersFunction = new OrdersFunction(_mockOrdersRepository.Object, _mockCustomerApi.Object);
            var request = CreateMockRequest(new GetCustomerOrdersRequestModel
            {
                customerId = "someId",
                user = "someUser"
            });

            var customerApiResponse = _fixture.Create<CustomerApiResponse>();
            var order = _fixture.Create<Order>();

            _mockCustomerApi.Setup(m => m.GetCustomer(It.IsAny<string>())).Returns(Task.FromResult(customerApiResponse));
            _mockOrdersRepository.Setup(m => m.GetOrder(It.IsAny<string>())).Returns(Task.FromResult(order));

            //  Act
            var response = ordersFunction.GetCustomerOrder(request.Object, _mockILogger.Object).GetAwaiter().GetResult();

            //  Assert
            Assert.IsInstanceOf<OkObjectResult>(response);
        }

        private static Mock<HttpRequest> CreateMockRequest(object body)
        {
            var ms = new MemoryStream();
            var sw = new StreamWriter(ms);

            var json = JsonConvert.SerializeObject(body);

            sw.Write(json);
            sw.Flush();

            ms.Position = 0;

            var mockRequest = new Mock<HttpRequest>();
            mockRequest.Setup(x => x.Body).Returns(ms);

            return mockRequest;
        }

        private static string GetBadResponseValue(IActionResult actionResult)
        {
            return (string)((BadRequestObjectResult)actionResult).Value;
        }
    }
}