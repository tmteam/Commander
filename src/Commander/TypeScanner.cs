using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Commander
{
    public class TypeScanner
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

        public void Registrate(CommandDescription description)
        {
            string key = ParseTools.NormalizeCommandTypeName(description.type.Name).ToLower();
            this.commands.Add(key, description);
        }

        public void Registrate(ICommand exemplar)
        {
            CommandDescription description = new CommandDescription {
                arguments = ReflectionTools.GetArgumentsDescription(exemplar.GetType()),
                attribute = ReflectionTools.GetCommandAttributeOrThrow(exemplar.GetType()),
                exemplarFactory = new SingletoneCommandAbstractFactory(exemplar),
                type = exemplar.GetType()
            };
            this.Registrate(description);
        }

        public void Registrate(Type type)
        {
            CommandAttribute commandAttributeOrThrow = ReflectionTools.GetCommandAttributeOrThrow(type);
            if (!typeof(ICommand).IsAssignableFrom(type))
            {
                throw new ArgumentException("Does not implement ICommand");
            }
            if (type.GetConstructor(new Type[0]) == null)
            {
                throw new ArgumentException("Got no empty constructor");
            }
            this.Registrate(type, commandAttributeOrThrow);
        }

        private void Registrate(Type type, CommandAttribute attribute)
        {
            var description = new CommandDescription {
                type = type,
                attribute = attribute,
                arguments = ReflectionTools.GetArgumentsDescription(type),
                exemplarFactory = new ReflectionCommandAbstractFactory(type)
            };
            this.Registrate(description);
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
