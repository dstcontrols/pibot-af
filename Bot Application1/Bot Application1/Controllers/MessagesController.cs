using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Connector.Utilities;
using Newtonsoft.Json;

namespace Bot_Application1
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {
        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>
        public async Task<Message> Post([FromBody]Message message)
        {
            if (message.Type == "Message")
            {
                string lroot = "https://api.projectoxford.ai/luis/v1/application?id=dd7f913b-e392-40b5-826f-2b36a09112ff&subscription-key=3f4f7cbaaf70411cafef204258c658a1&q=";
                string uri = lroot + Uri.EscapeDataString(message.Text);

                using (var client = new HttpClient())
                {
                    HttpResponseMessage msg = await client.GetAsync(uri);

                    if (msg.IsSuccessStatusCode)
                    {
                        var response = await msg.Content.ReadAsStringAsync();
                        var data = JsonConvert.DeserializeObject<piluis>(response);
                        
                    }

                }


                // return our reply to the user
                return message.CreateReplyMessage("test");
            }
            else
            {
                return HandleSystemMessage(message);
            }
        }

        private Message HandleSystemMessage(Message message)
        {
            if (message.Type == "Ping")
            {
                Message reply = message.CreateReplyMessage();
                reply.Type = "Ping";
                return reply;
            }
            else if (message.Type == "DeleteUserData")
            {
                // Implement user deletion here
                // If we handle user deletion, return a real message
            }
            else if (message.Type == "BotAddedToConversation")
            {
            }
            else if (message.Type == "BotRemovedFromConversation")
            {
            }
            else if (message.Type == "UserAddedToConversation")
            {
            }
            else if (message.Type == "UserRemovedFromConversation")
            {
            }
            else if (message.Type == "EndOfConversation")
            {
            }

            return null;
        }
    }
}