using System;
namespace TheGin
{
    public interface IExecutor: ILoggable
    {
        void Run(ICommand cmd);
    }
}
