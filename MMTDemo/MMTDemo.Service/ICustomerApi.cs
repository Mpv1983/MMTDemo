using MMTDemo.Models.ApiModels;
using System.Threading.Tasks;

namespace MMTDemo.Service
{
    public interface ICustomerApi
    {
        Task<CustomerApiResponse> GetCustomer(string customerEmail);
    }
}
