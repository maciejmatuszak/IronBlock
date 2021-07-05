using System.Collections.Generic;
using System.Threading;

namespace IronBlock
{
    public class ProcedureContext : Context
    {
        public ProcedureContext(IContext parentContext = null,
            CancellationToken interruptToken = default)
            : base(parentContext, interruptToken)
        {
            Parameters = new Dictionary<string, object>();
        }

        public IDictionary<string, object> Parameters { get; set; }
    }
}