using Shopping.Models.Customer;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Shopping.Models
{
    public class Item : IEntity
    {
        public Item()
        {
            OrderItem = new List<OrderItem>();
            DateAdd = DateTime.Now;
        }
        [Key]
        public Guid id { set; get; }
        public string Name { set; get; }

        public string color { set; get; }
        public bool count { set; get; }
        public int price { set; get; }
        
        public string pathImage { set; get; }
        public DateTime DateAdd { set; get; }


        public virtual Product product { set; get; }
        
        [ForeignKey("product")]
        public Guid productId { set; get; }
        
        public List<OrderItem> OrderItem { set; get; }

    }
}
