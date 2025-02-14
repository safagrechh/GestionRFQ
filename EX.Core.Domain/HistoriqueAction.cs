using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EX.Core.Domain
{
    public class HistoriqueAction
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public string CibleAction { get; set; }
        public string ReferenceCible { get; set; }
        public string DetailsAction { get; set; }
        public DateTime DateAction { get; set; }

        //les relations


        [ForeignKey("UserId")] 
        public virtual User User { get; set; }
        public int UserId { get; set; }
    }
}
