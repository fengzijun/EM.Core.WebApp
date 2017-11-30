using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace EM.Infrastructure.UnitOfWork
{
    public interface IUnitOfWorkManager
    {
        IUnitOfWork Current { get; }

        IUnitOfWork Begin();

        IUnitOfWork Begin(TransactionScopeOption scope);

        IUnitOfWork Begin(UnitOfWorkOptions options);
    }
}