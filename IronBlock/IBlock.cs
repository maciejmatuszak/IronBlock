using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace IronBlock
{
    public interface IBlock : IFragment
    {
        string Id { get; set; }
        IList<Field> Fields { get; set; }
        IList<Value> Values { get; set; }
        IList<Statement> Statements { get; set; }
        string Type { get; set; }
        bool Inline { get; set; }
        IBlock Next { get; set; }
        IList<Mutation> Mutations { get; set; }
        IList<Comment> Comments { get; set; }
        void BeforeEvaluate(IContext context);
        void AfterEvaluate(IContext context);
        object EvaluateInternal(IContext context);
        SyntaxNode Statement(SyntaxNode syntaxNode, SyntaxNode nextSyntaxNode, IContext context);
    }
}