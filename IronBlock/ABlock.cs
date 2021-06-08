using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace IronBlock
{
    public abstract class ABlock : IBlock
    {
        public ABlock()
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
        public ABlock Next { get; set; }
        public IList<Mutation> Mutations { get; set; }
        public IList<Comment> Comments { get; set; }

        public virtual void BeforeEvaluate(Context context)
        {
            context.InvokeBeforeEvent(this);
        }

        public virtual void AfterEvaluate(Context context)
        {
            context.InvokeAfterEvent(this);
        }

        public virtual object EvaluateInternal(Context context)
        {
            if (null != Next && context.EscapeMode == EscapeMode.None)
            {
                return Next.Evaluate(context);
            }

            return null;
        }

        public virtual object Evaluate(Context context)
        {
            BeforeEvaluate(context);
            object result = EvaluateInternal(context);
            AfterEvaluate(context);
            return result;
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

        public SyntaxNode Statement(SyntaxNode syntaxNode, SyntaxNode nextSyntaxNode, Context context)
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

        public override string ToString()
        {
            var t = Type ?? GetType().Name;
            return $"{t}(\"{Id}\")";
        }
    }
}