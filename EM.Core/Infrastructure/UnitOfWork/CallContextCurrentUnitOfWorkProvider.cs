using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace EM.Infrastructure.UnitOfWork
{
    public class CallContextCurrentUnitOfWorkProvider : IUnitOfWorkProvider
    {
        private ILogger _logger;

        private const string ContextKey = "EM.UnitOfWork";

        private readonly static ConcurrentDictionary<string, IUnitOfWork> UnitOfWorkDictionary = new ConcurrentDictionary<string, IUnitOfWork>();

        private string _id;


        public override string ToString()
        {
            return $"[UnitOfWorkProvider {_id}]";
        }

        public CallContextCurrentUnitOfWorkProvider()
        {
            _logger = EngineContext.Current.Resolve<ILogger>();
            _id = Guid.NewGuid().ToString("N");
        }

        public IUnitOfWork Current
        {
            get { return GetCurrentUow(); }
            set { SetCurrentUow(value); }
        }

        private IUnitOfWork GetCurrentUow()
        {
            var unitOfWorkKey = CallContext.LogicalGetData(ContextKey) as string;
            if (unitOfWorkKey == null)
            {
                return null;
            }

            IUnitOfWork unitOfWork;
            if (!UnitOfWorkDictionary.TryGetValue(unitOfWorkKey, out unitOfWork))
            {
                //logger.Warn("There is a unitOfWorkKey in CallContext but not in UnitOfWorkDictionary (on GetCurrentUow)! UnitOfWork key: " + unitOfWorkKey);
                CallContext.FreeNamedDataSlot(ContextKey);
                return null;
            }

            if (unitOfWork.IsDisposed)
            {
                //_logger.WriteWarn("There is a unitOfWorkKey in CallContext but the UOW was disposed!");
                UnitOfWorkDictionary.TryRemove(unitOfWorkKey, out unitOfWork);
                CallContext.FreeNamedDataSlot(ContextKey);
                return null;
            }

            return unitOfWork;


            //IUnitOfWork unitofwork = CallContext.GetData(ContextKey) as IUnitOfWork;

            //return unitofwork;
        }

        private void SetCurrentUow(IUnitOfWork value)
        {
            if (value == null)
            {
                ExitFromCurrentUowScope();
                return;
            }

            var unitOfWorkKey = CallContext.LogicalGetData(ContextKey) as string;
            if (unitOfWorkKey != null)
            {
                IUnitOfWork outer;
                if (UnitOfWorkDictionary.TryGetValue(unitOfWorkKey, out outer))
                {
                    if (outer == value)
                    {
                        //_logger.WriteWarn("Setting the same UOW to the CallContext, no need to set again!");
                        return;
                    }

                    value.Outer = outer;
                }
                else
                {
                    //logger.Warn("There is a unitOfWorkKey in CallContext but not in UnitOfWorkDictionary (on SetCurrentUow)! UnitOfWork key: " + unitOfWorkKey);
                }
            }

            unitOfWorkKey = value.Id;
            if (!UnitOfWorkDictionary.TryAdd(unitOfWorkKey, value))
            {
                throw new ArticleException("Can not set unit of work! UnitOfWorkDictionary.TryAdd returns false!");
            }

            CallContext.LogicalSetData(ContextKey, unitOfWorkKey);
            //IUnitOfWork unitofwork = CallContext.GetData(ContextKey) as IUnitOfWork;
            //if (unitofwork == null)
            //{
            //    CallContext.SetData(ContextKey, value);
            //}

        }

        private void ExitFromCurrentUowScope()
        {
            var unitOfWorkKey = CallContext.LogicalGetData(ContextKey) as string;
            if (unitOfWorkKey == null)
            {
                //_logger.WriteWarn("There is no current UOW to exit!");
                return;
            }

            IUnitOfWork unitOfWork;
            if (!UnitOfWorkDictionary.TryGetValue(unitOfWorkKey, out unitOfWork))
            {
                //logger.Warn("There is a unitOfWorkKey in CallContext but not in UnitOfWorkDictionary (on ExitFromCurrentUowScope)! UnitOfWork key: " + unitOfWorkKey);
                CallContext.FreeNamedDataSlot(ContextKey);
                return;
            }

            UnitOfWorkDictionary.TryRemove(unitOfWorkKey, out unitOfWork);
            if (unitOfWork.Outer == null)
            {
                CallContext.FreeNamedDataSlot(ContextKey);
                return;
            }

            //Restore outer UOW

            var outerUnitOfWorkKey = unitOfWork.Outer.Id;
            if (!UnitOfWorkDictionary.TryGetValue(outerUnitOfWorkKey, out unitOfWork))
            {
                //No outer UOW
                //_logger.WriteWarn("Outer UOW key could not found in UnitOfWorkDictionary!");
                CallContext.FreeNamedDataSlot(ContextKey);
                return;
            }

            CallContext.LogicalSetData(ContextKey, outerUnitOfWorkKey);
        }
    }
}