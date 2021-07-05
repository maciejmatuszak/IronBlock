using System;
using IronBlock.Utils;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace IronBlock.Blocks.Text
{
    public class TextIsEmpty : ABlock
    {
        public override object EvaluateInternal(IContext context)
        {
            var text = (Values.Evaluate("VALUE", context) ?? "").ToString();

            return string.IsNullOrEmpty(text);
        }

        public override SyntaxNode Generate(IContext context)
        {
            var textExpression = Values.Generate("VALUE", context) as ExpressionSyntax;
            if (textExpression == null)
            {
                throw new ApplicationException("Unknown expression for text.");
            }

            return SyntaxGenerator.MethodInvokeExpression(PredefinedType(Token(SyntaxKind.StringKeyword)),
                nameof(string.IsNullOrEmpty), textExpression);
        }
    }
}