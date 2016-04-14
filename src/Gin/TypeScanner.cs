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
        Dictionary<string, CommandSketch> commands 
            = new Dictionary<string, CommandSketch>();

        public CommandSketch GetOrNull(string name)
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
            var sketch = new CommandSketch(
                attribute:      ReflectionTools.GetCommandAttributeOrThrow(exemplar.GetType()),
                commandType:    exemplar.GetType(),
                factory:        ()=>exemplar
            );
            this.Registrate(sketch);
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
            var description = new CommandSketch(attribute, type, () => (ICommand)Activator.CreateInstance(type));
            this.Registrate(description);
        }

        void Registrate(CommandSketch sketch)
        {
            string key = ParseTools.GetCommandName(sketch.CommandType).ToLower();
            this.commands.Add(key, sketch);
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
        public IEnumerable<CommandSketch> Sketches {
            get { return this.commands.Values; }
        }
    }
}
