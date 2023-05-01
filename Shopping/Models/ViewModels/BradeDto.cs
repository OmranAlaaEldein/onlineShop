using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Shopping.Models.ViewModels
{
    public class BradeDto
    {
        //test init list
        public Guid id { set; get; }
        [Display(Name= "Brade")]
        public string Name { set; get; }
        
        [Display(Name = "Image")]
        public string pathImage { set; get; }

        public List<CategoryDto> Categorys { set; get; }
    }

    public class CreateUpdateBradeDto
    {
        public Guid? id { set; get; }
        [Required(ErrorMessage = "Name is required")]
        public string Name { set; get; }
        public string pathImage { set; get; }

    }
}
