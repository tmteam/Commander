using System;
using System.Collections.Generic;
namespace TheGin
{
    public interface ICommandLibrary
    {
        CommandSketch GetOrNull(string commandName);
        void Registrate(ICommand exemplar);
        void Registrate(Type cmdType);
        void Deregistrate(string commandName);
        IEnumerable<CommandSketch> Sketches { get; } 
    }
}
