// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the MIT license.  See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace IronBlock
{
    public interface IContext
    {
        IContext CreateChildContext();
        IContext RootContext { get; }
        IContext Parent { get; }

        void InvokeBeforeEvent(IBlock block);
        void InvokeAfterEvent(IBlock block);

        void HandleBlockError(IBlock sourceBlock, string errorType, object errorArg);

        #region Variable Access

        /// <summary>
        /// Sets the variable in this context. Note this may "hide" variable with the same name in Parent(s) context
        /// </summary>
        /// <param name="varName"></param>
        /// <param name="value"></param>
        void SetLocalVariable(string varName, object value);

        object GetLocalVariable(string varName);

        /// <summary>
        /// Variable setter. If the variable exists in this or any Parent context then it will be updated.
        /// Otherwise it is set in this context 
        /// </summary>
        /// <param name="varName"></param>
        /// <param name="value"></param>
        void SetVariable(string varName, object value);

        /// <summary>
        /// Variable Getter, returns variable value or default value if variable does not exists
        /// </summary>
        /// <param name="varName"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        object GetVariable(string varName, object defaultValue);

        T GetVariable<T>(string varName, object defaultValue);

        /// <summary>
        /// Variable Getter, returns variable value or throws ArgumentException if variable does not exists
        /// </summary>
        /// <param name="varName"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        object GetVariable(string varName);

        T GetVariable<T>(string varName);

        /// <summary>
        /// Check if variable exists in context hierarchy. 
        /// </summary>
        /// <param name="varName"></param>
        /// <returns></returns>
        bool DoesVariableExists(string varName);

        ICollection<string> GetLocalVariableNames();
        IContext GetVariableContext(string varName);

        void OverrideVariables(IDictionary<string, object> variables);

        #endregion

        #region Function Access

        void SetLocalFunction(string funcName, object value);
        object GetLocalFunction(string funcName);
        object GetFunction(string funcName);
        T GetFunction<T>(string funcName);
        bool DoesFunctionExists(string funcName);
        ICollection<string> GetLocalFunctionNames();
        IContext GetFunctionContext(string funcName);

        #endregion

        List<StatementSyntax> Statements { get; }
        EscapeMode EscapeMode { get; set; }

        CancellationToken InterruptToken { get; set; }
        void Interrupt();
    }
}