using System;
using PX.Common;

namespace PX.Objects.TW
{
    [PXLocalizable(Messages.Prefix)]
    public class Messages
    {
        public const string Prefix = "Twilio Integration Error:";
        public const string TwilioAccountNotSetup = "Twilio integration account has not been configured yet.";
        public const string InvoiceBillingContactNotExists = "Billing contact doesn't exist or phone # not specified.";
        public const string SMSNotificationIDNotSpecified = "Notification ID is not specified for sending SMS.";
        public const string CallNotificationIDNotSpecified = "Notification ID is not specified for out-bound call.";
        public const string NotificationNotFound = "Specified Notification is not found.";
        public const string SMSSubjectPrefix = "SMS Notification";
        public const string CallSubjectPrefix = "Out-bound call Notification";
        public const string UNSpecifiedTwilioNotificationType = "Specified Twilio Notification Type is not valid.";
    }
}
