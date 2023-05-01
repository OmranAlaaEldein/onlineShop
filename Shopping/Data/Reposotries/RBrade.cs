using Shopping.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Shopping.Data
{
    public class RBrade : Respository<Brade>, IRBrade
    {
        public RBrade(ApplicationDbContext repositoryContext)
            : base(repositoryContext)
        {
        }
    }
}
