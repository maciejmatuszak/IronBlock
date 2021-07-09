// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the MIT license.  See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace IronBlock
{
    public interface IContext
    {
        /// <summary>
        /// Create default child context
        /// </summary>
        /// <returns></returns>
        IContext CreateChildContext();
        
        IContext RootContext { get; }
        IContext Parent { get; }

        /// <summary>
        /// method intended to be invoked before evaluation of each block
        /// </summary>
        /// <param name="block"></param>
        void InvokeBeforeEvent(IBlock block);
        
        /// <summary>
        /// method intended to be invoked after evaluation of each block
        /// </summary>
        /// <param name="block"></param>
        void InvokeAfterEvent(IBlock block);

        /// <summary>
        /// method intended to be called when there are errors during block evaluation
        /// </summary>
        /// <param name="sourceBlock"></param>
        /// <param name="errorType"></param>
        /// <param name="errorArg"></param>
        void BlockEvaluationError(IBlock sourceBlock, string errorType, object errorArg);

        #region Variable Access

        /// <summary>
        /// Sets the variable in this context. Note this may "hide" variable with the same name in Parent(s) context
        /// </summary>
        /// <param name="varName"></param>
        /// <param name="value"></param>
        void SetLocalVariable(string varName, object value);

        /// <summary>
        /// returns this context variable or throws ArgumentException exception if it does not exists  
        /// </summary>
        /// <param name="varName"></param>
        /// <returns></returns>
        object GetLocalVariable(string varName);

        /// <summary>
        /// Variable setter. If the variable exists in this or any Parent context then it will be updated.
        /// Otherwise it is set in this context 
        /// </summary>
        /// <param name="varName"></param>
        /// <param name="value"></param>
        void SetVariable(string varName, object value);

        /// <summary>
        /// Variable Getter, returns variable value or default value if variable does not exists in context chain
        /// </summary>
        /// <param name="varName"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        object GetVariable(string varName, object defaultValue);

        /// <summary>
        /// Generic variable Getter, returns variable value or default value if variable does not exists in context chain
        /// if exists variable is cast to T
        /// </summary>
        /// <param name="varName"></param>
        /// <param name="defaultValue"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T GetVariable<T>(string varName, object defaultValue);

        /// <summary>
        /// Variable Getter, returns variable value or throws ArgumentException if variable does not exists
        /// </summary>
        /// <param name="varName"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        object GetVariable(string varName);

        /// <summary>
        /// Variable Getter, returns variable value or throws ArgumentException if variable does not exists
        /// if exists variable is cast to T
        /// </summary>
        /// <param name="varName"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T GetVariable<T>(string varName);

        /// <summary>
        /// Check if variable exists in context hierarchy. 
        /// </summary>
        /// <param name="varName"></param>
        /// <returns></returns>
        bool DoesVariableExists(string varName);

        /// <summary>
        /// Returns collection of local variables in this context
        /// </summary>
        /// <returns></returns>
        ICollection<string> GetLocalVariableNames();
        
        /// <summary>
        /// Check the context chain from this to Root. It returns first context that holds variable with specified name 
        /// </summary>
        /// <param name="varName"></param>
        /// <returns></returns>
        IContext GetVariableContext(string varName);

        /// <summary>
        /// Overrides variable storage dictionary. Any modification to the context variables will affect the dictionary as well  
        /// </summary>
        /// <param name="variables"></param>
        void OverrideVariables(IDictionary<string, object> variables);

        #endregion

        #region Function Access

        /// <summary>
        /// stores function definition in this context
        /// note that this may hide function definition from Parent(s) context chain
        /// </summary>
        /// <param name="funcName"></param>
        /// <param name="value"></param>
        void SetLocalFunction(string funcName, Statement value);
        
        /// <summary>
        /// gets function from this context or throws MissingMethodException 
        /// </summary>
        /// <param name="funcName"></param>
        /// <returns></returns>
        Statement GetLocalFunction(string funcName);
        
        /// <summary>
        /// gets function from context chain or throws MissingMethodException 
        /// </summary>
        /// <param name="funcName"></param>
        /// <returns></returns>
        Statement GetFunction(string funcName);

        /// <summary>
        /// check if function exists in context chain
        /// </summary>
        /// <param name="funcName"></param>
        /// <returns></returns>
        bool DoesFunctionExists(string funcName);
        
        /// <summary>
        /// Gets list of functions defined in this context
        /// </summary>
        /// <returns></returns>
        ICollection<string> GetLocalFunctionNames();
        
        /// <summary>
        /// gets first context in chain that defines the function with name specified
        /// </summary>
        /// <param name="funcName"></param>
        /// <returns></returns>
        IContext GetFunctionContext(string funcName);

        #endregion

        
        
        List<StatementSyntax> Statements { get; }
        
        /// <summary>
        /// Escape mode used to break/continue from loops  
        /// </summary>
        EscapeMode EscapeMode { get; set; }

        /// <summary>
        /// InterruptToken used to break evaluation.
        /// If interrupted the current Evaluate function will throw EvaluateInterruptedException 
        /// </summary>
        CancellationToken InterruptToken { get; set; }
        
        /// <summary>
        /// Method to trigger the interruption
        /// </summary>
        void Interrupt();
    }
}