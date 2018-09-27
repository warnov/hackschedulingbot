using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;

namespace Contoso.SchedulingBot.BotAPI
{ 
    [Serializable]
    public class RootDialog : IDialog<object>
    {
        Scheduling _scheduling;
        public Task StartAsync(IDialogContext context)
        {
            _scheduling = new Scheduling();
            context.Wait(MessageReceivedAsync);
            return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var initMsg = (await result).Text;
            if (initMsg != "CMD")
            {
                await this.SendWelcomeMessageAsync(context);
            }
            else
            {
                await SpecialCommand(context);
            }
        }

        private async Task SendWelcomeMessageAsync(IDialogContext context)
        {
            await context.PostAsync(K.ROOT_WELCOME);
            context.Call(new PatientInfoDialog(), PatientInfoResumeAfter);            
        }

        private Task SpecialCommand(IDialogContext context)
        {
            // PromptDialog.Text(context, OnSpecialCommandProcessing, K.INT_SPECIAL_COMMAND);
            var sch = new Scheduling()
            {
                Speciality = "Cardiología",
                Patient = new Patient()
                {
                    Name = "Pedrito"
                }
            };
            context.Call(new SchedulingDialog(ref sch), OnSpecialCommandProcessing2);
            return Task.CompletedTask;
        }

        private async Task OnSpecialCommandProcessing2(IDialogContext context, IAwaitable<DateTime> result)
        {
            var res = await result;
            await context.PostAsync(res.ToColombianDateTimeString());
        }

        private async Task OnSpecialCommandProcessing(IDialogContext context, IAwaitable<string> result)
        {
            var command = await result;
            var dates = U.ExtractPossibleDatesFromText(command, out bool hasTime);
            await context.PostAsync(dates[0]);
            await context.PostAsync(K.INT_SPECIAL_COMMAND_FINISHED);
            context.Wait(MessageReceivedAsync);
        }

        private async Task PatientInfoResumeAfter(IDialogContext context, IAwaitable<Patient> result)
        {
            _scheduling.Patient = await result;
            context.Call(new SpecialityDialog(), SpecialityResumeAfter);
        }

        private async Task SpecialityResumeAfter(IDialogContext context, IAwaitable<string> result)
        {
            _scheduling.Speciality = await result;            
            context.Call(new SchedulingDialog(ref _scheduling), SchedulingResumeAfter);
        }

        private async Task SchedulingResumeAfter(IDialogContext context, IAwaitable<DateTime> result)
        {
            await context.PostAsync($"Ok {_scheduling.Patient.InformalName} agendaré tu cita para el {_scheduling.DateTime.ToColombianDateTimeString()}");
            context.Wait(MessageReceivedAsync);
        }
    }
}