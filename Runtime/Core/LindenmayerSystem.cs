using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProceduralPlant.Symbols;
using UnityEditor;
using UnityEngine;

namespace ProceduralPlant.Core
{
    public class LindenmayerSystem
    {
        public ParametersInfo parametersInfo { get; private set; }
        
        private Node axiom = null;
        
        private readonly Dictionary<string, List<Production>> productions = new();

        public Node current { get; private set; } = null;
        
        public static LindenmayerSystem Compile(string text, ParametersInfo parametersInfo, StringBuilder error)
        {
            var lindenmayerSystem = new LindenmayerSystem();
            var context = new CompilationContext(text);

            if (!ParsingUtility.Axiom(context, out var axiom))
            {
                return null;
            }
            lindenmayerSystem.axiom = axiom;

            while (ParsingUtility.Production(context, out var symbol, out var structure, out float probability, error))
            {
                if (!lindenmayerSystem.productions.TryGetValue(symbol.name, out var list))
                {
                    lindenmayerSystem.productions.Add(symbol.name, list = new());
                }
                list.Add(new Production(structure, probability));
            }
            if (!context.finished)
            {
                error?.Append("\nCompilation interrupted by error.");
                return null;
            }

            foreach (var (name, list) in lindenmayerSystem.productions)
            {
                float sum = 0;
                foreach (var production in list)
                {
                    sum += production.probability;
                }
                if (!Mathf.Approximately(sum, 1))
                {
                    error.Append($"\nSum of {name}'s probabilities must be 1.");
                    return null;
                }
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
                    if (this.productions.TryGetValue(symbol.name, out var list))
                    {
                        Node structure = null;
                        if (list.Count == 1)
                        {
                            structure = list[0].structure;
                        }
                        else
                        {
                            float randomValue = Random.Range(0.0f, 1.0f);
                            foreach (var production in list)
                            {
                                randomValue -= production.probability;
                                if (randomValue <= 0)
                                {
                                    structure = production.structure;
                                    break;
                                }
                            }
                            structure ??= list[^1].structure;
                        }
                        node = structure.Clone();
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
            Random.InitState(parametersInfo.randomSeed);
            this.current = Simulate(this.current);
            return this;
        }

        public void MarkOrganFlags()
        {
            MarkOrganFlags(this.current);
        }

        private static OrganFlags MarkOrganFlags(Node node)
        {
            if (node == null)
            {
                return OrganFlags.Tip;
            }
            node.organFlags = OrganFlags.None;
            var propagation = MarkOrganFlags(node.next);
            switch (node)
            {
                case Symbol symbol:
                    if (symbol.descriptor?.GetType() == typeof(MoveForwardWithLine))
                    {
                        node.organFlags |= OrganFlags.Branch;
                        node.organFlags |= (propagation & OrganFlags.Tip);
                        propagation &= ~OrganFlags.Tip;
                    }
                    break;
                case Polygon:
                    node.organFlags |= OrganFlags.Leaf;
                    break;
                case Branch branch:
                    if (!MarkOrganFlags(branch.content).HasFlag(OrganFlags.Tip))
                    {
                        propagation &= ~OrganFlags.Tip;
                    }
                    break;
            }
            return propagation;
        }

        public override string ToString()
        {
            return axiom.ToString() + ";" + string.Concat(productions.Select((pair, _) => $"{pair.Key}->{pair.Value.ToString()};"));
        }
    }
}

