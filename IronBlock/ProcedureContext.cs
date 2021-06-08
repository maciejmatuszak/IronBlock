using System.Collections.Generic;

namespace IronBlock
{
    public class ProcedureContext : Context
    {
        public ProcedureContext()
        {
            Parameters = new Dictionary<string, object>();
        }

        public IDictionary<string, object> Parameters { get; set; }
    }
}