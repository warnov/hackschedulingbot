using Microsoft.Bot.Connector;

namespace Contoso.SchedulingBot.BotAPI

{
    //Constants class. K is for Konstants.
    internal static class K
    {
        //internal
        public const int INT_DIALOG_TRIES = 3;
        public const int INT_LEVENSTEIN_MAX_DISTANCE = 60;
        public const string INT_V = "0.991";
        //end conversation reasons
        //everything ok. Appointment scheduled
        public const string INT_HAPPY_END = "INT_HAPPY_END";
        //not available dates for the appointment in the system
        public const string INT_NOT_SCHEDULE_AVAILABLE = "NOT_SCHEDULE_AVAILABLE";
        //the available dates are not good for the user
        public const string INT_NOT_SCHEDULE_SUITABLE = "NOT_SCHEDULE_SUITABLE";
        //special undergound commands
        public const string INT_SPECIAL_COMMAND = "HI WARNOV. ENTER YOUR SPECIAL COMMAND";
        public const string INT_SPECIAL_COMMAND_FINISHED = "COMMAND FINISHED";


        //general
        public static readonly string[] GRAL_YES_NO = new string[] { "Sí", "No" };
        public static readonly string[][] GRAL_YES_NO_OPTIONS = new string[][] {
            new[] {
                "yes", "Sí", "si", "correcto", "ok", "bien", "está bien" },
            new[] {
                "no", "No", "NO", "negativo", "nada" } };
        public const string GRAL_RESTART = "reiniciar";
        public const string GRAL_K_RESTART = "*****RESTART";
        public const string GRAL_CANCEL = "Cancelar";
        public const string GRAL_UNSUCESSFUL_BYE = "Lamento no haberte podido colaborar en esta ocasión. " +
            "Pero quedamos muy pendientes para cuando lo quieras intentar de nuevo. Muchas gracias!";
        public const string GRAL_THANKS = "Muchas gracias";
        public static readonly string[] GRAL_POSITIVE_THANKS_OPTS = new string[] {
            GRAL_THANKS,
            "Excelente",
            "OK",
            "Bien",
            "Muy bien",
            "Perfecto" };

        //errors
        public const string GRAL_INVALID_OPTION = "Opción Inválida";
        public const string GRAL_DATE_NOT_RECOGNIZED = "No he entendido la fecha ingresada. Por favor ingresala en formato DD/MM/AAAA por ejemplo 13/03/1980";
        public const string GRAL_GENERAL_ERROR = "Lamento no poder entenderte. Reiniciaremos el proceso de agendamiento de tu cita";
        public const string GRAL_UNINTELIGIBLE_TRY_AGAIN = "No pude entender tu respuesta. Intenta de nuevo por favor";
        public const string GRAL_TOO_MANY_TRIES = "Tras varios intentos no he podido entenderte. Iniciemos todo el proceso de nuevo por favor!";

        //root
        public const string ROOT_GREETING = "Hola. Soy el agente automatizado de asignación de citas médicas de Contoso.";
        public static readonly string ROOT_RESTART_INSTRUCTIONS = $"-Recuerda que en cualquier momento puedes reiniciar el proceso de agendamiento de tu cita escribiendo en la ventana de chat: REINICIAR. {V}";
        public static readonly string ROOT_WELCOME = $"{ROOT_GREETING}\n\n\u200C  \n\n{ROOT_RESTART_INSTRUCTIONS}";

        //patientData    
        public const string PAT_DATA_NAME = "Cuál es tu nombre?";
        public const string PAT_DATA_ID_TYPE = "Por favor indícame tu tipo de documento de identidad:";
        public const string PAT_DATA_ID_CODE = "Indícame por favor tu número de";
        public static readonly string[] PAT_DATA_BIRTHDATE_OPTS = new string[] {
            "Para validar la información indicame por favor tu fecha de nacimiento:",
            "Indícame por favor tu fecha de nacimiento",
            "Fecha de Nacimiento:" };
        public const string PAT_DATA_BIRTHDATE_ERROR = "La fecha de nacimiento ingresada no es correcta.";


        //speciality
        public const string SPEC_SPECIALITY = "De qué especialidad necesitas tu cita?";
        public const string SPEC_CLARIFY = "Quisiste decir {0}?";

        //scheduling
        public static readonly string[] SCH_ASK_DATE_OPTS = new string[] { "Para qué día deseas tu cita?", "Qué fecha desearías para tu cita?", "Indicame por favor la fecha de la cita." };
        public static readonly string[] SCH_BAD_SCHEDULING_DATE_OPTS = new string[] {
            "Lo siento. Solo puedo consultar agendamientos a partir de mañana.",
            "Es necesario que me indiques una fecha por favor a partir de mañana" };
        public static readonly string[] SCH_QUERYING_SCHEDULE_OPTS = new string[]
        { "Por favor espera mientras confirmo disponibilidad de {0} para el {1}",
            "Confirmando disponibilidad; por favor espera...",
            "Buscando disponibilidad para el {1}" };
        public const string SCH_NOT_AVAILABILITY = "Lo siento. Por ahora no he podido encontrar disponibilidades. Intenta más tarde por favor!";
        public const string SCH_SAME_DAY_AVAILABILITY = "Muy bien. Tenemos disponibilidad para el día que necesitas.";
        public const string SCH_OTHER_AVAILABILITY = "No tenemos la disponibilidad exacta que deseas, pero tenemos otras alternativas.";
        public const string SCH_OTHER_AVAILABILITY_PROMPT = "Si alguna de éstas opciones te sirve, escógela por favor. De lo contrario escoge Cancelar e intenta agendar en otra fecha";
        public static readonly string[] SCH_ASK_ANOTHER_DATE_OPS = new string[] { "Deseas intentar con otra fecha?", "Quieres indicarme otra fecha para tu cita?" };
        public static readonly string SCH_TIME = "Éstas son las horas que tenemos disponibles para el {0}.";

        public static object V { get; private set; }
    }
}