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
        public RoleW Role { get; set; }
    }
}
