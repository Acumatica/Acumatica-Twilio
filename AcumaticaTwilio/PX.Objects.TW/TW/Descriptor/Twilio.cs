using System;
using System.Collections.Specialized;
using System.Linq;
using PX.Data;
using Twilio;

namespace PX.Objects.TW
{
    public static class NameValueCollectionExtensions
    {
        /// <summary>
        /// NameValueCollection extension that transforms the collection into a string with Uri encoded values 
        /// for an HTTP Request
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        public static string ToQueryString(this NameValueCollection collection, string startChar = "?")
        {
            var array = (from key in collection.AllKeys
                         from value in collection.GetValues(key)
                         select string.Format("{0}={1}", Uri.EscapeDataString(key), Uri.EscapeDataString(value))).ToArray();

            return startChar + string.Join("&", array);
        }
    }

    public class TwilioNotification
    {
        private static string TwimletBase = "http://twimlets.com/message";
        private TwilioRestClient _client;

        public string Origin { get; set; }

        public TwilioNotification(string sid, string token)
        {
            _client = new TwilioRestClient(sid, token);
        }

        public static string MessageUrl(params string[] messages)
        {
            var messageCollection = new NameValueCollection();

            for (int i = 0; i < messages.Length; i++)
            {
                messageCollection.Add("Message[" + i + "]", messages[i]);
            }

            return TwimletBase + messageCollection.ToQueryString();
        }

        public void SendSMS(string number, string message)
        {
            var msg = _client.SendMessage(Origin, number, message);

            if (msg.RestException != null)
            {
                throw new PXException(msg.RestException.Message);
            }
        }

        public void SendCall(string number, string message)
        {
            var call = _client.InitiateOutboundCall(Origin, number, MessageUrl(message));

            if (call.RestException != null)
            {
                throw new PXException(call.RestException.Message);
            }
        }

    }

}