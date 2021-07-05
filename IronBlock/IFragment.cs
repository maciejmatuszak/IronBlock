using Microsoft.CodeAnalysis;

namespace IronBlock
{
    public interface IFragment
    {
        // probably need a method like this here:
        object Evaluate(IContext context);
        SyntaxNode Generate(IContext context);
    }
}