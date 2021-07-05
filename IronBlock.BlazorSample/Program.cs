﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IronBlock.Blocks;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.JSInterop;

namespace IronBlock.BlazorSample
{
    internal class CustomPrintABlock : ABlock
    {
        public List<string> Text { get; set; } = new List<string>();

        public override object EvaluateInternal(IContext context)
        {
            Text.Add((Values.FirstOrDefault(x => x.Name == "TEXT")?.Evaluate(context) ?? "").ToString());
            return base.EvaluateInternal(context);
        }
    }

    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            await builder.Build().RunAsync();
        }

        [JSInvokable]
        public static string Evaluate(string xml)
        {
            var printBlock = new CustomPrintABlock();

            new Parser()
                .AddStandardBlocks()
                .AddBlock("text_print", printBlock)
                .Parse(xml)
                .Evaluate();

            return string.Join('\n', printBlock.Text);
        }
    }
}