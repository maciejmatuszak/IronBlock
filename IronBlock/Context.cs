using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace IronBlock
{
    public delegate void BeforeAfterBlockDelegate(object sender, IBlock block);

    public class Context
    {
        public Context(CancellationToken interruptToken = default, Context parentContext = null)
        {
            if (parentContext != null && interruptToken != null)
            {
                throw new ArgumentException(
                    "Either parentContext or interruptToken can be used;interruptToken is ony valid on root context ");
            }

            Parent = parentContext;
            var tokens = new List<CancellationToken>();

            if (interruptToken != default)
            {
                tokens.Add(interruptToken);
            }

            if (parentContext != null && parentContext.InterruptToken != default)
            {
                tokens.Add(parentContext.InterruptToken);
            }

            _interruptTokenSource = CancellationTokenSource.CreateLinkedTokenSource(tokens.ToArray());

            InterruptToken = _interruptTokenSource.Token;

            Variables = new Dictionary<string, object>();
            Functions = new Dictionary<string, object>();
            Statements = new List<StatementSyntax>();
        }

        public void Interrupt()
        {
            _interruptTokenSource.Cancel();
        }

        public Context RootContext
        {
            get
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
        }

        public virtual void InvokeBeforeEvent(IBlock block)
        {
            BeforeEvent?.Invoke(this, block);
            if (Parent != null)
            {
                Parent.InvokeBeforeEvent(block);
            }
        }

        public virtual void InvokeAfterEvent(IBlock block)
        {
            AfterEvent?.Invoke(this, block);
            Parent?.InvokeAfterEvent(block);
        }

        public virtual void HandleBlockError(IBlock sourceBlock, string errorType, object errorArg)
        {
            throw new BlockEvaluationException(sourceBlock, errorType, errorArg);
        }

        public CancellationToken InterruptToken { get; }
        private CancellationTokenSource _interruptTokenSource;
        public event BeforeAfterBlockDelegate BeforeEvent;
        public event BeforeAfterBlockDelegate AfterEvent;

        public IDictionary<string, object> Variables { get; set; }

        public IDictionary<string, object> Functions { get; set; }

        public EscapeMode EscapeMode { get; set; }

        public List<StatementSyntax> Statements { get; }

        public Context Parent { get; }
    }
}