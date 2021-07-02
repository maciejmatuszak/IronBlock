using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace IronBlock
{
    public delegate void BeforeAfterBlockDelegate(object sender, IBlock block);

    public delegate void OnErrorDelegate(IBlock senderBlock, string errorType, object errorAr);

    public class Context
    {
        public Context(CancellationToken interruptToken = default, Context parentContext = null)
        {
            if (parentContext != null && interruptToken != default)
            {
                throw new ArgumentException(
                    "Either parentContext or interruptToken can be used;interruptToken is ony valid on root context ");
            }

            Parent = parentContext;
            InterruptToken = interruptToken;

            Variables = new Dictionary<string, object>();
            Functions = new Dictionary<string, object>();
            Statements = new List<StatementSyntax>();
        }

        public virtual void Interrupt()
        {
            RootContext._interruptTokenSource.Cancel();
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
            OnError?.Invoke(sourceBlock, errorType, errorArg);
        }

        private CancellationTokenSource _interruptTokenSource;

        private CancellationToken _interruptToken;

        public CancellationToken InterruptToken
        {
            get => RootContext._interruptToken;
            private set
            {
                if (IsRoot)
                {
                    _interruptTokenSource = CancellationTokenSource.CreateLinkedTokenSource(value);
                    _interruptToken = _interruptTokenSource.Token;
                }
                else
                {
                    RootContext.InterruptToken = value;
                }
            }
        }

        public bool IsRoot => Parent == null;

        public event BeforeAfterBlockDelegate BeforeEvent;
        public event BeforeAfterBlockDelegate AfterEvent;
        public event OnErrorDelegate OnError;

        public IDictionary<string, object> Variables { get; set; }

        public IDictionary<string, object> Functions { get; set; }

        public EscapeMode EscapeMode { get; set; }

        public List<StatementSyntax> Statements { get; }

        public Context Parent { get; }
    }
}