using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;

namespace Contoso.SchedulingBot.BotAPI
{
    [Serializable]
    public class PatientInfoDialog : IDialog<Patient>
    {
        private Patient _patient;
        public Task StartAsync(IDialogContext context)
        {
            _patient = new Patient();
            PromptDialog.Text(context, OnSetNameAskDocType, K.PAT_DATA_NAME);
            return Task.CompletedTask;
        }

        private async Task OnSetNameAskDocType(IDialogContext context, IAwaitable<string> result)
        {
            _patient.Name = await result;
            _patient.SetFirstCapital();
            await context.PostAsync($"{K.GRAL_THANKS} { _patient.InformalName }.");

            PromptDialog.Choice(
                context,
                OnDocTypeSetAskDocCode,
                Enum.GetNames(typeof(IdType)),  //Displays the list of document types in the IdType Enum
                K.PAT_DATA_ID_TYPE,
                K.GRAL_INVALID_OPTION,
                3
            );
        }

        private async Task OnDocTypeSetAskDocCode(IDialogContext context, IAwaitable<string> result)
        {
            string strIdType = await result;
            var idType = (IdType)Enum.Parse(typeof(IdType), strIdType);
            _patient.IdDocument.IdTypeValue = idType;
            PromptDialog.Text(context, OnDocCodeSetAskBirthDate, $"{K.PAT_DATA_ID_CODE} {idType.ToString()}");
        }

        private async Task OnDocCodeSetAskBirthDate(IDialogContext context, IAwaitable<string> result)
        {
            _patient.IdDocument.IdCode = await result;
            PromptDialog.Text(context, OnBirthDateSetExit, K.PAT_DATA_BIRTHDATE_OPTS.GetOne());
        }

        private async Task OnBirthDateSetExit(IDialogContext context, IAwaitable<string> result)
        {
            var res = await result;
            if(ExtractBirthDateTime(res, out DateTime tempBirthDate))
            {
                _patient.BirthDate = tempBirthDate;
                context.Done(_patient);
            }
            else
            {
                //show error
                await context.PostAsync(K.PAT_DATA_BIRTHDATE_ERROR);
                //ask birthdate again
                PromptDialog.Text(context, OnBirthDateSetExit, K.PAT_DATA_BIRTHDATE_OPTS.GetOne());
            }
        }




        /********************  Utilities */

        private static bool ExtractBirthDateTime(string res, out DateTime birthdate)
        {
            birthdate = DateTime.MinValue;                  
            var dateStrings = U.ExtractPossibleDatesFromText(res, out bool hasTime);
            if (dateStrings.Count > 0)
            {
                //just the first component as the year never will be the current one
                //so it is supposed that the user will enter it
                var dateString = dateStrings[0];
                birthdate = DateTime.Parse(dateString);

                //returns true iff the patient birthdate is less than today
                return birthdate <= DateTime.Now;
            }
            else return false;
        }



        //Human friendly confirmation of patient data
        internal static string PatientDataConfirmation(Patient patient)
        {
            return $"Excelente, {patient.InformalName}. " +
                $"Tu {patient.IdDocument.IdTypeValue.ToString()} es " +
                $"{patient.IdDocument.IdCode} y naciste el " +
                $"{patient.BirthDate.ToString("D", new CultureInfo("es-ES"))}";
        }
    }
}

 