using PlanlaBakalim.Core.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanlaBakalim.Core.Entities
{
    public class District : Entity
    {
        public string Name { get; set; } = null!;

        public int CityId { get; set; }
        public City City { get; set; } = null!;

        public ICollection<BusinessAdress> BusinessAdresses { get; set; } = new HashSet<BusinessAdress>();

    }
}