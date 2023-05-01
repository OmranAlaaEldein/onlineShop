using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Shopping.Models.ViewModels
{
    public class ProductDto
    {
        public Guid id { set; get; }

        public string Name { set; get; }
        public string TagName { set; get; }

        public List<ItemDto> Items { set; get; }
    }

    public class CreateUpdateProductDto
    {
        public Guid id { set; get; }
        
        [Required(ErrorMessage = "Name is required")]
        public string Name { set; get; }
        public string TagName { set; get; }

        public Guid CategoryId { set; get; }
    }
}
