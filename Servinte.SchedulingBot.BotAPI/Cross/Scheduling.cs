using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Contoso.SchedulingBot.BotAPI
{
    [Serializable]
    public class Scheduling
    {
        public Patient Patient { get; set; }
        public string Speciality { get; set; }
        public DateTime DateTime { get; set; }
    }
}