using System;
using System.Collections.Generic;
using System.Linq;
using IronBlock.Blocks.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace IronBlock
{
    public interface IFragment
    {
        // probably need a method like this here:
        object Evaluate(Context context);
        SyntaxNode Generate(Context context);
    }

    public class Workspace : IFragment
    {
        public Workspace()
        {
            Blocks = new List<IBlock>();
        }

        public IList<IBlock> Blocks { get; set; }

        public virtual object Evaluate(Context context)

        {
            // TODO: variables
            object returnValue = null;

            // first process procedure def blocks
            var processedProcedureDefBlocks = new List<IBlock>();
            foreach (var block in Blocks)
                if (block is ProceduresDef)
                {
                    block.Evaluate(context);
                    processedProcedureDefBlocks.Add(block);
                }

            foreach (var block in Blocks)
                if (!processedProcedureDefBlocks.Contains(block))
                    returnValue = block.Evaluate(context);

            return returnValue;
        }

        public virtual SyntaxNode Generate(Context context)
        {
            foreach (var block in Blocks)
            {
                var syntaxNode = block.Generate(context);
                if (syntaxNode == null)
                    continue;

                var statement = syntaxNode as StatementSyntax;
                if (statement == null) statement = ExpressionStatement(syntaxNode as ExpressionSyntax);

                var comments = string.Join("\n", block.Comments.Select(x => x.Value));
                if (!string.IsNullOrWhiteSpace(comments))
                    statement = statement.WithLeadingTrivia(Comment($"/* {comments} */"));

                context.Statements.Add(statement);
            }

            foreach (var function in context.Functions.Reverse())
            {
                var methodDeclaration = function.Value as LocalFunctionStatementSyntax;
                if (methodDeclaration == null)
                    continue;

                context.Statements.Insert(0, methodDeclaration);
            }

            foreach (var variable in context.Variables.Reverse())
            {
                var variableDeclaration = GenerateVariableDeclaration(variable.Key);
                context.Statements.Insert(0, variableDeclaration);
            }

            var blockSyntax = Block(context.Statements);
            return blockSyntax;
        }

        private LocalDeclarationStatementSyntax GenerateVariableDeclaration(string variableName)
        {
            return LocalDeclarationStatement(
                VariableDeclaration(
                        IdentifierName("dynamic")
                    )
                    .WithVariables(
                        SingletonSeparatedList(
                            VariableDeclarator(
                                Identifier(variableName)
                            )
                        )
                    )
            );
        }
    }

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
            if (null != Next && context.EscapeMode == EscapeMode.None) return Next.Evaluate(context);
            return null;
        }

        public virtual SyntaxNode Generate(Context context)
        {
            if (null != Next && context.EscapeMode == EscapeMode.None)
            {
                var node = Next.Generate(context);
                var commentText = string.Join("\n", Next.Comments.Select(x => x.Value));
                if (string.IsNullOrWhiteSpace(commentText)) return node;
                return node.WithLeadingTrivia(Comment($"/* {commentText} */"));
            }

            return null;
        }

        protected SyntaxNode Statement(SyntaxNode syntaxNode, SyntaxNode nextSyntaxNode, Context context)
        {
            if (nextSyntaxNode == null)
                return syntaxNode;

            StatementSyntax statementSyntax = null;

            if (syntaxNode is ExpressionSyntax expressionSyntax)
                statementSyntax = ExpressionStatement(expressionSyntax);
            else if (syntaxNode is StatementSyntax statement) statementSyntax = statement;

            if (statementSyntax == null)
                throw new ApplicationException("Unknown statement.");

            context.Statements.Insert(0, statementSyntax);
            return nextSyntaxNode;
        }
    }

    public class Statement : IFragment
    {
        public string Name { get; set; }
        public IBlock Block { get; set; }

        public object Evaluate(Context context)
        {
            if (null == Block) return null;
            return Block.Evaluate(context);
        }

        public SyntaxNode Generate(Context context)
        {
            if (null == Block) return null;
            return Block.Generate(context);
        }
    }

    public class Value : IFragment
    {
        public string Name { get; set; }
        public IBlock Block { get; set; }

        public object Evaluate(Context context)
        {
            if (null == Block) return null;
            return Block.Evaluate(context);
        }

        public SyntaxNode Generate(Context context)
        {
            if (null == Block) return null;
            return Block.Generate(context);
        }
    }

    public class Field
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }


    public enum EscapeMode
    {
        None,
        Break,
        Continue
    }


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

    public class ProcedureContext : Context
    {
        public ProcedureContext()
        {
            Parameters = new Dictionary<string, object>();
        }

        public IDictionary<string, object> Parameters { get; set; }
    }

    public class Mutation
    {
        public Mutation(string domain, string name, string value)
        {
            Domain = domain;
            Name = name;
            Value = value;
        }

        public string Domain { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
    }


    public class Comment
    {
        public Comment(string value)
        {
            Value = value;
        }

        public string Value { get; set; }
    }
}