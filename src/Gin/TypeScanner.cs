using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TheGin
{
    public class TypeScanner : ICommandLibrary
    {
        Dictionary<string, CommandDescription> commands 
            = new Dictionary<string, CommandDescription>();

        public CommandDescription GetOrNull(string name)
        {
            name = name.ToLower();
            if (this.commands.ContainsKey(name))
            {
                return this.commands[name];
            }
            return null;
        }

        public void Registrate<T>() where T: ICommand, new()
        {
            this.Registrate(typeof(T));
        }

       
        public void Registrate(ICommand exemplar)
        {
            CommandDescription description = new CommandDescription {
                Arguments = ReflectionTools.GetArgumentsDescription(exemplar.GetType()),
                Attribute = ReflectionTools.GetCommandAttributeOrThrow(exemplar.GetType()),
                Exemplar  = exemplar
            };
            this.Registrate(description);
        }

        public void Registrate(Type type)
        {
            ReflectionTools.ThrowIfItIsNotValidCommand(type);
            var cmdAttribute = ReflectionTools.GetCommandAttributeOrThrow(type);
            this.Registrate(type, cmdAttribute);
        }

        void Registrate(Type type, CommandAttribute attribute)
        {
            ReflectionTools.ThrowIfItIsNotValidCommand(type);
            var description = new CommandDescription {
                Attribute = attribute,
                Arguments = ReflectionTools.GetArgumentsDescription(type),
                Exemplar  = (ICommand) Activator.CreateInstance(type)
            };
            this.Registrate(description);
        }

        void Registrate(CommandDescription description)
        {
            string key = ParseTools.NormalizeCommandTypeName(description.Exemplar.GetType().Name).ToLower();
            this.commands.Add(key, description);
        }

        public void ScanAssembly(Assembly assembly)
        {
            foreach (var type  in assembly.ExportedTypes.Where(t=>typeof(ICommand).IsAssignableFrom(t)))
            {
                var customAttribute = type.GetCustomAttribute<CommandAttribute>();
                if (customAttribute != null) {
                    this.Registrate(type, customAttribute);
                }
            }
        }

        // Properties
        public IEnumerable<CommandDescription> Descriptions {
            get { return this.commands.Values; }
        }
    }
}
