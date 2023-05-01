using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Shopping.Models
{
    public class Product : IEntity
    {
        public Product()
        {
            Items = new List<Item>();
            DateAdd = DateTime.Now;
        }
        [Key]
        public Guid id { set; get; }
        public string Name { set; get; }
        public string TagName { set; get; }
        public DateTime DateAdd { set; get; }
        
        public virtual Category Category { set; get; }
        
        [ForeignKey("Category")]
        public Guid CategoryId { set; get; }
        public virtual List<Item> Items { set; get; }
    }
}
