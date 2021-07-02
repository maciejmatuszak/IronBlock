using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IronBlock.Blocks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IronBlock.Tests
{
    [TestClass]
    public class ContextTests
    {
        [TestMethod]
        [ExpectedException(typeof(EvaluateInterruptedException))]
        public async Task Test_Interrupt()
        {
            const string xml = @"
<xml>
  <block type=""text_length"">
    <value name=""VALUE"">
      <shadow type=""text"">
        <field name=""TEXT"">abc</field>
      </shadow>
    </value>
  </block>
</xml>
";

            var workspace = new Parser()
                .AddStandardBlocks()
                .Parse(xml);

            var runner = new RunnerContext(RunMode.Stepped);
            var finished = false;
            var task = Task<object>.Run(() =>
            {
                try
                {
                    Console.WriteLine("evaluating...");
                    workspace.Evaluate(runner);
                    Console.WriteLine("evaluating...DONE");
                }
                catch (EvaluateInterruptedException)
                {
                    Console.WriteLine("interrupted");
                }
                finally
                {
                    finished = true;
                }
            });
            // runner.Interrupt();
            while (! finished)
            {
                runner.Step();
                await Task.Delay(100);
            }
            task.GetAwaiter().GetResult();
            
        }
    }
}