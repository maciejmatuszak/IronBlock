using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace IronBlock.Blocks.Math
{
    public class MathModulo : IBlock
    {
        public override object Evaluate(Context context)
        {
            var dividend = (double) Values.Evaluate("DIVIDEND", context);
            var divisor = (double) Values.Evaluate("DIVISOR", context);

            return dividend % divisor;
        }

        public override SyntaxNode Generate(Context context)
        {
            var dividendExpression = Values.Generate("DIVIDEND", context) as ExpressionSyntax;
            if (dividendExpression == null) throw new ApplicationException("Unknown expression for dividend.");

            var divisorExpression = Values.Generate("DIVISOR", context) as ExpressionSyntax;
            if (divisorExpression == null) throw new ApplicationException("Unknown expression for divisor.");

            return BinaryExpression(
                SyntaxKind.ModuloExpression,
                dividendExpression,
                divisorExpression
            );
        }
    }
}