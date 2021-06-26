using System.Collections.Generic;
using System.Threading;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace IronBlock
{
    public delegate void BeforeAfterBlockDelegate(object sender, IBlock block);

    public class Context
    {
        public Context(CancellationToken interruptToken = default(CancellationToken))
        {
            InterruptToken = interruptToken;
            Variables = new Dictionary<string, object>();
            Functions = new Dictionary<string, object>();

            Statements = new List<StatementSyntax>();
        }

        public Context GetRootContext()
        {
            var parentContext = Parent;

            while (parentContext != null)
            {
                if (parentContext.Parent == null)
                {
                    return parentContext;
                }

                parentContext = parentContext.Parent;
            }

            return this;
        }

        public void InvokeBeforeEvent(IBlock block)
        {
            BeforeEvent?.Invoke(this, block);
            if (Parent != null)
            {
                Parent.InvokeBeforeEvent(block);
            }
        }

        public void InvokeAfterEvent(IBlock block)
        {
            AfterEvent?.Invoke(this, block);
            Parent?.InvokeAfterEvent(block);
        }

        public CancellationToken InterruptToken { get; }
        public event BeforeAfterBlockDelegate BeforeEvent;
        public event BeforeAfterBlockDelegate AfterEvent;

        public IDictionary<string, object> Variables { get; set; }

        public IDictionary<string, object> Functions { get; set; }

        public EscapeMode EscapeMode { get; set; }

        public List<StatementSyntax> Statements { get; }

        public Context Parent { get; set; }
    }
}