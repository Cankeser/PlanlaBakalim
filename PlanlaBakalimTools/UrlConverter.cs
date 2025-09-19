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
            
            //return text.ToLower().Trim().Replace("�", "g").Replace("�", "u").Replace("�", "s").Replace("�", "i").Replace("�", "o").Replace("�", "c").Replace(" ", "-");

            // T�rk�e karakterleri de�i�tir
            var sb = new StringBuilder(text.ToLower());
            sb.Replace("�", "g").Replace("�", "u").Replace("�", "s")
              .Replace("�", "i").Replace("�", "o").Replace("�", "c").Replace("&", "-ve-");

            // T�m �zel karakterleri kald�r, bo�luklar� "-" yap
            string result = Regex.Replace(sb.ToString(), @"[^a-z0-9\s-]", "");
            result = Regex.Replace(result, @"\s+", "-"); // birden fazla bo�luk varsa tek "-" yap
            result = result.Trim('-'); // ba�ta ve sonda "-" kalmas�n

            return result;
        }

    }
    
}
