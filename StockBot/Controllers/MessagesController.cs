using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Microsoft.Bot.Connector;
using Newtonsoft.Json;

namespace StockBot
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {
       

        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>
        public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
        {
            ConnectorClient connector = new ConnectorClient(new Uri(activity.ServiceUrl));
            Activity reply;
            if (activity.Type == ActivityTypes.Message)
            {
              
                StockBot.Rootobject stLuis = await StockBot.LUISStockClient.ParseUserInput(activity.Text);
                string strRet = string.Empty;
                string strStock = activity.Text;
                

                // Get the stateClient to get/set Bot Data
                StateClient _stateClient = activity.GetStateClient();
                BotData _botData = _stateClient.BotState.GetUserData(activity.ChannelId, activity.Conversation.Id);

                switch (stLuis.intents[0].intent)
                {
                    case "StockPrice":
                        strRet = await YahooStock.Yahoo.GetStock(stLuis.entities[0].entity);
                        break;
                    case "None":
                        strRet = "Sorry, I don't understand, perhaps try something like \"Show me Microsoft stock\"";
                        break;
                    default:
                        break;
                }

                reply = activity.CreateReply(strRet);
                // return our reply to the user

            }
            else
            {
                reply=HandleSystemMessage(activity);
            }
           
            await connector.Conversations.ReplyToActivityAsync(reply);
            var response = Request.CreateResponse(HttpStatusCode.OK);
            return response;
        }
        private Activity HandleSystemMessage(Activity message)
        {
            if (message.Type == ActivityTypes.DeleteUserData)
            {
                // Implement user deletion here
                // If we handle user deletion, return a real message
            }
            else if (message.Type == ActivityTypes.ConversationUpdate)
            {
                // Handle conversation state changes, like members being added and removed
                // Use Activity.MembersAdded and Activity.MembersRemoved and Activity.Action for info
                // Not available in all channels

                Activity reply= message.CreateReply("Hello sir, How may I help you?");
                return reply;

            }
            else if (message.Type == ActivityTypes.ContactRelationUpdate)
            {
                // Handle add/remove from contact lists
                // Activity.From + Activity.Action represent what happened

            }
            else if (message.Type == ActivityTypes.Typing)
            {
                // Handle knowing tha the user is typing
            }
            else if (message.Type == ActivityTypes.Ping)
            {
            }

            return null;
        }




    }

}