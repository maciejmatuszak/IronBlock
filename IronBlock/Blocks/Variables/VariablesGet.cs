﻿using Microsoft.CodeAnalysis;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace IronBlock.Blocks.Variables
{
    public class VariablesGet : ABlock
    {
        public override object EvaluateInternal(IContext context)
        {
            var variableName = Fields.Get("VAR");

            return context.GetVariable(variableName);
        }

        public override SyntaxNode Generate(IContext context)
        {
            var variableName = Fields.Get("VAR").CreateValidName();

            return IdentifierName(variableName);
        }
    }
}