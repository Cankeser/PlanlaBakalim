using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanlaBakalim.Core.Base
{
    public abstract class Entity
    {
        public int Id { get; set; }
        public DateTime CreatedDate { get; set; }=DateTime.Now;
    }
}
