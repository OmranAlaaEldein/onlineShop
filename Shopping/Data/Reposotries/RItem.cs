using Shopping.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shopping.Data.Reposotries
{
    public class RItem : Respository<Item>, IRItem
    {
        public RItem(ApplicationDbContext repositoryContext)
            : base(repositoryContext)
        {
        }
    }
}
