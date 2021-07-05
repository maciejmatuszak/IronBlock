﻿using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace IronBlock.Blocks.Procedures
{
    public class ProceduresIfReturn : ABlock
    {
        public override object EvaluateInternal(IContext context)
        {
            var condition = Values.Evaluate("CONDITION", context);
            if ((bool) condition)
            {
                return Values.Evaluate("VALUE", context);
            }

            return base.EvaluateInternal(context);
        }

        public override SyntaxNode Generate(IContext context)
        {
            var condition = Values.Generate("CONDITION", context) as ExpressionSyntax;
            if (condition == null)
            {
                throw new ApplicationException("Unknown expression for condition.");
            }

            var returnStatement = ReturnStatement();

            if (Values.Any(x => x.Name == "VALUE"))
            {
                var statement = Values.Generate("VALUE", context) as ExpressionSyntax;
                if (statement == null)
                {
                    throw new ApplicationException("Unknown expression for return statement.");
                }

                returnStatement = ReturnStatement(statement);
            }

            var ifStatement = IfStatement(condition, returnStatement);
            return Statement(ifStatement, base.Generate(context), context);
        }
    }
}