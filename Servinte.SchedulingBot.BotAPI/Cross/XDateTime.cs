using System;

namespace Contoso.SchedulingBot.BotAPI
{
    public static class XDateTime
    {
        public static string ToColombianDateTimeString(this DateTime dateTime)
        {
            var preRet = dateTime.ToColombianDateString();
            var time = dateTime.ToString("h:mmtt").Replace(". ", "").Replace(".", "");
            var connector = dateTime.Hour == 1 || dateTime.Hour == 13 ? "la" : "las";
            return $"{preRet} a {connector} {time}";
        }

        public static string ToColombianDateString(this DateTime dateTime)
        {
            var preRet = dateTime.ToString(@"dddd, d \de MMMM", new System.Globalization.CultureInfo("es-CO"));
            return preRet;
        }

        public static DateTime InColombia(this DateTime dateTime)
        {
            return dateTime.AddHours(-5);
        }
    }
}