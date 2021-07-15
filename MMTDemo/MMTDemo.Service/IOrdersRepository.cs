using MMTDemo.Models.FunctionModels;
using System.Threading.Tasks;

namespace MMTDemo.Service
{
    public interface IOrdersRepository
    {
        public Task<Order> GetOrder(string customerId);
    }
}
