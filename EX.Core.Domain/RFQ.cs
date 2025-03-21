﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EX.Core.Domain
{
    public enum Statut
    {
        Complete, InProgress, NotStarted
    }

    public class RFQ
    {
       
        public int Id { get; set; }
        public int? CQ { get; set; }

        public string? QuoteName { get; set; }

        public int? NumRefQuoted { get; set; }

        public DateTime? SOPDate { get; set; }

        public int? MaxV { get; set; }

        public int? EstV { get; set; }

        public DateTime? KODate { get; set; }

        public DateTime? CustomerDataDate { get; set; }

        public DateTime? MDDate { get; set; }

        public DateTime? MRDate { get; set; }

        public DateTime? TDDate { get; set; }

        public DateTime? TRDate { get; set; }

        public DateTime? LDDate { get; set; }

        public DateTime? LRDate { get; set; }

        public DateTime? CDDate { get; set; }

        public DateTime? ApprovalDate { get; set; }

        public DateTime DateCreation { get; set; }

        public Statut Statut { get; set; }

        public Boolean Valide { get; set; }

        public Boolean Rejete { get; set; }

        public Boolean Brouillon { get; set; }

        // les relations

        public virtual Worker? MaterialLeader { get; set; }
        public int? MaterialLeaderId { get; set; }

        public virtual Worker? TestLeader { get; set; }
        public int? TestLeaderId { get; set; }

        public virtual MarketSegment? MarketSegment { get; set; }
        public int? MarketSegmentId { get; set; }

        public virtual Client? Client { get; set; }
        public int? ClientId { get; set; }

        public virtual IList<Commentaire>? Commentaires { get; set; }
        public virtual IList<VersionRFQ>? Versions { get; set; }

        public virtual User? IngenieurRFQ { get; set; }
        public int? IngenieurRFQId { get; set; }

        public virtual User? VALeader { get; set; }
        public int? VALeaderId { get; set; }
    }
}
