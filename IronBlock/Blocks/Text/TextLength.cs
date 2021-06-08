using System;
using IronBlock.Utils;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace IronBlock.Blocks.Text
{
    public class TextLength : ABlock
    {
        public override object EvaluateInternal(Context context)
        {
            var text = (Values.Evaluate("VALUE", context) ?? "").ToString();

            return (double) text.Length;
        }

        public override SyntaxNode Generate(Context context)
        {
            var textExpression = Values.Generate("VALUE", context) as ExpressionSyntax;
            if (textExpression == null)
            {
                throw new ApplicationException("Unknown expression for text.");
            }

            return SyntaxGenerator.PropertyAccessExpression(textExpression, nameof(string.Length));
        }
    }
}