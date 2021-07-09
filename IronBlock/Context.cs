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
            _functions = new Dictionary<string, Statement>();
            Statements = new List<StatementSyntax>();
        }

        /// <summary>
        /// Create default child context
        /// </summary>
        /// <returns></returns>
        public virtual IContext CreateChildContext()
        {
            return new Context(parentContext: this);
        }

        /// <summary>
        /// Triggers BeforeEvent event in entire context chain
        /// </summary>
        /// <param name="block"></param>
        public virtual void InvokeBeforeEvent(IBlock block)
        {
            BeforeEvent?.Invoke(this, block);
            if (Parent != null)
            {
                Parent.InvokeBeforeEvent(block);
            }
        }

        /// <summary>
        /// Triggers AfterEvent event in entire context chain
        /// </summary>
        /// <param name="block"></param>
        public virtual void InvokeAfterEvent(IBlock block)
        {
            AfterEvent?.Invoke(this, block);
            Parent?.InvokeAfterEvent(block);
        }

        /// <summary>
        /// triggers OnError event in entire context chain
        /// </summary>
        /// <param name="sourceBlock"></param>
        /// <param name="errorType"></param>
        /// <param name="errorArg"></param>
        public virtual void BlockEvaluationError(IBlock sourceBlock, string errorType, object errorArg)
        {
            OnError?.Invoke(sourceBlock, errorType, errorArg);
            Parent?.BlockEvaluationError(sourceBlock, errorType, errorArg);
        }

        #region VariableAccess

        /// <summary>
        /// Sets the variable in this context. Note this may "hide" variable with the same name in Parent(s) context
        /// </summary>
        /// <param name="varName"></param>
        /// <param name="value"></param>
        public virtual void SetLocalVariable(string varName, object value)
        {
            _variables[varName] = value;
        }

        /// <summary>
        /// returns this context variable or throws ArgumentException exception if it does not exists  
        /// </summary>
        /// <param name="varName"></param>
        /// <returns></returns>
        public virtual object GetLocalVariable(string varName)
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
        public virtual void SetVariable(string varName, object value)
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
        public virtual object GetVariable(string varName, object defaultValue)
        {
            var ctx = GetVariableContext(varName);
            // if the variable exists in any context
            if (ctx == null)
            {
                return defaultValue;
            }

            return ctx.GetLocalVariable(varName);
        }

        /// <summary>
        /// Generic variable Getter, returns variable value or default value if variable does not exists in context chain
        /// if exists variable is cast to T
        /// </summary>
        /// <param name="varName"></param>
        /// <param name="defaultValue"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public virtual T GetVariable<T>(string varName, object defaultValue)
        {
            return (T) GetVariable(varName, defaultValue);
        }

        /// <summary>
        /// Variable Getter, returns variable value or throws ArgumentException if variable does not exists
        /// </summary>
        /// <param name="varName"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public virtual object GetVariable(string varName)
        {
            var ctx = GetVariableContext(varName);
            // lets see if the variable exists in any context
            if (ctx == null)
            {
                throw new ArgumentException("Variable does not exists", varName);
            }

            return ctx.GetLocalVariable(varName);
        }

        /// <summary>
        /// Variable Getter, returns variable value or throws ArgumentException if variable does not exists
        /// if exists variable is cast to T
        /// </summary>
        /// <param name="varName"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public virtual T GetVariable<T>(string varName)
        {
            return (T) GetVariable(varName);
        }


        /// <summary>
        /// Check if variable exists in context hierarchy. 
        /// </summary>
        /// <param name="varName"></param>
        /// <returns></returns>
        public virtual bool DoesVariableExists(string varName)
        {
            return GetVariableContext(varName) != null;
        }

        /// <summary>
        /// Returns collection of local variables in this context
        /// </summary>
        /// <returns></returns>
        public virtual ICollection<string> GetLocalVariableNames()
        {
            return _variables.Keys.ToList();
        }

        /// <summary>
        /// Check the context chain from this to Root. It returns first context that holds variable with specified name 
        /// </summary>
        /// <param name="varName"></param>
        /// <returns></returns>
        public virtual IContext GetVariableContext(string varName)
        {
            if (_variables.ContainsKey(varName))
            {
                return this;
            }

            return Parent?.GetVariableContext(varName);
        }

        /// <summary>
        /// Overrides variable storage dictionary. Any modification to the context variables will affect the dictionary as well  
        /// </summary>
        /// <param name="variables"></param>
        public virtual void OverrideVariables(IDictionary<string, object> variables)
        {
            _variables = variables;
        }

        #endregion

        #region Functions

        /// <summary>
        /// stores function definition in this context
        /// note that this may hide function definition from Parent(s) context chain
        /// </summary>
        /// <param name="funcName"></param>
        /// <param name="value"></param>
        public virtual void SetLocalFunction(string funcName, Statement value)
        {
            _functions[funcName] = value;
        }

        /// <summary>
        /// gets function from this context or throws MissingMethodException 
        /// </summary>
        /// <param name="funcName"></param>
        /// <returns></returns>
        public virtual Statement GetLocalFunction(string funcName)
        {
            if (!_functions.ContainsKey(funcName))
            {
                throw new MissingMethodException(GetType().FullName, funcName);
            }

            return _functions[funcName];
        }

        /// <summary>
        /// gets function from context chain or throws MissingMethodException 
        /// </summary>
        /// <param name="funcName"></param>
        /// <returns></returns>
        public virtual Statement GetFunction(string funcName)
        {
            var ctx = GetFunctionContext(funcName);
            // lets see if the variable exists in any context
            if (ctx == null)
            {
                throw new MissingMethodException(GetType().FullName, funcName);
            }

            return ctx.GetLocalFunction(funcName);
        }

        /// <summary>
        /// check if function exists in context chain
        /// </summary>
        /// <param name="funcName"></param>
        /// <returns></returns>
        public virtual bool DoesFunctionExists(string funcName)
        {
            return GetFunctionContext(funcName) != null;
        }

        /// <summary>
        /// Gets list of functions defined in this context
        /// </summary>
        /// <returns></returns>
        public virtual ICollection<string> GetLocalFunctionNames()
        {
            return _functions.Keys.ToList();
        }

        /// <summary>
        /// gets first context in chain that defines the function with name specified
        /// </summary>
        /// <param name="funcName"></param>
        /// <returns></returns>
        public virtual IContext GetFunctionContext(string funcName)
        {
            if (_functions.ContainsKey(funcName))
            {
                return this;
            }

            return Parent?.GetFunctionContext(funcName);
        }

        private IDictionary<string, Statement> _functions { get; set; }

        #endregion


        private CancellationToken _interruptToken;
        private CancellationTokenSource _interruptTokenSource;


        /// <summary>
        /// Method to trigger the interruption
        /// </summary>
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

        /// <summary>
        /// InterruptToken used to break evaluation.
        /// If interrupted the current Evaluate function will throw EvaluateInterruptedException
        /// token is always sets/gets from RootContext without regard to which context it is sets/gets on     
        /// </summary>
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