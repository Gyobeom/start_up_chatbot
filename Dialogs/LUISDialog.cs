using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Bot.Builder.Dialogs;
using System.Net.Http;

using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using System.Threading.Tasks;
using Microsoft.Bot.Connector;

namespace GreatWall
{   
    [LuisModel("78dda2ad-76af-481e-8a5c-ff1cd454c686", "b00ec8902d914480b66e49316740f1da" , domain: "australiaeast.api.cognitive.microsoft.com")]
    [Serializable]
    public class LUISDialog : LuisDialog<string>
    {
        [LuisIntent("")]
        [LuisIntent("None")]
        public async Task None(IDialogContext context, LuisResult result)
        {
            string message = $"죄송합니다. 말씀을 이해하지 못했습니다.";

            await context.PostAsync(message);
            context.Wait(this.MessageReceived);
        }
        [LuisIntent("Order")]
        public async Task Order(IDialogContext context, IAwaitable<IMessageActivity>
            activity, LuisResult result)
        {
            var message = await activity;

            EntityRecommendation menuEntityRecommnedation;
            EntityRecommendation sizeEntityRecommnedation;
            EntityRecommendation quantityEntityRecommnedation;

            string menu = "";
            string size = "보통";
            string quantity = "한그릇";

            if(result.TryFindEntity("menu",out menuEntityRecommnedation))
            {
                menu = menuEntityRecommnedation.Entity.Replace(" ", "");

            }
            else
            {
                await context.PostAsync("없는 메뉴를 선택했습니다.");

                context.Wait(this.MessageReceived);
                return;
            }

            if(result.TryFindEntity("size", out sizeEntityRecommnedation))
            {
                size = sizeEntityRecommnedation.Entity.Replace(" ", "");
            }

            if(result.TryFindEntity("quantity", out quantityEntityRecommnedation))
            {
                quantity = quantityEntityRecommnedation.Entity.Replace(" ", "");
            }
            await context.PostAsync($"{menu} {size} {quantity}를 주문하셨습니다.");
            context.Wait(this.MessageReceived);
        }
        [LuisIntent("Delivery")]
        public async Task Delivery(IDialogContext context, IAwaitable<IMessageActivity>
                        activity, LuisResult result)
        {
            await context.PostAsync("출발 했습니다. 잠시만 기다려 주세요");
            context.Wait(this.MessageReceived);
        }

        [LuisIntent("Finish")]
        public async Task Finish(IDialogContext context, IAwaitable<IMessageActivity>
            activity, LuisResult result)
        {
            await context.PostAsync("주문 완료 되었습니다. 감사합니다.");
            context.Done("주문완료");
        }
    }
}