using Shopping.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shopping.Data.Reposotries
{
    public class RCategory : Respository<Category>, IRCategory
    {
        public RCategory(ApplicationDbContext repositoryContext)
            : base(repositoryContext)
        {
        }
    }
}
