using System;
namespace TheGin
{
    public interface ICommandLibrary
    {
        System.Collections.Generic.IEnumerable<CommandDescription> Descriptions { get; }
        CommandDescription GetOrNull(string name);
        void Registrate(ICommand exemplar);
        void Registrate(Type cmdType);
    }
}
