using Shopping.Models.Customer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shopping.Data.Reposotries
{
    public class ROrder : Respository<Order>, IROrder
    {
        public ROrder(ApplicationDbContext repositoryContext)
            : base(repositoryContext)
        {
        }
    }
}
