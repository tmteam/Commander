using System;
namespace TheGin
{
    /// <summary>
    /// Launches commands
    /// </summary>
    public interface IExecutor: ILoggable
    {
        /// <summary>
        /// Executes the command in try catch block
        /// </summary>
        void Run(ICommand cmd);
    }
}
