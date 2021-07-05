using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace IronBlock.Blocks.Text
{
    public class TextAppend : ABlock
    {
        public override object EvaluateInternal(IContext context)
        {
          
            var variableName = Fields.Get("VAR");
            var textToAppend = (Values.Evaluate("TEXT", context) ?? "").ToString();

            var value = context.GetVariable(variableName, "").ToString();
            value += textToAppend;
            context.SetVariable(variableName, value);

            return base.EvaluateInternal(context);
        }

        public override SyntaxNode Generate(IContext context)
        {
            var variableName = Fields.Get("VAR").CreateValidName();

            var textExpression = Values.Generate("TEXT", context) as ExpressionSyntax;
            if (textExpression == null)
            {
                throw new ApplicationException("Unknown expression for text.");
            }

            context.RootContext.SetLocalVariable(variableName, textExpression);

            var assignment =
                AssignmentExpression(
                    SyntaxKind.AddAssignmentExpression,
                    IdentifierName(variableName),
                    textExpression
                );

            return Statement(assignment, base.Generate(context), context);
        }
    }
}