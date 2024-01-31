using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace ProceduralPlant.Core
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class SymbolAttribute : Attribute
    {
        public readonly string name = null;
        
        public SymbolAttribute(string name)
        {
            this.name = name;
        }
    }

    public struct DiameterRange
    {
        public float start;
        public float end;
        
        public static DiameterRange none => new();
    }
    
    public class Symbol : Node
    {
        internal static readonly Dictionary<string, Descriptor> descriptors = new();

        internal DiameterRange diameterRange;

        public string name { get; private set; } = null;

        public Descriptor descriptor => descriptors.GetValueOrDefault(name);

        public Symbol()
        {
        }

        public Symbol(string name)
        {
            this.name = name;
        }
        
        static Symbol()
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var type in assembly.GetTypes())
                {
                    var symbolAttribute = type.GetCustomAttribute<SymbolAttribute>();
                    if (symbolAttribute == null)
                    {
                        continue;
                    }
                    var descriptor = Activator.CreateInstance(type) as Descriptor;
                    descriptor!.name = symbolAttribute.name;
                    Debug.Assert(descriptor.name != "#");
                    descriptors.Add(symbolAttribute.name, descriptor);
                }
            }
        }

        public override Node Clone()
        {
            var symbol = new Symbol();
            symbol.name = this.name;
            symbol.next = this.next?.Clone();
            return symbol;
        }

        public override string ToString()
        {
            return this.name + next;
        }
    }
}
