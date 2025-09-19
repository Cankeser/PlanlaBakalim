using PlanlaBakalim.Core.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanlaBakalim.Core.Entities
{
    public class BusinessAdress:Entity
    {
        [Display(Name = "İşletme")]
        public int BusinessId { get; set; }
        public Business Business { get; set; }
        

        [Display(Name = "İlçe")]
        public int DistrictId { get; set; }
        public District District { get; set; }

        [Display(Name = "Adres")]
        public string StreetAddress { get; set; } = null!;
    }
}
