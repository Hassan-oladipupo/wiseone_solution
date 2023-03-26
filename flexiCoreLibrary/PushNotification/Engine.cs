using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace flexiCoreLibrary.PushNotification
{
    public class Engine
    {
        const string serverKey = "AAAAosUSvDw:APA91bHP17ubavCi_dgR1Yr3_uddyBFH7XeaZRVPzUwnPrQiDFL5Ki_WtJiEhPqkmQ7CL8JXWRrP7xDuQwaH4PIXjSik4VzbseYU7gjsNyzHE9L5evZYjXQpm1rroQaGMFDzoS9rQvsg";

        public static Response SendMessage(List<string> userTokens, string messageText, string title, Enums.NotificationType type)
        {
            try
            {
                var result = new Response();

                if (userTokens.Any())
                {
                    var message = new Message()
                    {
                        registration_ids = userTokens,
                        notification = new Notification
                        {
                            title = title,
                            body = messageText,
                            sound = "default",
                            click_action = "FCM_PLUGIN_ACTIVITY"
                        },
                        data = new MessageContent()
                        {
                            message = messageText,
                            type = type.ToString()
                        },
                        priority = "HIGH"
                    };                    

                    var webAddr = "https://fcm.googleapis.com/fcm/send";

                    var httpWebRequest = (HttpWebRequest)WebRequest.Create(webAddr);
                    httpWebRequest.ContentType = "application/json";
                    httpWebRequest.Headers.Add("Authorization:key=" + serverKey);
                    httpWebRequest.Method = "POST";

                    using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                    {
                        string json = JsonConvert.SerializeObject(message);
                        streamWriter.Write(json);
                        streamWriter.Flush();
                    }

                    var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    {
                        result = JsonConvert.DeserializeObject<Response>(streamReader.ReadToEnd());
                    }                    
                }
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                throw ex;
            }
        }

    }
}
