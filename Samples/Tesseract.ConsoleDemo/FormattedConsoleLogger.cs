using System;
using System.Collections.Generic;
using System.Text;

namespace Tesseract.ConsoleDemo
{
	 public class FormattedConsoleLogger
    {
        const string Tab = "    ";
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
                    indent.Append(Tab);
                }
                this.indent = indent.ToString();
            }

            public void Log(string format, object[] args)
            {
                var message = String.Format(format, args);
                StringBuilder indentedMessage = new StringBuilder(message.Length + indent.Length * 10);
                int i=0;
                bool isNewLine = true;
                while(i < message.Length) {
                    if (message.Length > i && message[i] == '\r' && message[i + 1] == '\n') {
                        indentedMessage.AppendLine();
                        isNewLine = true;
                        i+=2;
                    } else if (message[i] == '\r' || message[i] == '\n') {
                        indentedMessage.AppendLine();
                        isNewLine = true;
                        i++;
                    } else {
                        if (isNewLine) {
                            indentedMessage.Append(indent);
                            isNewLine = false;
                        }
                        indentedMessage.Append(message[i]);
                        i++;
                    }
                }

                Console.WriteLine(indentedMessage.ToString());
                
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
