using PlanlaBakalim.Core.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanlaBakalim.Core.Entities
{
    public class UserFavorites:Entity
    {
        public int UserId { get; set; }      
        public  User User { get; set; }
        public int BusinessId { get; set; }
        public  Business Business { get; set; }
    }
}
