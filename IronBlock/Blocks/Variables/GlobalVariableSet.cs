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

            var rootContext = context.RootContext;

            if (rootContext.Variables.ContainsKey(variableName))
            {
                rootContext.Variables[variableName] = value;
            }
            else
            {
                rootContext.Variables.Add(variableName, value);
            }

            return base.EvaluateInternal(context);
        }

        public override SyntaxNode Generate(Context context)
        {
            return null;
        }
    }
}