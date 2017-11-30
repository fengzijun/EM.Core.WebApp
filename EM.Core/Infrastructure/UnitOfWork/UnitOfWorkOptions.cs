using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace EM.Infrastructure.UnitOfWork
{
    public class UnitOfWorkOptions
    {
        public TransactionScopeOption? Scope { get; set; }

        public bool? IsTransactional { get; set; }

        public TimeSpan? Timeout { get; set; }

        public IsolationLevel? IsolationLevel { get; set; }

        public TransactionScopeAsyncFlowOption? AsyncFlowOption { get; set; }

        /// <summary>
        /// Creates a new <see cref="UnitOfWorkOptions"/> object.
        /// </summary>
        public UnitOfWorkOptions()
        {
        }
    }
}