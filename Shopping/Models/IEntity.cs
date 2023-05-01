using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shopping.Models
{
    public interface IEntity
    {
        Guid id { set; get; }
    }
}
