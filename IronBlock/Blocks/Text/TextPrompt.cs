using System;
using IronBlock.Utils;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace IronBlock.Blocks.Text
{
    public class TextPrompt : ABlock
    {
        public override object EvaluateInternal(IContext context)
        {
            var inputType = Mutations.GetValue("type") ?? "TEXT";

            var text = (Values.Evaluate("TEXT", context) ?? "").ToString();

            if (!string.IsNullOrWhiteSpace(text))
            {
                Console.Write($"{text}: ");
            }

            var value = Console.ReadLine();

            if (inputType == "NUMBER")
            {
                return int.Parse(value);
            }

            return value;
        }

        public override SyntaxNode Generate(IContext context)
        {
            var inputType = Mutations.GetValue("type") ?? "TEXT";

            var expression = Values.Generate("TEXT", context) as ExpressionSyntax;
            if (expression != null)
            {
                context.Statements.Add(
                    ExpressionStatement(
                        SyntaxGenerator.MethodInvokeExpression(
                            IdentifierName(nameof(Console)),
                            nameof(Console.WriteLine),
                            expression
                        )
                    )
                );
            }

            context.Statements.Add(
                LocalDeclarationStatement(
                    VariableDeclaration(
                            IdentifierName("var")
                        )
                        .WithVariables(
                            SingletonSeparatedList(
                                VariableDeclarator(
                                        Identifier("value")
                                    )
                                    .WithInitializer(
                                        EqualsValueClause(
                                            SyntaxGenerator.MethodInvokeExpression(
                                                IdentifierName(nameof(Console)),
                                                nameof(Console.ReadLine)
                                            )
                                        )
                                    )
                            )
                        )
                )
            );

            if (inputType == "NUMBER")
            {
                return
                    SyntaxGenerator.MethodInvokeExpression(
                        PredefinedType(
                            Token(SyntaxKind.IntKeyword)
                        ),
                        nameof(int.Parse),
                        IdentifierName("value")
                    );
            }

            return IdentifierName("value");
        }
    }
}