using Microsoft.AspNetCore.Identity;
using Shopping.Models.Customer;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Shopping.Models
{
    public class ApplicationUser : IdentityUser
    {
        public ApplicationUser()
        {
            OrderItem = new List<OrderItem>();
        }
        [Required]
        [MaxLength(20)]
        public string LastName { set; get; }

        public string Address { set; get; }

        public List<OrderItem> OrderItem { set; get; }


    }
}
