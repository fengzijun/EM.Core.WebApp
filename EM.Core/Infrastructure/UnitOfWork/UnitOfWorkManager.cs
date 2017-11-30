using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace EM.Infrastructure.UnitOfWork
{
    public class UnitOfWorkManager : IUnitOfWorkManager
    {
        private ILogger _logger;

        private string _id;

        private readonly IUnitOfWorkProvider _currentUnitOfWorkProvider;

        public UnitOfWorkManager(IUnitOfWorkProvider currentUnitOfWorkProvider)
        {
            _logger = EngineContext.Current.Resolve<ILogger>();

            _currentUnitOfWorkProvider = currentUnitOfWorkProvider;

            _id = Guid.NewGuid().ToString("N");
        }

        public IUnitOfWork Current
        {
            get { return _currentUnitOfWorkProvider.Current; }
        }


        public IUnitOfWork Begin()
        {
            return Begin(new UnitOfWorkOptions());
        }

        public IUnitOfWork Begin(TransactionScopeOption scope)
        {
            return Begin(new UnitOfWorkOptions { Scope = scope });
        }

        public IUnitOfWork Begin(UnitOfWorkOptions options)
        {
            var outerUow = _currentUnitOfWorkProvider.Current;

            if (outerUow != null)
            {
                //((UnitOfWorkBase)outerUow).Options = options;
                //var innerunitofwork = new InnerUnitOfWork(options);
                //innerunitofwork.Outer = outerUow;
                //innerunitofwork.Begin(options);
                //return innerunitofwork;
                return new InnerUnitOfWork(options);
            }

            var uow = EngineContext.Current.Resolve<IUnitOfWork>();

            uow.Begin(options);

            _currentUnitOfWorkProvider.Current = uow;

            return uow;
        }

        public override string ToString()
        {
            return $"[UnitOfWorkManager {_id}]";
        }

    }
}