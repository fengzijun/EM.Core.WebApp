using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace EM.Infrastructure.UnitOfWork
{
    public class InnerUnitOfWork : UnitOfWorkBase
    {
        public const string DidNotCallCompleteMethodExceptionMessage = "Did not call Complete method of a unit of work.";

    
      
        public InnerUnitOfWork(UnitOfWorkOptions options) 
        {
            Options = options;
        }

        public override void Complete(UnitOfWorkOptions options)
        {
           

        }

        public new void Begin(UnitOfWorkOptions options)
        {
            Options = options;
            IsBeginCalledBefore = true;
            BeginUow();
        }

        public override void CompleteUow()
        {

        }

        public override Task CompleteUowAsync()
        {
            return Task.FromResult(0);
        }

        public override void DisposeUow()
        {
        }

        public override void BeginUow()
        {
            //throw new NotImplementedException();
        }

        public override void RollBack()
        {
            //throw new NotImplementedException();
        }

        public void Complete()
        {
            IsCompleteCalledBefore = true;
        }

        public Task CompleteAsync()
        {
            IsCompleteCalledBefore = true;
            return Task.FromResult(0);
        }

        public void Dispose()
        {
            if (IsDisposed)
            {
                return;
            }

            IsDisposed = true;

            if (!IsCompleteCalledBefore)
            {
                if (HasException())
                {
                    return;
                }

                throw new ArticleException(DidNotCallCompleteMethodExceptionMessage);
            }
        }

        private static bool HasException()
        {
            try
            {
                return Marshal.GetExceptionCode() != 0;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}