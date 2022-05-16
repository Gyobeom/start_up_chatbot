using System;
using System.Threading.Tasks;

using Microsoft.Bot.Connector;
using Microsoft.Bot.Builder.Dialogs;
using System.Net.Http;
using System.Collections.Generic;  //ADD for List<>





namespace GreatWall
{
    [Serializable]
    public class RootDialog : IDialog<object>
    {
        protected int count = 1;
        string strMessage;
        private string strWelcomeMessage = "[Great Wall Bot]";

        public Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);
            return Task.CompletedTask;
        }

        public async Task SendWelcomeMessageAsync(IDialogContext context, IAwaitable<object> result)
        {
            Activity activity = await result as Activity;
            string strSelected = activity.Text.Trim();

            if (strSelected == "1")
            {
                //strMessage = "[Food order Menu] Select the menu you want to order.> ";
                //await context.PostAsync(strMessage);
                context.Call(new OrderDialog(), DialogResumeAfter);

            }
            else if (strSelected == "2")
            {
                strMessage = "[FAQ Service] Please enter a question >";
                await context.PostAsync(strMessage);
                context.Call(new FAQDialog(), DialogResumeAfter);
            }
            else if (strSelected == "3")
            {
                strMessage = "[LUIS] Please enter what you want >";
                await context.PostAsync(strMessage);

                context.Call(new LUISDialog(), DialogResumeAfter);
            }
            else
            {
                strMessage = "You have made a mistake. Please select again...";
                await context.PostAsync(strMessage);
                context.Wait(SendWelcomeMessageAsync);
            }


        }

        public async Task DialogResumeAfter(IDialogContext context, IAwaitable<string> result)
        {
            try
            {
                strMessage = await result;

                //await context.PostAsync(strWelcomeMessage);
                await this.MessageReceivedAsync(context, result);

            }
            catch (TooManyAttemptsException)
            {
                await context.PostAsync("Error occured....");
            }
        }

        public async Task AfterResetAsync(IDialogContext context, IAwaitable<bool> argument)
        {
            var confirm = await argument;
            if (confirm)
            {
                this.count = 1;
                await context.PostAsync("Reset count.");
            }
            else
            {
                await context.PostAsync("Did not reset count.");
            }
            context.Wait(MessageReceivedAsync);
        }

        public async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            
            await context.PostAsync(strWelcomeMessage);

            var message = context.MakeMessage();
            var actions = new List<CardAction>();

            actions.Add(new CardAction() { Title = "1. Order", Value = "1", Type = ActionTypes.ImBack });
            actions.Add(new CardAction() { Title = "2. FAQ", Value = "2", Type = ActionTypes.ImBack });
            actions.Add(new CardAction() { Title = "3. LUIS", Value = "3", Type = ActionTypes.ImBack });

            message.Attachments.Add(
                new HeroCard { Title = "Select 1 - 3.>", Buttons = actions }.ToAttachment());

            await context.PostAsync(message);

            context.Wait(SendWelcomeMessageAsync);
        }

    }

 
}