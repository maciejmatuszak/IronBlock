using System;
using IronBlock.Utils;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace IronBlock.Blocks.Text
{
    public class TextTrim : ABlock
    {
        public override object EvaluateInternal(IContext context)
        {
            var mode = Fields.Get("MODE");

            var text = (Values.Evaluate("TEXT", context) ?? "").ToString();

            switch (mode)
            {
                case "BOTH":
                    return text.Trim();
                case "LEFT":
                    return text.TrimStart();
                case "RIGHT":
                    return text.TrimEnd();
                default:
                    throw new ApplicationException("unknown mode");
            }
        }

        public override SyntaxNode Generate(IContext context)
        {
            var textExpression = Values.Generate("TEXT", context) as ExpressionSyntax;
            if (textExpression == null)
            {
                throw new ApplicationException("Unknown expression for text.");
            }

            var mode = Fields.Get("MODE");

            switch (mode)
            {
                case "BOTH":
                    return SyntaxGenerator.MethodInvokeExpression(textExpression, nameof(string.Trim));
                case "LEFT":
                    return SyntaxGenerator.MethodInvokeExpression(textExpression, nameof(string.TrimStart));
                case "RIGHT":
                    return SyntaxGenerator.MethodInvokeExpression(textExpression, nameof(string.TrimEnd));
                default:
                    throw new ApplicationException("unknown mode");
            }
        }
    }
}