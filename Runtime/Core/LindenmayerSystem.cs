using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace ProceduralPlant.Core
{
    public class LindenmayerSystem
    {
        public ParametersInfo parameterInfo { get; private set; }
        
        private Node axiom = null;
        
        private readonly Dictionary<string, Node> productions = new();

        public Node current { get; private set; } = null;
        
        public static LindenmayerSystem Compile(string text, ParametersInfo parameterInfo)
        {
            var lindenmayerSystem = new LindenmayerSystem();
            var context = new CompilationContext(text);

            if (!ParsingUtility.Axiom(context, out var axiom))
            {
                return null;
            }
            lindenmayerSystem.axiom = axiom;

            while (ParsingUtility.Production(context, out var symbol, out var structure))
            {
                lindenmayerSystem.productions[symbol.name] = structure;
            }
            if (!context.finished)
            {
                return null;
            }

            lindenmayerSystem.current = lindenmayerSystem.axiom.Clone();
            lindenmayerSystem.parameterInfo = parameterInfo;
            return lindenmayerSystem;
        }

        private Node Simulate(Node node)
        {
            if (node == null)
            {
                return null;
            }
            var old = node;
            switch (node)
            {
                case Symbol symbol:
                    if (this.productions.TryGetValue(symbol.name, out var production))
                    {
                        node = production.Clone();
                        node.last.next = Simulate(old.next);
                    }
                    else
                    {
                        node.next = Simulate(old.next);
                    }
                    break;
                case Branch branch:
                    branch.content = Simulate(branch.content);
                    branch.next = Simulate(old.next);
                    break;
                case Polygon polygon:
                    polygon.content = Simulate(polygon.content);
                    polygon.next = Simulate(old.next);
                    break;
            }
            return node;
        }

        public LindenmayerSystem Simulate()
        {
            this.current = Simulate(this.current);
            return this;
        }

        public override string ToString()
        {
            return axiom.ToString() + ";" + string.Concat(productions.Select((pair, _) => $"{pair.Key}->{pair.Value.ToString()};"));
        }
    }
}

