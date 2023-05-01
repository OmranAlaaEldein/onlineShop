using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Shopping.Models
{
    public class Category : IEntity
    {
        public Category()
        {
            Products = new List<Product>();
            DateAdd = DateTime.Now;
        }
        [Key]
        public Guid id { set; get; }
        public string Name { set; get; }
        public DateTime DateAdd { set; get; }

        public virtual Brade Brade { set; get; }
        [ForeignKey("Brade")]
        public Guid BradeId { set; get; }

        public virtual List<Product> Products { set; get; }

    }
}
