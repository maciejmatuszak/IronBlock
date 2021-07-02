using System;
using System.Collections.Generic;
using System.Linq;
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
            RootContext = parentContext == null ? this : parentContext.RootContext;
            InterruptToken = interruptToken;

            _variables = new Dictionary<string, object>();
            Functions = new Dictionary<string, object>();
            Statements = new List<StatementSyntax>();
        }

        public virtual void Interrupt()
        {
            RootContext._interruptTokenSource.Cancel();
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

        #region VariableAccess

        /// <summary>
        /// Sets the variable in this context. Note this may "hide" variable with the same name in Parent(s) context
        /// </summary>
        /// <param name="varName"></param>
        /// <param name="value"></param>
        public void SetLocalVariable(string varName, object value)
        {
            _variables[varName] = value;
        }

        public void SetLocalVariable<T>(string varName, T value)
        {
            _variables[varName] = value;
        }

        /// <summary>
        /// Variable setter. If the variable exists in this or any Parent context then it will be updated.
        /// Otherwise it is set in this context 
        /// </summary>
        /// <param name="varName"></param>
        /// <param name="value"></param>
        public void SetVariable(string varName, object value)
        {
            var ctx = GetVariableContext(varName);
            // if the variable exists in any context
            if (ctx == null)
            {
                _variables[varName] = value;
            }
            else
            {
                ctx._variables[varName] = value;
            }
        }

        /// <summary>
        /// Variable Getter, returns variable value or default value if variable does not exists
        /// </summary>
        /// <param name="varName"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public object GetVariable(string varName, object defaultValue)
        {
            var ctx = GetVariableContext(varName);
            // if the variable exists in any context
            if (ctx == null)
            {
                return defaultValue;
            }

            return ctx._variables[varName];
        }

        public T GetVariable<T>(string varName, object defaultValue)
        {
            return (T) GetVariable(varName, defaultValue);
        }

        /// <summary>
        /// Variable Getter, returns variable value or throws ArgumentException if variable does not exists
        /// </summary>
        /// <param name="varName"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public object GetVariable(string varName)
        {
            var ctx = GetVariableContext(varName);
            // lets see if the variable exists in any context
            if (ctx == null)
            {
                throw new ArgumentException("Variable does not exists", nameof(varName));
            }

            return ctx._variables[varName];
        }

        public T GetVariable<T>(string varName)
        {
            return (T) GetVariable(varName);
        }


        /// <summary>
        /// Check if variable exists in context hierarchy. 
        /// </summary>
        /// <param name="varName"></param>
        /// <returns></returns>
        public bool DoesVariableExists(string varName)
        {
            return GetVariableContext(varName) != null;
        }

        public ICollection<string> GetVariableNames()
        {
            return _variables.Keys;
        }

        /// <summary>
        /// Gets the context where the variable is set
        /// </summary>
        /// <param name="varName"></param>
        /// <returns></returns>
        private Context GetVariableContext(string varName)
        {
            if (_variables.ContainsKey(varName))
            {
                return this;
            }

            return Parent?.GetVariableContext(varName);
        }

        #endregion

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

        public readonly Context RootContext;
        public event BeforeAfterBlockDelegate BeforeEvent;
        public event BeforeAfterBlockDelegate AfterEvent;
        public event OnErrorDelegate OnError;

        private readonly IDictionary<string, object> _variables;
        public IDictionary<string, object> Functions { get; set; }

        public EscapeMode EscapeMode { get; set; }

        public List<StatementSyntax> Statements { get; }

        public Context Parent { get; }
    }
}