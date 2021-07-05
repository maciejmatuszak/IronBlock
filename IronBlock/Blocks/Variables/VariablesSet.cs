using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace IronBlock.Blocks.Variables
{
    public class VariablesSet : ABlock
    {
        public override object EvaluateInternal(IContext context)
        {
            var value = Values.Evaluate("VALUE", context);
            var variableName = Fields.Get("VAR");

            context.SetVariable(variableName, value);

            return base.EvaluateInternal(context);
        }

        public override SyntaxNode Generate(IContext context)
        {

            var variableName = Fields.Get("VAR").CreateValidName();

            var valueExpression = Values.Generate("VALUE", context) as ExpressionSyntax;
            if (valueExpression == null)
            {
                throw new ApplicationException("Unknown expression for value.");
            }

            context.RootContext.SetLocalVariable(variableName, valueExpression);

            var assignment = AssignmentExpression(
                SyntaxKind.SimpleAssignmentExpression,
                IdentifierName(variableName),
                valueExpression
            );

            return Statement(assignment, base.Generate(context), context);
        }
    }
}