using Castle.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace EM
{
    public class NoLockIInterceptor : IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {
            //var transactionOptions = new TransactionOptions();
            ////set it to read uncommited
            //transactionOptions.IsolationLevel = IsolationLevel.ReadUncommitted;
            ////create the transaction scope, passing our options in
            //using (var transactionScope = new TransactionScope(TransactionScopeOption.Required, transactionOptions))
            //{
            //    //declare our context
            //    using (var scope = new TransactionScope())
            //    {
            //
            //        scope.Complete();
            //    }
            //}

            invocation.Proceed();
        }
    }
}