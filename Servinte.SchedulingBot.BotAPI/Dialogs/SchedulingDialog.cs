using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Contoso.SchedulingBot.BotAPI
{
    [Serializable]
    public class SchedulingDialog : IDialog<DateTime>
    {
        private Scheduling _scheduling;
        private DateTime[] _proposedDates;
        private Dictionary<string, DateTime> _availableDateOptions;

        public SchedulingDialog(ref Scheduling scheduling) => _scheduling = scheduling;

        public Task StartAsync(IDialogContext context)
        {
            //_context = context;
            AskSchedulingDate(context);
            return Task.CompletedTask;
        }


        //Asks the scheduling date to the user
        private void AskSchedulingDate(IDialogContext context)
        {
            PromptDialog.Text(
                context,
                OnSetSchedule,
                $"{K.GRAL_POSITIVE_THANKS_OPTS.GetOne()}" +
                $" {_scheduling.Patient.InformalName}. " +
                $"{K.SCH_ASK_DATE_OPTS.GetOne()}");
        }

        /// <summary>
        /// When the user writes his desired dateEvaluates the date input given by the user and acts in consequencve
        /// </summary>
        /// <param name="context"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        private async Task OnSetSchedule(IDialogContext context, IAwaitable<string> result)
        {
            var res = await result;

            if (ExtractScheduleDateTime(res, out DateTime preferredDate, out bool hasTime))
            {
                await context.PostAsync(String.Format(K.SCH_QUERYING_SCHEDULE_OPTS.GetOne(), _scheduling.Speciality, preferredDate.ToColombianDateString()));
                var typingMsg = context.MakeMessage();
                typingMsg.Type = ActivityTypes.Typing;
                typingMsg.Text = null;

                var dateQueryTask = Task.Run(() => AvailableSchedule(_scheduling.Speciality, preferredDate));
                await context.PostAsync(typingMsg);
                _proposedDates = await dateQueryTask;

                if (_proposedDates.Length <= 0)
                {
                    //No near availability: no check for time required. Conversation ends.

                    await context.PostAsync(K.SCH_NOT_AVAILABILITY);
                    AskForAnotherDate(context);
                }
                else
                {
                    //Same date found
                    if (_proposedDates[0].Date == preferredDate.Date)
                    {
                        await context.PostAsync(K.SCH_SAME_DAY_AVAILABILITY);
                        _scheduling.DateTime = preferredDate;
                        await ShowTimes(context);
                    }
                    else
                    {
                        await context.PostAsync(K.SCH_OTHER_AVAILABILITY);
                        SetAvailableOptionsDictionary();
                        PromptDialog.Choice(
                           context,
                           OnSetOptionalAvailability,
                           _availableDateOptions.Keys.ToList(),
                           K.SCH_OTHER_AVAILABILITY_PROMPT,
                           K.GRAL_INVALID_OPTION,
                           3
                       );
                    }
                }
            }
            else
            {
                await context.PostAsync(K.GRAL_DATE_NOT_RECOGNIZED);
                //Try again or end conversation?
                AskForAnotherDate(context);
            }
        }

        private Task OnSetOptionalAvailability(IDialogContext context, IAwaitable<KeyValuePair<string, DateTime>> result)
        {
            throw new NotImplementedException();
        }

        //Show available times to the user
        private async Task ShowTimes(IDialogContext context)
        {
            await context.PostAsync(String.Format(K.SCH_TIME, _scheduling.DateTime.ToColombianDateString()));
            List<string> timeStrings = GetTimeStrings(_scheduling.DateTime);
            timeStrings.Add(K.GRAL_CANCEL);

            PromptDialog.Choice(
                context,
                OnTimeSelected,
                timeStrings,
                K.SCH_OTHER_AVAILABILITY_PROMPT,
                K.GRAL_INVALID_OPTION,
                3
            );
        }

        //When the user selects a given time (or cancels the operation)
        private async Task OnTimeSelected(IDialogContext context, IAwaitable<string> result)
        {
            string timeSelected = await result;
            //User cancelled
            if (timeSelected.ToLower() == K.GRAL_CANCEL.ToLower())
            {
                AskForAnotherDate(context);
            }
            else
            {
                var justTimeDate = Convert.ToDateTime(timeSelected);
                _scheduling.DateTime = _scheduling.DateTime.AddHours(justTimeDate.Hour);
                _scheduling.DateTime = _scheduling.DateTime.AddMinutes(justTimeDate.Minute);
                context.Done(_scheduling.DateTime);
            }
        }

        /// <summary>
        /// Asks the user if he wants to write another date
        /// </summary>
        private void AskForAnotherDate(IDialogContext context)
        {
            PromptDialog.Confirm(
                   context,
                   OnAskForAnotherDateAnswer,
                   K.SCH_ASK_ANOTHER_DATE_OPS.GetOne(),
                   K.GRAL_UNINTELIGIBLE_TRY_AGAIN,
                   3,
                   PromptStyle.Auto,
                   K.GRAL_YES_NO,
                   K.GRAL_YES_NO_OPTIONS
                   );
        }

        //Process to be done after answering the try another date question
        private async Task OnAskForAnotherDateAnswer(IDialogContext context, IAwaitable<bool> result)
        {
            var giveAnotherDate = await result;
            //False, then exit
            if (!giveAnotherDate)
            {
                await context.PostAsync(K.GRAL_UNSUCESSFUL_BYE);
                context.EndConversation(K.INT_NOT_SCHEDULE_AVAILABLE);
            }
            else
            {
                //True, then ask again
                AskSchedulingDate(context);
            }
        }

        //When the user have choosen another date
        private async Task OnSetOptionalAvailability(IDialogContext context, IAwaitable<string> result)
        {
            var strChosenDate = await result;

            //Have the user cancelled?
            if (strChosenDate.ToLower() == K.GRAL_CANCEL.ToLower())
            {
                AskForAnotherDate(context);
            }
            else
            {
                _scheduling.DateTime = _availableDateOptions[strChosenDate];
                await ShowTimes(context);
            }
        }

        /// <summary>
        /// Generates a menu from a list of possible dates
        /// </summary>
        private void SetAvailableOptionsDictionary()
        {
            _availableDateOptions = new Dictionary<string, DateTime>();
            var uniqueProposedDates = _proposedDates.GroupBy(date => date.Date).Select(grp => grp.First()).ToList();
            foreach (var proposedDateTime in uniqueProposedDates)
            {
                _availableDateOptions.Add(proposedDateTime.ToColombianDateString(), proposedDateTime.Date);
            }
            _availableDateOptions.Add(K.GRAL_CANCEL, DateTime.MinValue);
        }



        /********************  Utilities */
        //Extracts dates from text        
        private static bool ExtractScheduleDateTime(string res, out DateTime scheduleDate, out bool timeGiven)
        {
            timeGiven = false;
            scheduleDate = DateTime.MinValue;
            var dateStrings = U.ExtractPossibleDatesFromText(res, out timeGiven);
            if (dateStrings.Count > 0)
            {
                //Look for the first date that is greater than today
                foreach (var sDate in dateStrings)
                {
                    scheduleDate = DateTime.Parse(sDate);
                    if (scheduleDate.Date > DateTime.Now.InColombia().Date) return true;
                }
                return false;
            }
            else return false;
        }



        /// <summary>
        /// Simulation of a communication with a scheduling service
        /// </summary>
        /// <param name="speciality"></param>
        /// <param name="preferredDateTime"></param>
        /// <returns></returns>
        internal static DateTime[] AvailableSchedule(string speciality, DateTime preferredDateTime)
        {
            Random rnd = new Random();
            var probExactDay = rnd.NextDouble() <= 0.2d;
            Thread.Sleep(8000);
            if (probExactDay)
            {
                return new DateTime[] { preferredDateTime.AddHours(8),
                preferredDateTime.AddHours(10),
                preferredDateTime.AddHours(15)};
            }
            else
            {
                return new DateTime[] {
                    preferredDateTime.AddHours(-29.5),
                    preferredDateTime.AddHours(26.5),
                    preferredDateTime.AddHours(50)
                };
            }
        }

        /// <summary>
        /// From a list of dates, returns a friendly list of strings for the user to choose
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        private List<string> GetTimeStrings(DateTime selectedDate)
        {
            List<string> ret = new List<string>();
            foreach (var dt in _proposedDates)
            {
                if (dt.Date == selectedDate.Date)
                    ret.Add(string.Format("{0:t}\n", dt));
            }
            return ret;
        }
    }
}