using Microsoft.CodeAnalysis;

namespace IronBlock
{
    public interface IFragment
    {
        // probably need a method like this here:
        object Evaluate(Context context);
        SyntaxNode Generate(Context context);
    }
}