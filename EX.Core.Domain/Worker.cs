using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EX.Core.Domain
{   public enum RoleW
    {
        Material , Test
    }
    public class Worker
    {
        public int Id { get; set; }

        public string Nom { get; set; }

        public RoleW Role { get; set; }

        public virtual IList<VersionRFQ> VMaterialLeader { get; set; }
        public virtual IList<VersionRFQ> vTestLeader { get; set; }

        public virtual IList<RFQ> AsMaterialLeader { get; set; }
        public virtual IList<RFQ> AsTestLeader { get; set; }
    }
}
