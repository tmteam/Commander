using TheGin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheGin.Tests
{
    [Command("someCommandDescription")]
    public class TestAttributesCommand: CommandBase
    {
        [Setting("int", "someIntegerDescription")]
        public int IntProperty { get; set; }

        [Setting("str","someStringDescription", Optional = false)]
        public string StringProperty { get; set; }

        [Setting("flag", "someFlagDescription")]
        public bool BoolProperty { get; set; }
        public override void Run()
        {

        }
    }
}
