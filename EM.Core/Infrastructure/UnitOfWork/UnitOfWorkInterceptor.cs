using Castle.DynamicProxy;
using EM.ComponentModel;
using EM.Data;
using EM.Infrastructure.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EM.Infrastructure.UnitOfWork
{
    public class UnitOfWorkInterceptor : IInterceptor
    {
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public UnitOfWorkInterceptor()
        {
            _unitOfWorkManager = EngineContext.Current.Resolve<IUnitOfWorkManager>();
        }

        public UnitOfWorkInterceptor(IUnitOfWorkManager unitOfWorkManager)
        {
            _unitOfWorkManager = unitOfWorkManager;
        }

        /// <summary>
        /// Intercepts a method.
        /// </summary>
        /// <param name="invocation">Method invocation arguments</param>
        public void Intercept(IInvocation invocation)
        {
            var unitOfWorkAttr = GetUnitOfWorkAttributeOrNull(invocation.MethodInvocationTarget);
            if (unitOfWorkAttr == null || unitOfWorkAttr.IsDisabled)
            {
                //No need to a uow
                invocation.Proceed();
                return;
            }

            //No current uow, run a new one
            PerformUow(invocation, unitOfWorkAttr.CreateOptions());
        }

        private void PerformUow(IInvocation invocation, UnitOfWorkOptions options)
        {
            if (AsyncHelper.IsAsyncMethod(invocation.Method))
            {
                PerformAsyncUow(invocation, options);
            }
            else
            {
                PerformSyncUow(invocation, options);
            }
        }

        private void PerformSyncUow(IInvocation invocation, UnitOfWorkOptions options)
        {
            using (var uow = _unitOfWorkManager.Begin(options))
            {
                invocation.Proceed();
                uow.Complete();
            }
        }

        private void PerformAsyncUow(IInvocation invocation, UnitOfWorkOptions options)
        {
            var uow = _unitOfWorkManager.Begin(options);

            try
            {
                invocation.Proceed();
            }
            catch
            {
                uow.Dispose();
                throw;
            }

            if (invocation.Method.ReturnType == typeof(Task))
            {
                invocation.ReturnValue = InternalAsyncHelper.AwaitTaskWithPostActionAndFinally(
                    (Task)invocation.ReturnValue,
                    async () => await uow.CompleteAsync(),
                    exception => uow.Dispose()
                );
            }
            else //Task<TResult>
            {
                invocation.ReturnValue = InternalAsyncHelper.CallAwaitTaskWithPostActionAndFinallyAndGetResult(
                    invocation.Method.ReturnType.GenericTypeArguments[0],
                    invocation.ReturnValue,
                    async () => await uow.CompleteAsync(),
                    exception => uow.Dispose()
                );
            }
        }

        private UnitOfWorkAttribute GetUnitOfWorkAttributeOrNull(MethodInfo methodInfo)
        {
            var attrs = methodInfo.GetCustomAttributes(true).OfType<UnitOfWorkAttribute>().ToArray();
            if (attrs.Length > 0)
            {
                return attrs[0];
            }

            attrs = methodInfo.DeclaringType.GetTypeInfo().GetCustomAttributes(true).OfType<UnitOfWorkAttribute>().ToArray();
            if (attrs.Length > 0)
            {
                return attrs[0];
            }

            if (IsConventionalUowClass(methodInfo.DeclaringType))
            {
                return new UnitOfWorkAttribute(); //Default
            }

            return null;
        }

        private bool IsConventionalUowClass(Type type)
        {
            var ConventionalUowSelectors = new List<Func<Type, bool>>
            {
                mytype => typeof(EM.Data.IRepository<>).IsAssignableFrom(type) ||
                        typeof(IApplicationService).IsAssignableFrom(type)
            };
            return ConventionalUowSelectors.Any(selector => selector(type));
        }
    }
}