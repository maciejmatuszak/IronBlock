using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace IronBlock
{
    public delegate void BeforeAfterBlockDelegate(object sender, IBlock block);

    public delegate void OnErrorDelegate(IBlock senderBlock, string errorType, object errorAr);

    public class Context : IContext
    {
        public Context() : this(null, default)
        {
        }

        public Context(CancellationToken interruptToken) : this(null, interruptToken)
        {
        }

        public Context(IContext parentContext) : this(parentContext, default)
        {
        }

        public Context(IContext parentContext, CancellationToken interruptToken)
        {
            InterruptToken = interruptToken;
            Parent = parentContext;
            RootContext = parentContext == null ? this : parentContext.RootContext;
            _variables = new Dictionary<string, object>();
            _functions = new Dictionary<string, object>();
            Statements = new List<StatementSyntax>();
        }

        public IContext CreateChildContext()
        {
            return new Context(parentContext: this);
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

        public object GetLocalVariable(string varName)
        {
            if (!_variables.ContainsKey(varName))
            {
                throw new ArgumentException("Variable does not exists in this context", nameof(varName));
            }

            return _variables[varName];
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
                ctx.SetLocalVariable(varName, value);
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

            return ctx.GetLocalVariable(varName);
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
                throw new ArgumentException("Variable does not exists", varName);
            }

            return ctx.GetLocalVariable(varName);
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

        public ICollection<string> GetLocalVariableNames()
        {
            return _variables.Keys.ToList();
        }

        /// <summary>
        /// Gets the context where the variable is set
        /// </summary>
        /// <param name="varName"></param>
        /// <returns></returns>
        public IContext GetVariableContext(string varName)
        {
            if (_variables.ContainsKey(varName))
            {
                return this;
            }

            return Parent?.GetVariableContext(varName);
        }

        public void OverrideVariables(IDictionary<string, object> variables)
        {
            _variables = variables;
        }

        #endregion

        #region Functions

        public void SetLocalFunction(string funcName, object value)
        {
            _functions[funcName] = value;
        }

        public void SetLocalFunction<T>(string funcName, T value)
        {
            _functions[funcName] = value;
        }

        public object GetLocalFunction(string funcName)
        {
            if (!_functions.ContainsKey(funcName))
            {
                throw new ArgumentException("Variable does not exists in this context", funcName);
            }

            return _functions[funcName];
        }

        public object GetFunction(string funcName)
        {
            var ctx = GetFunctionContext(funcName);
            // lets see if the variable exists in any context
            if (ctx == null)
            {
                throw new MissingMethodException(GetType().FullName, funcName);
            }

            return ctx.GetLocalFunction(funcName);
        }

        public T GetFunction<T>(string funcName)
        {
            return (T) GetFunction(funcName);
        }

        public bool DoesFunctionExists(string funcName)
        {
            return GetFunctionContext(funcName) != null;
        }

        public ICollection<string> GetLocalFunctionNames()
        {
            return _functions.Keys.ToList();
        }

        public IContext GetFunctionContext(string funcName)
        {
            if (_functions.ContainsKey(funcName))
            {
                return this;
            }

            return Parent?.GetFunctionContext(funcName);
        }

        private IDictionary<string, object> _functions { get; set; }

        #endregion


        private CancellationToken _interruptToken = default;
        private CancellationTokenSource _interruptTokenSource = null;

        public virtual void Interrupt()
        {
            if (IsRoot)
            {
                _interruptTokenSource?.Cancel();
            }
            else
            {
                RootContext.Interrupt();
            }
        }

        public CancellationToken InterruptToken
        {
            get
            {
                if (IsRoot)
                {
                    return _interruptToken;
                }

                return RootContext.InterruptToken;
            }

            set
            {
                if (IsRoot)
                {
                    if (_interruptTokenSource != null)
                    {
                        throw new Exception("Can not change Interrupt Token");
                    }

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

        public IContext RootContext { get; }
        public event BeforeAfterBlockDelegate BeforeEvent;
        public event BeforeAfterBlockDelegate AfterEvent;
        public event OnErrorDelegate OnError;

        private IDictionary<string, object> _variables;

        public EscapeMode EscapeMode { get; set; }

        public List<StatementSyntax> Statements { get; }

        public IContext Parent { get; }
    }
}