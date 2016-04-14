using Gin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gin.Tests
{
    [Command("someCommandDescription")]
    public class TestAttributesCommand: CommandBase
    {
        [CommandArgument("int", "someIntegerDescription")]
        public int IntProperty { get; set; }

        [CommandArgument("str","someStringDescription", Optional = false)]
        public string StringProperty { get; set; }

        [FlagArgument("flag","someFlagDescription")]
        public bool BoolProperty { get; set; }
        public override void Run()
        {

        }
    }
}
