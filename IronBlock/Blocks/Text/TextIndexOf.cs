﻿using System;
using IronBlock.Utils;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace IronBlock.Blocks.Text
{
    public class TextIndexOf : ABlock
    {
        public override object EvaluateInternal(IContext context)
        {
            var mode = Fields.Get("END");

            var text = (Values.Evaluate("VALUE", context) ?? "").ToString();
            var term = (Values.Evaluate("FIND", context) ?? "").ToString();

            switch (mode)
            {
                case "FIRST":
                    return (double) text.IndexOf(term) + 1;
                case "LAST":
                    return (double) text.LastIndexOf(term) + 1;
                default:
                    throw new ApplicationException("unknown mode");
            }
        }

        public override SyntaxNode Generate(IContext context)
        {
            var textExpression = Values.Generate("VALUE", context) as ExpressionSyntax;
            if (textExpression == null)
            {
                throw new ApplicationException("Unknown expression for value.");
            }

            var findExpression = Values.Generate("FIND", context) as ExpressionSyntax;
            if (findExpression == null)
            {
                throw new ApplicationException("Unknown expression for find.");
            }

            var mode = Fields.Get("END");
            switch (mode)
            {
                case "FIRST":
                    return SyntaxGenerator.MethodInvokeExpression(textExpression, nameof(string.IndexOf),
                        findExpression);
                case "LAST":
                    return SyntaxGenerator.MethodInvokeExpression(textExpression, nameof(string.LastIndexOf),
                        findExpression);
                default:
                    throw new NotSupportedException("unknown mode");
            }
        }
    }
}