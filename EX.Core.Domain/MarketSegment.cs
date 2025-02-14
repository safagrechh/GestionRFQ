using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EX.Core.Domain
{
    public class MarketSegment
    {
        public int Id { get; set; }
        public string Nom { get; set; }

        public virtual IList<VersionRFQ> Versions{ get; set; }
        public virtual IList<RFQ> RFQs { get; set; }

      

    }
}
