using Shopping.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shopping.Data.Reposotries
{
    public class RProduct : Respository<Product>, IRProduct
    {
        public RProduct(ApplicationDbContext repositoryContext)
            : base(repositoryContext)
        {
        }
    }
}
