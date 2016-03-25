using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Commander
{
   [AttributeUsage(AttributeTargets.Property)]
public class FlagArgumentAttribute : Attribute, IArgumentDescription
{
    // Methods
    public FlagArgumentAttribute(string shortAlias, string description)
    {
        this.ShortAlias = shortAlias;
        this.Description = description;
    }

    // Properties
    public string Description { get; set; }
    public bool Optional{get{return false;}}
    public string ShortAlias { get; set; }
}


}
