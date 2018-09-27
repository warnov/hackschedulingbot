using Microsoft.Bot.Connector;
using Microsoft.Recognizers.Text;
using Microsoft.Recognizers.Text.DateTime;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Contoso.SchedulingBot.BotAPI
{
    //Utilities class. U is for Utilities.
    public static class U
    {                   
        /// <summary>
        /// Takes a text and extras a list of the possible dates it contains
        /// </summary>
        /// <param name="text">The text to be analyzed</param>
        /// <returns>The possible dates the text contains</returns>
        public static List<string> ExtractPossibleDatesFromText(string text, out bool hasTime)
        {
            hasTime = false;
            var result = DateTimeRecognizer.RecognizeDateTime(text, Culture.Spanish).FirstOrDefault();
            if (result != null)
            {
                var results = ((List<Dictionary<string, string>>)result.Resolution["values"]);
                var ret = new List<string>();
                foreach (var item in results)
                {
                    var strDate = (from pair in item
                                   where pair.Key == "value"
                                   select pair.Value).FirstOrDefault();
                    var timex = (from pair in item
                                   where pair.Key == "timex"
                                   select pair.Value).FirstOrDefault();
                    hasTime = timex.ToLower().Contains("t");
                    if(!String.IsNullOrEmpty(strDate))
                        ret.Add(strDate);
                }
                return ret;
            }
            else return null;
        }

        internal static CommandType CommandAnalysis(IMessageActivity message)
        {
            return message.Text.ToLower() == K.GRAL_RESTART.ToLower() ?
                CommandType.RESTART :
                CommandType.NOT_COMMAND;
        }
    }
}
