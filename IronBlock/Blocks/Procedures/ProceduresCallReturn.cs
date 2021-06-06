using System;
using System.Linq;

namespace IronBlock.Blocks.Procedures
{
    public class ProceduresCallReturn : ProceduresCallNoReturn
    {
        public override object Evaluate(Context context)
        {
            // todo: add guard for missing name

            var name = Mutations.GetValue("name");

            if (!context.Functions.ContainsKey(name))
            {
                throw new MissingMethodException($"Method '{name}' not defined");
            }

            var statement = (IFragment) context.Functions[name];

            var funcContext = new Context {Parent = context};
            funcContext.Functions = context.Functions;

            var counter = 0;
            foreach (var mutation in Mutations.Where(x => x.Domain == "arg" && x.Name == "name"))
            {
                var value = Values.Evaluate($"ARG{counter}", context);
                funcContext.Variables.Add(mutation.Value, value);
                counter++;
            }

            return statement.Evaluate(funcContext);
        }
    }
}