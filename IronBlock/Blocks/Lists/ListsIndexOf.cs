using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace IronBlock.Blocks.Lists
{
    public class ListsIndexOf : IBlock
    {
        public override object Evaluate(Context context)
        {
            var direction = Fields.Get("END");
            var value = Values.Evaluate("VALUE", context) as IEnumerable<object>;
            var find = Values.Evaluate("FIND", context);

            switch (direction)
            {
                case "FIRST":
                    return value.ToList().IndexOf(find) + 1;

                case "LAST":
                    return value.ToList().LastIndexOf(find) + 1;

                default:
                    throw new NotSupportedException("$Unknown end: {direction}");
            }
        }

        public override SyntaxNode Generate(Context context)
        {
            throw new NotImplementedException();
        }
    }
}