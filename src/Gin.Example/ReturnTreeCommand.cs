using TheGin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheGin.ComplexExample
{
    [Command("Generate tree")]
    public class ReturnTreeCommand: FuncCommandBase<Node>
    {
        protected override Node RunAndReturn()
        {
            Log.WriteMessage("Runing...");
            return new Node { Name = "root",
                 Children = new [] { 
                     new Node{ Name = "1A",
                         Children = new []{
                             new Node {Name = "2A"},
                             new Node {Name = "2B"}}},
                     new Node{ Name = "1B",
                         Children = new []{
                             new Node {Name = "2A"}}},
                     new Node {Name = "1C"}}
            };
        }
    }

    public class Node {
        [Result] public string Name { get; set; }
        [Result] public IEnumerable<Node> Children { get; set; }
    }

}
