using System;
using IronBlock.Utils;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace IronBlock.Blocks.Text
{
    public class TextPrint : ABlock
    {
        public override object EvaluateInternal(IContext context)
        {
            var text = Values.Evaluate("TEXT", context);

            Console.WriteLine(text);

            return base.EvaluateInternal(context);
        }

        public override SyntaxNode Generate(IContext context)
        {
            var syntaxNode = Values.Generate("TEXT", context);
            var expression = syntaxNode as ExpressionSyntax;
            if (expression == null)
            {
                throw new ApplicationException("Unknown expression for text.");
            }

            var invocationExpression =
                SyntaxGenerator.MethodInvokeExpression(IdentifierName(nameof(Console)), nameof(Console.WriteLine),
                    expression);

            return Statement(invocationExpression, base.Generate(context), context);
        }
    }
}