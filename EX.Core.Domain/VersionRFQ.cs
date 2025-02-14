using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EX.Core.Domain
{
    public enum StatutV
    {
        Brouillon, Finalise, Valide, Rejete
    }
    public class VersionRFQ
    {
        public int CodeV { get; set; }
        public string QuoteName { get; set; }
        public int NumRefQuoted { get; set; }
        public DateTime SOPDate { get; set; }
        public int MaxV { get; set; }
        public int EstV { get; set; }

        public DateTime KODate { get; set; }
        public DateTime CustomerDataDate { get; set; }
        public DateTime MDDate { get; set; }
        public DateTime MRDate { get; set; }
        public DateTime TDDate { get; set; }
        public DateTime TRDate { get; set; }
        public DateTime LDDate { get; set; }
        public DateTime LRDate { get; set; }
        public DateTime CDDate { get; set; }
        public DateTime ApprovalDate { get; set; }
        public DateTime DateCreation { get; set; }
        public StatutV Statut { get; set; }

        //les relations
        public virtual Worker MaterialLeader { get; set; }
        public virtual Worker TestLeader { get; set; }
        public virtual MarketSegment MarketSegment { get; set; }
        public User VALeader { get; set; }
        public RFQ RFQ { get; set; }
        public virtual IList<Commentaire> Commentaires { get; set; }

    }
}
