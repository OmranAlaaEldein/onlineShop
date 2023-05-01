using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Shopping.Models.ViewModels
{
    public class OrderDto
    {
        public OrderDto()
        {
            OrderItem = new List<OrderItemDto>();
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

        public List<OrderItemDto> OrderItem { set; get; }

        public string userId { set; get; }
        public string userName { set; get; }
        public int evaluate { set; get; } = 5;
    }

    public class OrderCreateUpdateDto
    {
        public OrderCreateUpdateDto()
        {
            OrderItem = new List<OrderItemDto>();
        }
        public Guid? id { set; get; }

        public DateTime DateOrderAsk { set; get; }
        public DateTime DateOrderSend { set; get; }
        public DateTime DateOrderEnd { set; get; }
        public int state { set; get; }

        [Required(ErrorMessage = "numbers is required")]
        public string numbers { set; get; }

        [Required(ErrorMessage = "Address is required")]
        public string Address { set; get; }
        public int lang { set; get; }
        public int lat { set; get; }
        public string Note { set; get; }
        public int evaluate { set; get; } = 5;

        public List<OrderItemDto> OrderItem { set; get; }
        public string idUser { set; get; }

    }

    public class OrderItemDto
    {
        public Guid id { set; get; }
        public int Quantity { set; get; }
        public int Pirce { set; get; }

        public Guid ItemId { set; get; }
        public ItemDto Item { set; get; }

        public Guid orderId { set; get; }
        public OrderDto order { set; get; }
    }
}
