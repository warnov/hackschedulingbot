using System;
using System.Text;


namespace Contoso.SchedulingBot.BotAPI

{
    [Serializable]
    public class Patient
    {
        public string Name { get; set; }
        public DateTime BirthDate { get; set; }
        public IdDocument IdDocument { get; set; }

        public Patient()
        {
            IdDocument = new IdDocument();
        }

        public string InformalName
        {
            get
            {
                var parts = Name.Split(' ');
                return parts.Length > 1 ? parts[0] : Name;
            }
        }

        public void SetFirstCapital()
        {
            var parts = Name.Split(' ');
            var ret = new StringBuilder();
            for (int i = 0; i < parts.Length; i++)
            {
                ret.Append($"{parts[i].CapitalizeFirstLetter()} ");
            }
            ret.Remove(ret.Length - 1, 1);
            Name = ret.ToString();
        }
    }
}