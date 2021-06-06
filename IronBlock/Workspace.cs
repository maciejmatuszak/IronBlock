using System.Collections.Generic;
using System.Linq;
using IronBlock.Blocks.Procedures;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace IronBlock
{
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
            {
                if (block is ProceduresDef)
                {
                    block.Evaluate(context);
                    processedProcedureDefBlocks.Add(block);
                }
            }

            foreach (var block in Blocks)
            {
                if (!processedProcedureDefBlocks.Contains(block))
                {
                    returnValue = block.Evaluate(context);
                }
            }

            return returnValue;
        }

        public virtual SyntaxNode Generate(Context context)
        {
            foreach (var block in Blocks)
            {
                var syntaxNode = block.Generate(context);
                if (syntaxNode == null)
                {
                    continue;
                }

                var statement = syntaxNode as StatementSyntax;
                if (statement == null)
                {
                    statement = SyntaxFactory.ExpressionStatement(syntaxNode as ExpressionSyntax);
                }

                var comments = string.Join("\n", block.Comments.Select(x => x.Value));
                if (!string.IsNullOrWhiteSpace(comments))
                {
                    statement = statement.WithLeadingTrivia(SyntaxFactory.Comment($"/* {comments} */"));
                }

                context.Statements.Add(statement);
            }

            foreach (var function in context.Functions.Reverse())
            {
                var methodDeclaration = function.Value as LocalFunctionStatementSyntax;
                if (methodDeclaration == null)
                {
                    continue;
                }

                context.Statements.Insert(0, methodDeclaration);
            }

            foreach (var variable in context.Variables.Reverse())
            {
                var variableDeclaration = GenerateVariableDeclaration(variable.Key);
                context.Statements.Insert(0, variableDeclaration);
            }

            var blockSyntax = SyntaxFactory.Block(context.Statements);
            return blockSyntax;
        }

        private LocalDeclarationStatementSyntax GenerateVariableDeclaration(string variableName)
        {
            return SyntaxFactory.LocalDeclarationStatement(
                SyntaxFactory.VariableDeclaration(
                        SyntaxFactory.IdentifierName("dynamic")
                    )
                    .WithVariables(
                        SyntaxFactory.SingletonSeparatedList(
                            SyntaxFactory.VariableDeclarator(
                                SyntaxFactory.Identifier(variableName)
                            )
                        )
                    )
            );
        }
    }
}