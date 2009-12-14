namespace Caliburn.Core.Threading
{
    using System.Threading;

    /// <summary>
    /// A service that implements a thread pool.
    /// </summary>
    public interface IThreadPool
    {
        /// <summary>
        /// Queues the user work item.
        /// </summary>
        /// <param name="callback">The work callback.</param>
        /// <param name="userState">The user state.</param>
        /// <returns></returns>
        bool QueueUserWorkItem(WaitCallback callback, object userState);
    }
}