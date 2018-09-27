using System;

namespace Contoso.SchedulingBot.BotAPI
{
    public enum CommandType
    {
        RESTART,
        NOT_COMMAND
    }

    public enum IdType
    {
        CC,
        TI,
        TE,
        PP
    }

    [Serializable]
    public class IdDocument
    {
        public IdType IdTypeValue;
        public string IdCode;
    }
}