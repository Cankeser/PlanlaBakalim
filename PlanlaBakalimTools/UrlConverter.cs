using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PlanlaBakalim.Utilities
{
    public static class UrlConverter
    {
        public static string ConvertToUrl(string text)
        {

            if (string.IsNullOrWhiteSpace(text)) return string.Empty;
            
            //return text.ToLower().Trim().Replace("ð", "g").Replace("ü", "u").Replace("þ", "s").Replace("ý", "i").Replace("ö", "o").Replace("ç", "c").Replace(" ", "-");

            // Türkçe karakterleri deðiþtir
            var sb = new StringBuilder(text.ToLower());
            sb.Replace("ð", "g").Replace("ü", "u").Replace("þ", "s")
              .Replace("ý", "i").Replace("ö", "o").Replace("ç", "c").Replace("&", "-ve-");

            // Tüm özel karakterleri kaldýr, boþluklarý "-" yap
            string result = Regex.Replace(sb.ToString(), @"[^a-z0-9\s-]", "");
            result = Regex.Replace(result, @"\s+", "-"); // birden fazla boþluk varsa tek "-" yap
            result = result.Trim('-'); // baþta ve sonda "-" kalmasýn

            return result;
        }

    }
    
}
