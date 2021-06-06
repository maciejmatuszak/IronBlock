using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace IronBlock
{
    public class Context
    {
        public Context()
        {
            Variables = new Dictionary<string, object>();
            Functions = new Dictionary<string, object>();

            Statements = new List<StatementSyntax>();
        }

        public IDictionary<string, object> Variables { get; set; }

        public IDictionary<string, object> Functions { get; set; }

        public EscapeMode EscapeMode { get; set; }

        public List<StatementSyntax> Statements { get; }

        public Context Parent { get; set; }
    }
}