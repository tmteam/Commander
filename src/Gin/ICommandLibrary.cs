using System;
using System.Collections.Generic;
namespace TheGin
{
    /// <summary>
    /// Command sketch Library 
    /// </summary>
    public interface ICommandLibrary
    {
        /// <summary>
        /// Returns command sketch for the specified command name or null
        /// </summary>
        CommandSketch GetOrNull(string commandName);
        /// <summary>
        /// Registrates new singletone command sketch 
        /// </summary>
        void Registrate(ICommand exemplar);
        /// <summary>
        /// Registrates new command sketch for specified command type
        /// </summary>
        void Registrate(Type cmdType);
        /// <summary>
        /// Removes command sketch of the specified name
        /// </summary>
        void Deregistrate(string commandName);
        /// <summary>
        /// Known command sketches
        /// </summary>
        IEnumerable<CommandSketch> Sketches { get; } 
    }
}
