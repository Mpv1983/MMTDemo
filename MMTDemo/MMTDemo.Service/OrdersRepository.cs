using Microsoft.EntityFrameworkCore;
using MMTDemo.Models.DbModels;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MMTDemo.Service
{

    public class OrdersRepository : DbContext, IOrdersRepository
    {

        public OrdersRepository(DbContextOptions<OrdersRepository> options) : base(options)
        {

        }

        public DbSet<Product> Products { get; set; }

        public DbSet<Order> Orders { get; set; }

        public DbSet<OrderItem> OrderItems { get; set; }

        public async Task<MMTDemo.Models.FunctionModels.Order> GetOrder(string customerId) 
        {
            // Find the most Recent Order
            var mostRecentOrder = Orders.Where(x=>x.CustomerId == customerId).OrderByDescending(t => t.OrderDate).FirstOrDefault();

            if(mostRecentOrder == null)
            {
                return null;
            }

            //  Get Products if needed
            List<Product> products = null;

            if (!mostRecentOrder.ContainsGift)
            {
                products = Products.ToList();
            }

            //  Get Order Items
            var orderItems = OrderItems.Where(x => x.OrderId == mostRecentOrder.OrderId);

            return BuildOrderResponse(mostRecentOrder, orderItems, products);
        }

        private MMTDemo.Models.FunctionModels.Order BuildOrderResponse(Order mostRecentOrder, IQueryable<OrderItem> orderItems, List<Product> products)
        {

            var orderDetails = new MMTDemo.Models.FunctionModels.Order();

            orderDetails.DeliveryExpected = mostRecentOrder.DeliveryExpected.ToString();
            orderDetails.OrderDate = mostRecentOrder.OrderDate.ToString();
            orderDetails.OrderNumber = mostRecentOrder.OrderId;
            orderDetails.OrderItems = new List<Models.FunctionModels.OrderItem>();

            foreach (var item in orderItems)
            {
                string product = "Gift";

                if(products != null)
                {
                    product = products.Where(x => x.ProductId == item.ProductId).FirstOrDefault().ProductName;
                }

                orderDetails.OrderItems.Add(new Models.FunctionModels.OrderItem
                {
                    Quantity = item.Quantity,
                    Product = product,
                    PriceEach = item.Price.ToString()
                });
            }

            return orderDetails;
        }
    }
}

