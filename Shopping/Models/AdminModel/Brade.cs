using Shopping.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Shopping.Models
{
    public class Brade:IEntity
    {
        public Brade()
        {
            Categorys = new List<Category>();
            DateAdd = DateTime.Now;
        }
        [Key]
        public Guid id { set; get; }
        
        public string Name { set; get; }
        public string pathImage { set; get; }

        public DateTime DateAdd { set; get; }


        public virtual List<Category> Categorys { set; get; }

    }
}
