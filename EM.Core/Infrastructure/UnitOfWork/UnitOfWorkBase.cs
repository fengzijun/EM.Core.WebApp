using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EM.Infrastructure.UnitOfWork
{
    public abstract class UnitOfWorkBase : IUnitOfWork
    {
       
        public bool IsBeginCalledBefore { get; set; }

        public bool IsCompleteCalledBefore { get; set; }

        public bool Success { get; set; }

        public Exception InnerException { get; set; }

        public UnitOfWorkBase()
        {
            Logger = EngineContext.Current.Resolve<ILogger>();
            Id = Guid.NewGuid().ToString("N");
        }

        public ILogger Logger;

        public bool IsDisposed { get; set; }

        public abstract void CompleteUow();

        public abstract Task CompleteUowAsync();

        public abstract void DisposeUow();

        public abstract void RollBack();

        public abstract void Complete(UnitOfWorkOptions options);

        public UnitOfWorkOptions Options { get; set; }

        public IUnitOfWork Outer { get; set; }

        public void Complete()
        {
            PreventMultipleComplete();
            try
            {
                CompleteUow();

                //if(GetType() == typeof(InnerUnitOfWork) && Outer!=null)
                //{
                //    Outer.Complete(Options);
                //}

                Success = true;
            }
            catch (Exception ex)
            {
                InnerException = ex;
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task CompleteAsync()
        {
            PreventMultipleComplete();
            try
            {
                await CompleteUowAsync();
                //if (GetType() == typeof(InnerUnitOfWork) && Outer != null)
                //{
                //    Outer.Complete(Options);
                //}
                Success = true;
            }
            catch (Exception ex)
            {
                InnerException = ex;
                throw;
            }
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            if (!IsBeginCalledBefore || IsDisposed)
            {
                return;
            }

            IsDisposed = true;

            if (!Success)
            {
                //OnFailed(_exception);
                //if(Options!=null && Options.IsTransactional == true)
                //    RollBack();
                if(InnerException!=null)
                    Logger.WriteErrorLog(InnerException);
            }

            DisposeUow();
        }

        public void PreventMultipleBegin()
        {
            if (IsBeginCalledBefore)
            {
                throw new ArticleException("This unit of work has started before. Can not call Start method more than once.");
            }

            IsBeginCalledBefore = true;
        }

        public void PreventMultipleComplete()
        {
            if (IsCompleteCalledBefore)
            {
                throw new ArticleException("Complete is called before!");
            }

            IsCompleteCalledBefore = true;
        }

        public string Id { get; }


        public void Begin(UnitOfWorkOptions options)
        {
            Options = options;
            PreventMultipleBegin();
            BeginUow();
        }

        public abstract void BeginUow();
    
        public override string ToString()
        {
            return $"[UnitOfWork {Id}]";
        }
    }
}