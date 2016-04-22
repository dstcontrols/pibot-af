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
        private static string userName = "hackuser046";
        private static string password = "ti6djbly2A$";
        private static string serverUrl = @"https://proghackuc2016.osisoft.com/piwebapi";
        private static string baseElement = @"\\JUPITER001\San Diego Airport\Utilities\Terminals";
        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>
public async Task<Message> Post([FromBody]Message message)
{
    if (message.Type == "Message")
    {
        // Lonnie's Bot
        string appId = @"9c1d7df5-92be-4ade-ab29-7affaa91b797";
        string subKey = @"d1a9a95fc7b5400cb7996db63ed26f66";
        // Lisa's Bot
        //string appId = @"dd7f913b-e392-40b5-826f-2b36a09112ff";
        //string subKey = @"3f4f7cbaaf70411cafef204258c658a1";

        string lroot = @"https://api.projectoxford.ai/luis/v1/application?id=" + appId + "&subscription-key=" + subKey + "&q=";

        string uri = lroot + Uri.EscapeDataString(message.Text);
        string val = "I did not understand...";
        using (var client = new HttpClient())
        {
            HttpResponseMessage msg = await client.GetAsync(uri);

            if (msg.IsSuccessStatusCode)
            {
                var response = await msg.Content.ReadAsStringAsync();
                var data = JsonConvert.DeserializeObject<piluis>(response);
                if(data.intents[0].intent == "GetKPI")
                {
                    var piService = new PIServices(serverUrl, baseElement, userName, password);
                    val =  await piService.GetKPI(data);
                }
            }

        }
        // return our reply to the user
        return message.CreateReplyMessage(val);
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