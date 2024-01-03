using System.Collections.Generic;
using System.Linq;
using ProceduralPlant.Symbols;
using UnityEditor;
using UnityEngine;

namespace ProceduralPlant.Core
{
    public class LindenmayerSystem
    {
        public ParametersInfo parametersInfo { get; private set; }
        
        private Node axiom = null;
        
        private readonly Dictionary<string, Node> productions = new();

        public Node current { get; private set; } = null;
        
        public static LindenmayerSystem Compile(string text, ParametersInfo parametersInfo)
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
            lindenmayerSystem.parametersInfo = parametersInfo;
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

        public void MarkOrganFlags()
        {
            MarkOrganFlags(this.current);
        }

        private static void MarkOrganFlags(Node node)
        {
            node.organFlags = Node.OrganFlags.None;
            if (node.next != null)
            {
                MarkOrganFlags(node.next);
                if (!(node.next is Symbol symbol) || symbol.descriptor == null || symbol.descriptor.GetType() != typeof(MoveForwardWithLine))
                {
                    node.organFlags |= node.next.organFlags & Node.OrganFlags.Tip;
                }
            }
            else
            {
                node.organFlags |= Node.OrganFlags.Tip;
            }
            switch (node)
            {
                case Symbol symbol:
                    break;
                case Branch branch:
                    MarkOrganFlags(branch.content);
                    break;
            }
        }

        public override string ToString()
        {
            return axiom.ToString() + ";" + string.Concat(productions.Select((pair, _) => $"{pair.Key}->{pair.Value.ToString()};"));
        }
    }
}

