using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace IronBlock.Blocks.Logic
{
    public class LogicNegate : ABlock
    {
        public override object EvaluateInternal(Context context)
        {
            return !(bool) (Values.Evaluate("BOOL", context) ?? false);
        }

        public override SyntaxNode Generate(Context context)
        {
            var boolExpression = Values.Generate("BOOL", context) as ExpressionSyntax;
            if (boolExpression == null)
            {
                throw new ApplicationException("Unknown expression for negate.");
            }

            return PrefixUnaryExpression(SyntaxKind.LogicalNotExpression, boolExpression);
        }
    }
}