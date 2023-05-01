using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Shopping.Models.ViewModels
{
    public class CategoryDto
    {
        public Guid id { set; get; }
        
        [Display(Name = "Category")]
        public string Name { set; get; }
        public Guid BradeId { set; get; }

        public List<ProductDto> Products { set; get; }
    }

    public class CareateUpdateCategoryDto
    {
        public Guid? id { set; get; }

        [Required(ErrorMessage = "Name is required")]
        public string Name { set; get; }

        public Guid BradeId { set; get; }
    }
}
