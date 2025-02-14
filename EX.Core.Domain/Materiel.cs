using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EX.Core.Domain
{
    public class Materiel
    {
        [Key]
        public int Code { get; set; }
        public string Type { get; set; }
        public float Cout { get; set; }

    }
}
