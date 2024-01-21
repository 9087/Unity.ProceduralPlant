using System;

namespace ProceduralPlant.Core
{
    internal class CompilationContext
    {
        private string text { get; set; }

        public char current => finished ? char.MinValue : text[index];

        public int index { get; private set; } = 0;

        public bool finished => index >= text.Length;
            
        public CompilationContext(string text)
        {
            this.text = text;
        }

        public char Step()
        {
            this.index++;
            return current;
        }
        
        public class Transaction : IDisposable
        {
            private int index;

            private CompilationContext context;
            
            public Transaction(CompilationContext context)
            {
                this.context = context;
                this.index = this.context.index;
            }

            public void Dispose()
            {
                if (this.context == null) return;
                this.context.index = this.index;
            }

            public string Finish()
            {
                string text = this.context.text.Substring(index, this.context.index - index);
                this.context = null;
                return text;
            }
        }

        public Transaction CreateTransaction()
        {
            return new Transaction(this);
        }
    }
}
