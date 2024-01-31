using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProceduralPlant.Symbols;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ProceduralPlant.Core
{
    public class LindenmayerSystem
    {
        private Node axiom = null;
        
        private readonly Dictionary<string, List<Production>> productions = new();

        public Node current { get; private set; } = null;
        
        public static LindenmayerSystem Compile(string text, StringBuilder error)
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

        public LindenmayerSystem Simulate(int randomSeed)
        {
            Random.InitState(randomSeed);
            this.current = Simulate(this.current);
            return this;
        }

        public void Pregenerate(PlantSpecies species)
        {
            PregenerateOrganFlags(this.current);
            PregenerateDiameter(species, this.current, 0, 0);
        }

        private static OrganFlags PregenerateOrganFlags(Node node)
        {
            if (node == null)
            {
                return OrganFlags.Tip;
            }
            node.organFlags = OrganFlags.None;
            var propagation = PregenerateOrganFlags(node.next);
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
                    if (!PregenerateOrganFlags(branch.content).HasFlag(OrganFlags.Tip))
                    {
                        propagation &= ~OrganFlags.Tip;
                    }
                    break;
            }
            return propagation;
        }

        private static float PregenerateDiameter(PlantSpecies species, Node node, float forward, int deep)
        {
            float backward = float.PositiveInfinity;
            if (node == null)
                return backward;
            if (node is Symbol { descriptor: DiameterDecrease })
            {
                forward = 0;
                backward = 0;
                deep++;
            }
            float forward_ = forward;
            if (node is Symbol { descriptor: MoveForwardWithLine })
                forward++;
            backward = Mathf.Min(backward, PregenerateDiameter(species, node.next, forward, deep));
            if (node is Branch branch)
                backward = Mathf.Min(backward, PregenerateDiameter(species, branch.content, forward, deep));
            float backward_ = backward;
            if (node is Symbol { descriptor: MoveForwardWithLine })
                backward++;
            if (node is Symbol { descriptor: MoveForwardWithLine } symbol )
            {
                float p = 1 - species.thinningRate;
                float from = species.initialDiameter * Mathf.Pow(p, deep);
                float to = species.initialDiameter * Mathf.Pow(p, float.IsPositiveInfinity(backward_) ? deep : deep + 1);
                float total = backward_ + forward_ + 1;
                float delta = to - from;
                if (float.IsPositiveInfinity(backward_))
                {
                    if (float.IsPositiveInfinity(total))
                        symbol.diameterRange = new DiameterRange() { start = from + delta, end = from + delta };
                    else
                        throw new Exception();
                }
                else
                {
                    if (float.IsPositiveInfinity(total))
                        symbol.diameterRange = new DiameterRange() { start = from, end = from };
                    else
                        symbol.diameterRange = new DiameterRange
                        {
                            start = from + delta * (float)(1 - (backward_ + 1) / (float)total),
                            end = from + delta * (float)(1 - backward_ / (float)total)
                        };
                }
            }
            return backward;
        }

        public override string ToString()
        {
            return axiom.ToString() + ";" + string.Concat(productions.Select((pair, _) => $"{pair.Key}->{pair.Value.ToString()};"));
        }
    }
}

