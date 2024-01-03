using System.Linq;
using System.Text;

namespace ProceduralPlant.Core
{
    internal static class ParsingUtility
    {
        private static bool Letter(CompilationContext context, out char letter)
        {
            letter = context.current;
            if (!(letter is >= 'a' and <= 'z' or >= 'A' and <= 'Z')) return false;
            context.Step();
            return true;
        }

        private static bool Digit(CompilationContext context, out char digit)
        {
            digit = context.current;
            if (!(digit is >= '0' and <= '9')) return false;
            context.Step();
            return true;
        }

        private static bool Blank(CompilationContext context)
        {
            bool matched = false;
            while (context.current == ' ')
            {
                matched = true;
                context.Step();
            }
            return matched;
        }

        private static bool Predefined(CompilationContext context, out Symbol symbol)
        {
            using var transaction = context.CreateTransaction();
            symbol = null;
            var descriptors = ProceduralPlant.Core.Symbol.descriptors.Values.ToList();
            int length = 0;
            while (!context.finished)
            {
                descriptors = descriptors.Where(s => s.name[length] == context.current).ToList();
                context.Step();
                if (descriptors.Count <= 1)
                {
                    break;
                }
                length++;
            }
            if (descriptors.Count != 1)
            {
                return false;
            }
            var descriptor = descriptors.First();
            symbol = new Symbol(descriptor.name);
            transaction.Finish();
            return true;
        }
        
        private static bool Symbol(CompilationContext context, out Symbol symbol)
        {
            if (Predefined(context, out symbol))
            {
                return true;
            }
            else if (Letter(context, out var letter))
            {
                symbol = new Symbol(new StringBuilder().Append(letter).ToString());
                return true;
            }
            else
            {
                symbol = null;
                return false;
            }
        }

        private static bool Branch(CompilationContext context, out Node branch)
        {
            using var transaction = context.CreateTransaction();
            branch = null;
            if (context.current != '[') return false;
            context.Step();
            if (!Structure(context, out var node)) return false;
            if (context.current != ']') return false;
            context.Step();
            transaction.Finish();
            branch = new Branch(node);
            return true;
        }

        private static bool Polygon(CompilationContext context, out Node polygon)
        {
            using var transaction = context.CreateTransaction();
            polygon = null;
            if (context.current != '{') return false;
            context.Step();
            if (!Structure(context, out var node)) return false;
            if (context.current != '}') return false;
            context.Step();
            transaction.Finish();
            polygon = new Polygon(node);
            return true;
        }
        
        private static bool Semicolon(CompilationContext context)
        {
            if (context.current == ';')
            {
                context.Step();
                return true;
            }
            else
            {
                return false;
            }
        }

        private static bool Structure(CompilationContext context, out Node structure)
        {
            structure = null;
            Node last = null;
            while (!context.finished)
            {
                if (Blank(context)) continue;
                Node node = null;
                if (Symbol(context, out var symbol))
                {
                    node = symbol;
                }
                else if (Branch(context, out var branch))
                {
                    node = branch;
                }
                else if (Polygon(context, out var polygon))
                {
                    node = polygon;
                }
                structure ??= node;
                if (node != null)
                {
                    if (last != null) last.next = node;
                    last = node;
                    continue;
                }
                break;
            }
            return structure != null;
        }

        public static bool Axiom(CompilationContext context, out Node axiom)
        {
            return Structure(context, out axiom) && Semicolon(context);
        }

        public static bool Production(CompilationContext context, out Symbol symbol,
            out Node structure)
        {
            using var transaction = context.CreateTransaction();
            symbol = null;
            structure = null;
            Blank(context);
            if (!Symbol(context, out symbol)) return false;
            Blank(context);
            if (context.current != '-') return false;
            context.Step();
            if (context.current != '>') return false;
            context.Step();
            Blank(context);
            if (!Structure(context, out structure)) return false;
            if (!Semicolon(context)) return false;
            transaction.Finish();
            return true;
        }
    }
}
