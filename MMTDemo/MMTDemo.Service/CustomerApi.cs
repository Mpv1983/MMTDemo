using Flurl;
using Flurl.Http;
using MMTDemo.Models;
using MMTDemo.Models.ApiModels;
using System.Threading.Tasks;

namespace MMTDemo.Service
{
    public class CustomerApi : ICustomerApi
    {
        private readonly string _ApiEndPoint;
        private readonly string _ApiKey;
        public CustomerApi(ServiceOptions serviceOptions)
        {
            _ApiEndPoint = serviceOptions.CustomersApi;
            _ApiKey = serviceOptions.CustomersApiKey;
        }
        public async Task<CustomerApiResponse> GetCustomer(string customerEmail)
        {
            var requestBody = new CustomerApiRequest { email = customerEmail };

            var result = await _ApiEndPoint
                .SetQueryParam("code", _ApiKey)
                .PostJsonAsync(requestBody)
                .ReceiveJson<CustomerApiResponse>();

            return result;
   
        }
    }
}
