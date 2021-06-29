using System.Collections.Generic;
using System.Threading;

namespace IronBlock
{
    public class ProcedureContext : Context
    {
        public ProcedureContext(CancellationToken interruptToken = default(CancellationToken),
            Context parentContext = null)
            : base(interruptToken, parentContext)
        {
            Parameters = new Dictionary<string, object>();
        }

        public IDictionary<string, object> Parameters { get; set; }
    }
}