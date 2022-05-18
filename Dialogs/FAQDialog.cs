using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Bot.Connector;
using System.Threading.Tasks;
using System.Threading;
using QnAMakerDialog.Models;
using QnAMakerDialog;
using System.Linq;

namespace GreatWall
{
    [Serializable]
    //[QNAMakerService(Host, EndpoingKey, Knowledgebases, MaxAnsers = 0)]
    [QnAMakerService(" https://greatwallqna-brian.azurewebsites.net/qnamaker", "6a3cd7b9-c816-42f7-92e2-cc8f394710fc"
        , "cd6a8dc3-cd97-4dbf-a994-8b0633a83ad5", MaxAnswers = 5)]

    public class FAQDialog : QnAMakerDialog<string>
    {
        //this method is called automatically when there are no results for the question.
        public override async Task NoMatchHandler(IDialogContext context, string originalquerytext)
        {
            await context.PostAsync($"sorry, i couldn't find an answer for'{originalquerytext}'.");

            context.Wait(MessageReceived);
        }
        //this method is called automatically when there is a results for the question.
        public override async Task DefaultMatchHandler(IDialogContext context, string originalquerytext, QnAMakerResult result)
        {
            if (originalquerytext == "exit")
            {
                context.Done("");
                return;
            }
            await context.PostAsync(result.Answers.First().Answer);

            context.Wait(MessageReceived);
        }
        [QnAMakerResponseHandler(0.5)]
        //this merhod is called when there is a low-order result.
        public async Task LowScoreHandler(IDialogContext context, string originalquerytext, QnAMakerResult result)
        {
            var messageactivity = ProcessResultAndCreateMessageActivity(context, ref result);
            messageactivity.Text = $"i found an answer that might help..." +
                                   $"{result.Answers.First().Answer}.";
            await context.PostAsync(messageactivity);
            context.Wait(MessageReceived);
        }
        //public async task startasync(idialogcontext context)
        //{
        //    await context.postasync("faq service: ");
        //    context.wait(messagereceivedasync);

        //}
        //public async task messagereceivedasync(idialogcontext context, iawaitable<object> result)
        //{
        //    activity activity = await result as activity;

        //    if (activity.text.trim() == "exit")
        //    {
        //        context.done("order completed");
        //    }
        //    else
        //    {
        //        await context.postasync("faq dialog.");
        //        context.wait(messagereceivedasync);
        //    }
        //}
    }

    //public class FAQDialog : IDialog<string>
    //{
    //    public async Task StartAsync(IDialogContext context)
    //    {
    //        await context.PostAsync("FAQ Service: ");
    //        context.Wait(MessageReceivedAsync);

    //    }
    //    public async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
    //    {
    //        Activity activity = await result as Activity;

    //        if (activity.Text.Trim() == "Exit")
    //        {
    //            context.Done("Order Completed");
    //        }
    //        else
    //        {
    //            await context.PostAsync("FAQ Dialog.");
    //            context.Wait(MessageReceivedAsync);
    //        }
    //    }
    //}
}