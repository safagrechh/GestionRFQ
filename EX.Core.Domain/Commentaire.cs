using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EX.Core.Domain
{
    public class Commentaire
    {
        public int Id { get; set; }
        public string Contenu { get; set; }
        public DateTime DateC { get; set; }

        public virtual User User { get; set; }
        public virtual RFQ RFQ { get; set; }
        public virtual VersionRFQ VersionRFQ { get; set; }
    }
}
