using Microsoft.CodeAnalysis;

namespace IronBlock.Blocks.Variables
{
    // Fast-Solution
    public class GlobalVariablesSet : ABlock
    {
        public override object EvaluateInternal(Context context)
        {
            var value = Values.Evaluate("VALUE", context);
            var variableName = Fields.Get("VAR");

            context.RootContext.SetLocalVariable(variableName, value);

            return base.EvaluateInternal(context);
        }

        public override SyntaxNode Generate(Context context)
        {
            return null;
        }
    }
}