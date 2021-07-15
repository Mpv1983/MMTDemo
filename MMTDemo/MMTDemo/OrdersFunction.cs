using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using MMTDemo.Models.ApiModels;
using MMTDemo.Models.FunctionModels;
using MMTDemo.Service;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MMTDemo
{
    public class OrdersFunction
    {
        private readonly IOrdersRepository _ordersRepository;
        private readonly ICustomerApi _customerApi;

        public OrdersFunction(IOrdersRepository ordersRepository, ICustomerApi customerApi)
        {
            _ordersRepository = ordersRepository;
            _customerApi = customerApi;
        }

        [FunctionName("GetCustomerOrder")]
        public async Task<IActionResult> GetCustomerOrder(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "CustomerOrders")] HttpRequest req,
            ILogger log)
        {

            //  Return Bad request if body invalid
            var body = await GetRequestBody<GetCustomerOrdersRequestModel>(req, log);
            if(body == null)
            {
                return new BadRequestObjectResult("Invalid request body");
            }

            //  Get Customer
            var customer = await _customerApi.GetCustomer(body.user);
            if(customer == null)
            {
                log.LogWarning($"Api did not return a valid customer for user:{body.user}");
                return new BadRequestObjectResult("Customer not found");
            }

            //  Get Order
            var order = await _ordersRepository.GetOrder(body.customerId);
            if(order == null)
            {
                log.LogWarning($"No order information found in the database");
                return new BadRequestObjectResult($"Order for Customer {body.customerId} not found");
            }

            var response = BuildResponse(customer, order);

            return new OkObjectResult(response);
        }

        private async static Task<T> GetRequestBody<T>(HttpRequest req, ILogger log) where T:class
        {
            try
            {
                string json = await req.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<T>(json);
            }
            catch (Exception e)
            {
                log.LogWarning($"Exception occurred deserializing request body Error:{e.Message}");
            }

            return null;
        }

        private CustomerOrderResponse BuildResponse(CustomerApiResponse customer, Order order)
        {
            return new CustomerOrderResponse
            {
                Customer = new Customer
                {
                    FirstName = customer.firstName,
                    LastName = customer.lastName
                },
                Order = new Order
                {
                    DeliveryAddress = $"{customer.houseNumber} {customer.street} {customer.town} {customer.postcode}",
                    DeliveryExpected = order.DeliveryExpected,
                    OrderDate = order.OrderDate,
                    OrderItems = order.OrderItems,
                    OrderNumber = order.OrderNumber
                }
            };

        }

    }
}
