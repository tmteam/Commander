using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheGin
{
    [AttributeUsage(AttributeTargets.Property)]
    public class SettingAttribute : Attribute
    {
        public SettingAttribute() { }
        public SettingAttribute(string shortAlias, string description)
        {
            this.Optional = true;
            this.ShortAlias = shortAlias;
            this.Description = description;
        }

        public string Description { get; set; }
        public bool Optional { get; set; }
        public string ShortAlias { get; set; }
    }


}
