using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EX.Core.Domain
{
    public class Rapport
    {
        public int Id { get; set; }
        public string CheminFichier { get; set; }
        public DateTime DateCreation { get; set; }

        public virtual User user { get; set; }

    }
}
