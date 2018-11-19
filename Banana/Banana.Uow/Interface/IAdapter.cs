using Banana.Uow.Extension;
using Banana.Uow.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Banana.Uow.Interface
{
    public interface IAdapter
    {
        SqlBuilder GetPageSQL<T>(IRepository<T> repository, int pageNum, int pageSize, string whereString = null, object param = null, object order = null, bool asc = false)
           where T : class, IEntity;
    }
}
