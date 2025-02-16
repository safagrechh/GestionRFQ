using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
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

        [ForeignKey("ValidateurId")] 
        public virtual User Validateur { get; set; }
        public int ValidateurId { get; set; }


        public int? RFQId { get; set; }
        public virtual RFQ RFQ { get; set; }
        public int? VersionRFQId { get; set; }
        public virtual VersionRFQ VersionRFQ { get; set; }
    }
}
