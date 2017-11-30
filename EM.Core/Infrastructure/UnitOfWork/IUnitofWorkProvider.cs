using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EM.Infrastructure.UnitOfWork
{
    public interface IUnitOfWorkProvider
    {
        IUnitOfWork Current { get; set; }
    }
}