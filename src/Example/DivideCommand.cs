using Commander;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Example
{
    [Command("a/b math operation")]
    public class DivideCommand: CommandBase<double>
    {
        [CommandArgument("a", "Double, that is divided by divider (b) ")]
        public double Dividend { get; set; }
        [CommandArgument("b", "Double, who divide divider (a)")]
        public double Divider { get; set; }
        

        protected override double RunAndReturn()
        {
            Log.WriteMessage("output from the inside of command ");
            if (Divider == 0) {
                Log.WriteError("Divider is 0. Answer will be equal to Double.NaN");
                return Double.NaN;
            }
            return Dividend / Divider;
        }
    }
}
