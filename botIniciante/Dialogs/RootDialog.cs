using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using Newtonsoft.Json;
using System.Linq;

using botIniciante.Componemts;
using botIniciante.Models;

namespace botIniciante.Dialogs
{
    [Serializable]
    public class RootDialog : IDialog<object>
    {
        public Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);

            return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            //var activity = await result as Activity;

            //// Calculate something for us to return
            //int length = (activity.Text ?? string.Empty).Length;

            //// Return our reply to the user
            //await context.PostAsync($"You sent {activity.Text} which was {length} characters");

            //context.Wait(MessageReceivedAsync);

            var message = await result;

            var qnaResponse = await QNAComponent.MakeQNARequest(message.Text);

            var qna = JsonConvert.DeserializeObject<QnaResponse>(qnaResponse);

            if (qna.answers.First().answer != "No good match found in KB.")
            {
                //show QnA answer
                await context.PostAsync(qna.answers.First().answer);
                context.Wait(this.MessageReceivedAsync);
            }
            else
            {
                await context.PostAsync("Desculpe, não consegui entender sua pergunta!");
                context.Wait(this.MessageReceivedAsync);
            }
        }
    }
}