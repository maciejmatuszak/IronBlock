using System.Collections.Generic;
using System.Linq;
using IronBlock.Blocks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IronBlock.Tests
{
    internal class CustomPrintABlock : ABlock
    {
        public List<string> Text { get; set; } = new List<string>();

        public override object EvaluateInternal(Context context)
        {
            Text.Add((Values.FirstOrDefault(x => x.Name == "VALUE")?.Evaluate(context) ?? "").ToString());
            return base.EvaluateInternal(context);
        }
    }

    [TestClass]
    public class CustomBlockTests
    {
        [TestMethod]
        public void Test_Custom_Block()
        {
            const string xml = @"
<xml>
  <block type=""text_print"">
    <value name=""VALUE"">
      <shadow type=""text"">
        <field name=""TEXT"">abc</field>
      </shadow>
    </value>
  </block>
</xml>
";

            var printBlock = new CustomPrintABlock();
            var output = new Parser()
                .AddStandardBlocks()
                .AddBlock("text_print", printBlock)
                .Parse(xml)
                .Evaluate();

            Assert.AreEqual("abc", string.Join("", printBlock.Text));
        }
    }
}