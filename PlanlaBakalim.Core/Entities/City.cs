using PlanlaBakalim.Core.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanlaBakalim.Core.Entities
{
    public class City :Entity
    {
        public string Name { get; set; } = null!;

        public ICollection<District> Districts { get; set; } = new HashSet<District>();
    }
}
