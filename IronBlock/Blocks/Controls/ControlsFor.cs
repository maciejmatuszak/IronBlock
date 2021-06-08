using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace IronBlock.Blocks.Controls
{
    public class ControlsFor : ABlock
    {
        public override object EvaluateInternal(Context context)
        {
            var variableName = Fields.Get("VAR");

            var fromValue = (double) Values.Evaluate("FROM", context);
            var toValue = (double) Values.Evaluate("TO", context);
            var byValue = (double) Values.Evaluate("BY", context);

            var statement = Statements.FirstOrDefault();


            if (context.Variables.ContainsKey(variableName))
            {
                context.Variables[variableName] = fromValue;
            }
            else
            {
                context.Variables.Add(variableName, fromValue);
            }


            while ((double) context.Variables[variableName] <= toValue)
            {
                statement.Evaluate(context);
                context.Variables[variableName] = (double) context.Variables[variableName] + byValue;
            }

            return base.EvaluateInternal(context);
        }

        public override SyntaxNode Generate(Context context)
        {
            var variableName = Fields.Get("VAR").CreateValidName();

            var fromValueExpression = Values.Generate("FROM", context) as ExpressionSyntax;
            if (fromValueExpression == null)
            {
                throw new ApplicationException("Unknown expression for from value.");
            }

            var toValueExpression = Values.Generate("TO", context) as ExpressionSyntax;
            if (toValueExpression == null)
            {
                throw new ApplicationException("Unknown expression for to value.");
            }

            var byValueExpression = Values.Generate("BY", context) as ExpressionSyntax;
            if (byValueExpression == null)
            {
                throw new ApplicationException("Unknown expression for by value.");
            }

            var statement = Statements.FirstOrDefault();

            var rootContext = context.GetRootContext();
            if (!rootContext.Variables.ContainsKey(variableName))
            {
                rootContext.Variables[variableName] = null;
            }

            var forContext = new Context { Parent = context };
            if (statement?.Block != null)
            {
                var statementSyntax = statement.ABlock.GenerateStatement(forContext);
                if (statementSyntax != null)
                {
                    forContext.Statements.Add(statementSyntax);
                }
            }

            var forStatement =
                ForStatement(
                        Block(forContext.Statements)
                    )
                    .WithInitializers(
                        SingletonSeparatedList<ExpressionSyntax>(
                            AssignmentExpression(
                                SyntaxKind.SimpleAssignmentExpression,
                                IdentifierName(variableName),
                                fromValueExpression
                            )
                        )
                    )
                    .WithCondition(
                        BinaryExpression(
                            SyntaxKind.LessThanOrEqualExpression,
                            IdentifierName(variableName),
                            toValueExpression
                        )
                    )
                    .WithIncrementors(
                        SingletonSeparatedList<ExpressionSyntax>(
                            AssignmentExpression(
                                SyntaxKind.AddAssignmentExpression,
                                IdentifierName(variableName),
                                byValueExpression
                            )
                        )
                    );

            return Statement(forStatement, base.Generate(context), context);
        }
    }
}