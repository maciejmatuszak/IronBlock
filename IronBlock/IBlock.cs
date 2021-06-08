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
        ABlock Next { get; set; }
        IList<Mutation> Mutations { get; set; }
        IList<Comment> Comments { get; set; }
        void BeforeEvaluate(Context context);
        void AfterEvaluate(Context context);
        object EvaluateInternal(Context context);
        SyntaxNode Statement(SyntaxNode syntaxNode, SyntaxNode nextSyntaxNode, Context context);
    }
}