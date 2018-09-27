using System;

namespace Contoso.SchedulingBot.BotAPI
{
    public static class XString
    {
        //john smith ==> John Smith
        public static string CapitalizeFirstLetter(this string str)
        {
            var lowerPart = str.ToLower();
            return lowerPart[0].ToString().ToUpper() +//Initial capitalized
                lowerPart.Substring(1);            
        }

        //[ok, muy bien, excelente, gracias] ==> muy bien
        public static string GetOne(this string[] components)
        {
            Random r = new Random();
            var idx=r.Next(components.Length);
            return components[idx];
        }
    }
}