using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EX.Core.Domain
{   
    public enum RoleU
    { validateur , IngenieurRFQ , Admin , Lecteur  }
    public class User
    {
        public int Id { get; set; }
        public string  NomUser { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public RoleU Role { get; set; }
        //les relations 

        public virtual IList<Commentaire> Commentaires { get; set; }
        public virtual IList<HistoriqueAction> HistoriqueActions { get; set; }
        public virtual IList<Rapport> Rapports { get; set; }

        public virtual IList<RFQ> RFQs { get; set; }


    }
}
