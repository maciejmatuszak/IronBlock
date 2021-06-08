using System;
using System.Collections.Generic;
using System.Linq;
using IronBlock.Utils;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace IronBlock.Blocks.Lists
{
    public class ListsIsEmpty : ABlock
    {
        public override object EvaluateInternal(Context context)
        {
            var value = Values.Evaluate("VALUE", context) as IEnumerable<object>;
            if (null == value)
            {
                return true;
            }

            return !value.Any();
        }

        public override SyntaxNode Generate(Context context)
        {
            var valueExpression = Values.Generate("VALUE", context) as ExpressionSyntax;
            if (valueExpression == null)
            {
                throw new ApplicationException("Unknown expression for value.");
            }

            return SyntaxGenerator.MethodInvokeExpression(valueExpression, nameof(Enumerable.Any));
        }
    }
}