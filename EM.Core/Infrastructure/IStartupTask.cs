namespace EM.Infrastructure
{
    /// <summary>
    /// 程序 开始 任务
    /// </summary>
    public interface IStartupTask
    {
        /// <summary>
        /// Executes a task
        /// </summary>
        void Execute();

        /// <summary>
        /// Order
        /// </summary>
        int Order { get; }
    }
}