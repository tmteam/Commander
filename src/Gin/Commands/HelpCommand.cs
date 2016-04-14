using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheGin
{
    [Command("Show allowed commands help")]
    public class HelpCommand : CommandBase
    {
        public readonly IEnumerable<CommandSketch> Commands;

        public HelpCommand(IEnumerable<CommandSketch> commands)
        {
            this.Commands = commands;
        }

        private void fillArgDescription(StringBuilder msg, ArgumentDescription arg)
        {
            msg.AppendLine("    -" + arg.Attribute.ShortAlias + ",  (" + arg.Property.PropertyType.Name + ")   " + arg.Attribute.Description);
        }

        public override void Run()
        {
            StringBuilder msg = new StringBuilder("Threre are " + this.Commands.Count<CommandSketch>() + " commands: \r\n");
            foreach (CommandSketch description in this.Commands)
            {
                msg.AppendLine("[" + ParseTools.GetCommandName(description.CommandType) + "] - " + description.Attribute.Description);
                ArgumentDescription[] descriptionArray = (from a in description.Arguments
                                                          where !a.Attribute.Optional
                                                          select a).ToArray<ArgumentDescription>();
                if (descriptionArray.Length > 0)
                {
                    msg.AppendLine("neccessary: ");
                    foreach (ArgumentDescription description2 in descriptionArray)
                    {
                        this.fillArgDescription(msg, description2);
                    }
                }
                ArgumentDescription[] descriptionArray2 = (from a in description.Arguments
                                                           where a.Attribute.Optional
                                                           select a).ToArray<ArgumentDescription>();
                if (descriptionArray2.Length > 0)
                {
                    msg.AppendLine("optional: ");
                    foreach (ArgumentDescription description2 in descriptionArray2)
                    {
                        this.fillArgDescription(msg, description2);
                    }
                }
            }
            this.Log.WriteMessage(msg.ToString());
        }
    }


}
