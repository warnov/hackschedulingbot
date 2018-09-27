using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Contoso.SchedulingBot.BotAPI
{
    [Serializable]
    public class SpecialityDialog : IDialog<string>
    {
        private string _speciality;
        private int tries = K.INT_DIALOG_TRIES;
        public Task StartAsync(IDialogContext context)
        {
            PromptDialog.Text(context, OnSetSpeciality, K.SPEC_SPECIALITY);
            return Task.CompletedTask;
        }

        private async Task OnSetSpeciality(IDialogContext context, IAwaitable<string> result)
        {
            var preSpeciality = (await result).ToLower();
            _speciality = GetSpeciality(preSpeciality);
            if (!String.IsNullOrEmpty(_speciality))
            {
                _speciality = _speciality.CapitalizeFirstLetter();
                if (preSpeciality != _speciality.ToLower())
                {

                    //This is a modified prompt that prompts not only in english
                    //The last parameter: patterns allows you to specify the words or phrases that will be recognized as valid answers for the prompt
                    //The first dimension determines the answers in the prompt that will be classified as true:
                    //for example, if in the first dimension we specify: no, No, NO, negativo and the prompt is in the form No - Yes, then clicking on Yes will return false
                    //but clicking on No or writing negativo, will return true, because the option in the first array was chosen
                    //So, to avoid confusions and in summary in the first component of the array should be present the patterns for positive answers
                    PromptDialog.Confirm(
                        context,
                        OnRetrySpeciality,
                        string.Format(K.SPEC_CLARIFY, _speciality),
                        K.GRAL_UNINTELIGIBLE_TRY_AGAIN,
                        3,
                        PromptStyle.Auto,
                        K.GRAL_YES_NO,
                        K.GRAL_YES_NO_OPTIONS
                        );
                }
                else
                {
                    context.Done(_speciality);
                }
            }
            else
            {
                await context.PostAsync(K.GRAL_UNINTELIGIBLE_TRY_AGAIN);
                await StartAsync(context);
            }
        }

        private async Task OnRetrySpeciality(IDialogContext context, IAwaitable<bool> result)
        {
            var correctSpeciality = await result;
            if (correctSpeciality) context.Done(_speciality);
            else
            {
                tries--;
                if (tries > 0)
                {
                    PromptDialog.Text(context, OnSetSpeciality, K.SPEC_SPECIALITY);
                }
                else
                {
                    TooManyAttemptsException e = new TooManyAttemptsException(K.GRAL_TOO_MANY_TRIES);
                    context.Fail(e);
                }
            }
        }



        /********************  Utilities */

        internal static string GetSpeciality(string preSpeciality)
        {
            string[] specialitiesSample = new string[] { "cardiología", "optometría", "pediatría", "dermatología", "oftalmología" };
            var levenshteinResults = Levenshtein.CaculateDistances(preSpeciality, specialitiesSample);
            var firstLevenshtein = levenshteinResults.ElementAt(0);
            if (firstLevenshtein.Key <= K.INT_LEVENSTEIN_MAX_DISTANCE)
                return firstLevenshtein.Value;
            else return String.Empty;
        }
    }
}