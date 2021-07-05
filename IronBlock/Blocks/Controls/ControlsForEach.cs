using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace IronBlock.Blocks.Controls
{
    public class ControlsForEach : ABlock
    {
        public override object EvaluateInternal(IContext context)
        {
            var variableName = Fields.Get("VAR");
            var list = Values.Evaluate("LIST", context) as IEnumerable<object>;

            var statement = Statements.Where(x => x.Name == "DO").FirstOrDefault();

            if (null == statement)
            {
                return base.EvaluateInternal(context);
            }

            var forContext = context.CreateChildContext();

            foreach (var item in list)
            {
                forContext.SetLocalVariable(variableName, item);

                statement.Evaluate(forContext);
            }

            return base.EvaluateInternal(context);
        }

        public override SyntaxNode Generate(IContext context)
        {
            var variableName = Fields.Get("VAR").CreateValidName();
            var listExpression = Values.Generate("LIST", context) as ExpressionSyntax;
            if (listExpression == null)
            {
                throw new ApplicationException("Unknown expression for list.");
            }

            var statement = Statements.Where(x => x.Name == "DO").FirstOrDefault();

            if (null == statement)
            {
                return base.Generate(context);
            }

            var forEachContext = context.CreateChildContext();
            if (statement?.Block != null)
            {
                var statementSyntax = statement.Block.GenerateStatement(forEachContext);
                if (statementSyntax != null)
                {
                    forEachContext.Statements.Add(statementSyntax);
                }
            }

            var forEachStatement =
                ForEachStatement(
                    IdentifierName("var"),
                    Identifier(variableName),
                    listExpression,
                    Block(
                        forEachContext.Statements
                    )
                );

            return Statement(forEachStatement, base.Generate(context), context);
        }
    }
}