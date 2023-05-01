using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Shopping.Models.Customer
{
    public class Order : IEntity
    {
        public Order()
        {
            OrderItem = new List<OrderItem>();
        }
        public Guid id { set; get; }

        public DateTime DateOrderAsk { set; get; }
        public DateTime DateOrderSend { set; get; }
        public DateTime DateOrderEnd { set; get; }
        public int state { set; get; }
        
        public string numbers { set; get; }
        public string Address { set; get; }
        public int lang { set; get; }
        public int lat { set; get; }
        public string Note { set; get; }
        public int evaluate { set; get; } = 5;

        public List<OrderItem> OrderItem { set; get; }

        [ForeignKey("ApplicationUser")]
        public string ApplicationUserid { set; get; }
        public ApplicationUser ApplicationUser { set; get; }
    }

    public class OrderItem
    {
        public OrderItem()
        {        }

        public Guid id { set; get; }
        public int Quantity { set; get; }
        public int Pirce { set; get; }

        [ForeignKey("Item")]
        public Guid ItemId { set; get; }
        public Item Item { set; get; }

        [ForeignKey("order")]
        public Guid orderId { set; get; }
        public Order order { set; get; }
    }
}
