using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace IronBlock.Blocks.Lists
{
    public class ListsCreateWith : ABlock
    {
        public override object EvaluateInternal(IContext context)
        {
            var list = new List<object>();
            foreach (var value in Values)
            {
                list.Add(value.Evaluate(context));
            }

            return list;
        }

        public override SyntaxNode Generate(IContext context)
        {
            var expressions = new List<ExpressionSyntax>();

            foreach (var value in Values)
            {
                var itemExpression = value.Generate(context) as ExpressionSyntax;
                if (itemExpression == null)
                {
                    throw new ApplicationException("Unknown expression for item.");
                }

                expressions.Add(itemExpression);
            }

            return
                ObjectCreationExpression(
                        GenericName(
                                Identifier(nameof(List))
                            )
                            .WithTypeArgumentList(
                                TypeArgumentList(
                                    SingletonSeparatedList<TypeSyntax>(
                                        IdentifierName("dynamic")
                                    )
                                )
                            )
                    )
                    .WithInitializer(
                        InitializerExpression(
                            SyntaxKind.CollectionInitializerExpression,
                            SeparatedList(expressions)
                        )
                    );
        }
    }
}