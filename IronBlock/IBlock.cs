using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace IronBlock
{
    public abstract class IBlock : IFragment
    {
        public IBlock()
        {
            Fields = new List<Field>();
            Values = new List<Value>();
            Statements = new List<Statement>();
            Mutations = new List<Mutation>();
            Comments = new List<Comment>();
        }

        public string Id { get; set; }
        public IList<Field> Fields { get; set; }
        public IList<Value> Values { get; set; }
        public IList<Statement> Statements { get; set; }
        public string Type { get; set; }
        public bool Inline { get; set; }
        public IBlock Next { get; set; }
        public IList<Mutation> Mutations { get; set; }
        public IList<Comment> Comments { get; set; }

        public virtual object Evaluate(Context context)
        {
            if (null != Next && context.EscapeMode == EscapeMode.None)
            {
                return Next.Evaluate(context);
            }

            return null;
        }

        public virtual SyntaxNode Generate(Context context)
        {
            if (null != Next && context.EscapeMode == EscapeMode.None)
            {
                var node = Next.Generate(context);
                var commentText = string.Join("\n", Next.Comments.Select(x => x.Value));
                if (string.IsNullOrWhiteSpace(commentText))
                {
                    return node;
                }

                return node.WithLeadingTrivia(SyntaxFactory.Comment($"/* {commentText} */"));
            }

            return null;
        }

        protected SyntaxNode Statement(SyntaxNode syntaxNode, SyntaxNode nextSyntaxNode, Context context)
        {
            if (nextSyntaxNode == null)
            {
                return syntaxNode;
            }

            StatementSyntax statementSyntax = null;

            if (syntaxNode is ExpressionSyntax expressionSyntax)
            {
                statementSyntax = SyntaxFactory.ExpressionStatement(expressionSyntax);
            }
            else if (syntaxNode is StatementSyntax statement)
            {
                statementSyntax = statement;
            }

            if (statementSyntax == null)
            {
                throw new ApplicationException("Unknown statement.");
            }

            context.Statements.Insert(0, statementSyntax);
            return nextSyntaxNode;
        }
    }
}