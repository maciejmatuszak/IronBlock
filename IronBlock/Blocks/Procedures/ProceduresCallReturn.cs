using System;
using System.Linq;

namespace IronBlock.Blocks.Procedures
{
    public class ProceduresCallReturn : ProceduresCallNoReturn
    {
        public override object EvaluateInternal(IContext context)
        {
            // todo: add guard for missing name

            var name = Mutations.GetValue("name");

            var statement = context.GetFunction(name);

            var funcContext = context.CreateChildContext();

            var counter = 0;
            foreach (var mutation in Mutations.Where(x => x.Domain == "arg" && x.Name == "name"))
            {
                var value = Values.Evaluate($"ARG{counter}", funcContext);
                funcContext.SetLocalVariable(mutation.Value, value);
                counter++;
            }

            return statement.Evaluate(funcContext);
        }
    }
}