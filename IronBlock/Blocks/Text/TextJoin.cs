using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IronBlock.Utils;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace IronBlock.Blocks.Text
{
    public class TextJoin : ABlock
    {
        public override object EvaluateInternal(IContext context)
        {
            var items = int.Parse(Mutations.GetValue("items"));

            var sb = new StringBuilder();
            for (var i = 0; i < items; i++)
            {
                if (!Values.Any(x => x.Name == $"ADD{i}"))
                {
                    continue;
                }

                sb.Append(Values.Evaluate($"ADD{i}", context));
            }

            return sb.ToString();
        }

        public override SyntaxNode Generate(IContext context)
        {
            var items = int.Parse(Mutations.GetValue("items"));

            var arguments = new List<ExpressionSyntax>();

            for (var i = 0; i < items; i++)
            {
                if (!Values.Any(x => x.Name == $"ADD{i}"))
                {
                    continue;
                }

                var addExpression = Values.Generate($"ADD{i}", context) as ExpressionSyntax;
                if (addExpression == null)
                {
                    throw new ApplicationException($"Unknown expression for ADD{i}.");
                }

                arguments.Add(addExpression);
            }

            if (!arguments.Any())
            {
                return base.Generate(context);
            }

            return
                SyntaxGenerator.MethodInvokeExpression(
                    PredefinedType(
                        Token(SyntaxKind.StringKeyword)
                    ),
                    nameof(string.Concat),
                    arguments
                );
        }
    }
}