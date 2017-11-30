using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EM.Infrastructure.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        bool IsDisposed { get; }


        void Complete();

        Task CompleteAsync();

        UnitOfWorkOptions Options { get; }

        string Id { get; }

        IUnitOfWork Outer { get; set; }

        void Begin(UnitOfWorkOptions options);

        void Complete(UnitOfWorkOptions options);
    }
}