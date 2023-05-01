using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Shopping.Models.ViewModels
{
    public class ItemDto
    {
        public Guid id { set; get; }
        
        [Display(Name = "Item")]
        public string Name { set; get; }

        public string color { set; get; }
        public bool count { set; get; }
        public int price { set; get; }
        
        public string pathImage { set; get; }
    }

    public class CreateUpdateItemDto
    {
        public Guid? id { set; get; }

        [Required(ErrorMessage = "Name is required")]
        public string Name { set; get; }

        [Required(ErrorMessage = "color is required")]
        public string color { set; get; }
        
        [Required(ErrorMessage = "count is required")]
        public bool count { set; get; }
        
        [Required(ErrorMessage = "price is required")]
        public int price { set; get; }

        public string pathImage { set; get; }

        public Guid productId { set; get; }
    }
}
