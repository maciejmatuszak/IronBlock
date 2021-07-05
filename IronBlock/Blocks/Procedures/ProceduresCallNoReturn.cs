using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace IronBlock.Blocks.Procedures
{
    public class ProceduresCallNoReturn : ABlock
    {
        public override object EvaluateInternal(IContext context)
        {
            // todo: add guard for missing name

            var name = Mutations.GetValue("name");

            var statement = context.GetFunction<IFragment>(name);

            var funcContext = context.CreateChildContext();
            
            var counter = 0;
            foreach (var mutation in Mutations.Where(x => x.Domain == "arg" && x.Name == "name"))
            {
                var value = Values.Evaluate($"ARG{counter}", funcContext);
                funcContext.SetLocalVariable(mutation.Value, value);
                counter++;
            }

            statement.Evaluate(funcContext);

            return base.EvaluateInternal(context);
        }

        public override SyntaxNode Generate(IContext context)
        {
            var methodName = Mutations.GetValue("name").CreateValidName();

            var arguments = new List<ArgumentSyntax>();

            var counter = 0;
            foreach (var mutation in Mutations.Where(x => x.Domain == "arg" && x.Name == "name"))
            {
                var argumentExpression = Values.Generate($"ARG{counter}", context) as ExpressionSyntax;
                if (argumentExpression == null)
                {
                    throw new ApplicationException($"Unknown argument expression for ARG{counter}.");
                }

                arguments.Add(Argument(argumentExpression));
                counter++;
            }

            var methodInvocation =
                InvocationExpression(
                    IdentifierName(methodName)
                );


            if (arguments.Any())
            {
                var syntaxList = SeparatedList(arguments);

                methodInvocation =
                    methodInvocation
                        .WithArgumentList(
                            ArgumentList(
                                syntaxList
                            )
                        );
            }

            return Statement(methodInvocation, base.Generate(context), context);
        }
    }
}