using System.Collections.Generic;
using System.Linq;

namespace IronBlock.Tests
{
    internal static class TestExtensions
    {
        internal static IList<string> GetDebugText()
        {
            return DebugPrint.Text;
        }

        internal static Parser AddDebugPrinter(this Parser parser)
        {
            DebugPrint.Text.Clear();

            parser.AddBlock<DebugPrint>("text_print");
            return parser;
        }

        internal class DebugPrint : ABlock
        {
            static DebugPrint()
            {
                Text = new List<string>();
            }

            public static List<string> Text { get; set; }

            public override object EvaluateInternal(IContext context)
            {
                Text.Add((Values.First(x => x.Name == "TEXT").Evaluate(context) ?? "").ToString());
                return base.EvaluateInternal(context);
            }
        }
    }
}