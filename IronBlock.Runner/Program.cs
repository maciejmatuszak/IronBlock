﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using IronBlock.Blocks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.CSharp.RuntimeBinder;

namespace IronBlock.Runner
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            try
            {
                if (args.Length < 1)
                {
                    Console.WriteLine(
                        @"Specify an XML file as the first argument

Specify any of the following as a second argument
  -e  (evaluate)
  -g  (generate)
  -co (compile)
  -ex (execute) (default)
");
                    Environment.ExitCode = 1;
                    return;
                }

                var filename = args.First();
                if (!File.Exists(filename))
                {
                    Console.WriteLine($"ERROR: File ({filename}) does not exist");
                    Environment.ExitCode = 1;
                    return;
                }

                var xml = File.ReadAllText(filename);

                var workspace =
                    new Parser()
                        .AddStandardBlocks()
                        .Parse(xml);

                var mode = args.Skip(1).FirstOrDefault();
                if (mode?.Equals("-g") ?? false)
                {
                    var syntaxTree = workspace.Generate();
                    var code = syntaxTree.NormalizeWhitespace().ToFullString();
                    Console.WriteLine(code);
                }
                else if (mode?.Equals("-co") ?? false)
                {
                    var syntaxTree = workspace.Generate();
                    var code = syntaxTree.NormalizeWhitespace().ToFullString();
                    var script = GenerateScript(code);

                    Console.WriteLine("Compiling...");

                    var diagnostics = Compile(script);
                    Console.WriteLine("Compile result:");

                    if (!diagnostics.Any())
                    {
                        Console.WriteLine("OK");
                    }
                    else
                    {
                        foreach (var diagnostic in diagnostics)
                        {
                            Console.WriteLine(diagnostic.GetMessage());
                        }
                    }
                }
                else if (mode?.Equals("-e") ?? false)
                {
                    var ctx = new RunnerContext(RunMode.Timed, 200.0);
                    ctx.BeforeEvent += OnCtxOnBeforeEvent;
                    ctx.AfterEvent += OnCtxOnAfterEvent;

                    workspace.Evaluate(ctx);


                    ctx.BeforeEvent -= OnCtxOnBeforeEvent;
                    ctx.AfterEvent -= OnCtxOnAfterEvent;
                    ctx.Dispose();
                }
                else // -ex
                {
                    // execute
                    var syntaxTree = workspace.Generate();
                    var code = syntaxTree.NormalizeWhitespace().ToFullString();
                    var script = GenerateScript(code);

                    ExecuteAsync(script).Wait();
                }

                //Console.ReadKey();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR: {ex}");
                Environment.ExitCode = 1;
            }
        }

        private static void OnCtxOnBeforeEvent(object sender, IBlock block)
        {
            Console.WriteLine($"Evaluating Block: {block}...");
        }

        private static void OnCtxOnAfterEvent(object sender, IBlock block)
        {
            Console.WriteLine($"Evaluating Block: {block}...DONE");
        }

        public static IEnumerable<Diagnostic> Compile(Script<object> script)
        {
            if (script == null)
            {
                return Enumerable.Empty<Diagnostic>();
            }

            try
            {
                return script.Compile();
            }
            catch (CompilationErrorException compilationErrorException)
            {
                return compilationErrorException.Diagnostics;
            }
        }

        public static Script<object> GenerateScript(string code)
        {
            var dynamicRuntimeReference = MetadataReference.CreateFromFile(typeof(DynamicAttribute).Assembly.Location);
            var runtimeBinderReference = MetadataReference.CreateFromFile(typeof(CSharpArgumentInfo).Assembly.Location);

            var scriptOptions =
                ScriptOptions.Default
                    .WithImports("System", "System.Linq", "System.Math")
                    .AddReferences(dynamicRuntimeReference, runtimeBinderReference);

            return CSharpScript.Create<object>(code, scriptOptions);
        }

        public static async Task<object> ExecuteAsync(Script<object> script)
        {
            var scriptState = await script.RunAsync();
            return scriptState.ReturnValue;
        }
    }
}