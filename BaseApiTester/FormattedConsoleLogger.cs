using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tesseract;

namespace BaseApiTester
{
    public class FormattedConsoleLogger
    {
        private class Scope : DisposableBase
        {
            private int indentLevel;
            private string indent;
            private FormattedConsoleLogger container;

            public Scope(FormattedConsoleLogger container, int indentLevel)
            {
                this.container = container;
                this.indentLevel = indentLevel;
                StringBuilder indent = new StringBuilder();
                for (int i = 0; i < indentLevel; i++) {
                    indent.Append('\t');
                }
                this.indent = indent.ToString();
            }

            public void Log(string format, object[] args)
            {
                var line = String.Format(format, args);
                Console.WriteLine(indent + line);
            }

            public Scope Begin()
            {
                return new Scope(container, indentLevel + 1);
            }

            protected override void Dispose(bool disposing)
            {
                if (disposing) {
                    var scope = container.scopes.Pop();
                    if (scope != this) {
                        throw new InvalidOperationException("Format scope removed out of order.");
                    }
                }
            }
        }

        private Stack<Scope> scopes = new Stack<Scope>();

        public IDisposable Begin(string title = "", params object[] args)
        {
            Log(title, args);
            Scope scope;
            if (scopes.Count == 0) {
                scope = new Scope(this, 1);
            } else {
                scope = ActiveScope.Begin();
            }
            scopes.Push(scope);
            return scope;
        }

        public void Log(string format, params object[] args)
        {
            if (scopes.Count > 0) {
                ActiveScope.Log(format, args);
            } else {
                Console.WriteLine(String.Format(format, args));
            }
        }

        private Scope ActiveScope
        {
            get
            {
                var top = scopes.Peek();
                if (top == null) throw new InvalidOperationException("No current scope");
                return top;
            }
        }
    }
}
