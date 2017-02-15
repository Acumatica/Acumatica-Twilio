using System;
using PX.Data;
using PX.Objects.AR;
using System.Collections;
using PX.Objects.EP;
using PX.Objects.CR;
using System.Collections.Generic;

namespace PX.Objects.TW
{
    public class ARInvoiceEntryPXExt : PXGraphExtension<ARInvoiceEntry>
    {
        #region Selects/Views

        public PXSetup<TwilioNotificationSetup> TwilioSetupInfo;

        #endregion

        #region Constructor

        public override void Initialize()
        {
            TwilioNotificationSetup setup = TwilioSetupInfo.Current;

            Base.action.AddMenuAction(SendSMSNotification);
            Base.action.AddMenuAction(SendCallNotification);
        }

        #endregion

        #region BLC Events

        //Configure UI 
        protected virtual void ARInvoice_RowSelected(PXCache sender, PXRowSelectedEventArgs e, PXRowSelected InvokeBaseHandler)
        {
            if (InvokeBaseHandler != null)
                InvokeBaseHandler(sender, e);

            ARInvoice document = e.Row as ARInvoice;
            if (document == null) return;

            var enabled = (document.CuryDocBal.HasValue ? document.CuryDocBal.Value > 0m : false);

            enabled = (enabled) && (document.Released.Value) && (document.DocType == ARInvoiceType.Invoice);

            SendSMSNotification.SetEnabled(enabled);
            SendCallNotification.SetEnabled(enabled);
        }

        #endregion

        #region Actions

        public PXAction<ARInvoice> SendSMSNotification;

        [PXUIField(DisplayName = "Send SMS Notification", MapEnableRights = PXCacheRights.Insert, MapViewRights = PXCacheRights.Select)]
        [PXProcessButton]
        public virtual IEnumerable sendSMSNotification(PXAdapter adapter)
        {
            ARInvoice invoice = PXCache<ARInvoice>.CreateCopy(Base.Document.Current);
            PXLongOperation.StartOperation(Base, delegate
            {
                ARInvoiceEntry invGraph = PXGraph.CreateInstance<ARInvoiceEntry>();
                invGraph.Document.Current = invoice;
                ARInvoiceEntryPXExt invGraphExt = invGraph.GetExtension<ARInvoiceEntryPXExt>();
                invGraphExt.SendTwilioNotification(invGraph, TwilioNotificationType.SMS);
            });
            return adapter.Get();
        }

        public PXAction<ARInvoice> SendCallNotification;

        [PXUIField(DisplayName = "Send Call Notification", MapEnableRights = PXCacheRights.Insert, MapViewRights = PXCacheRights.Select)]
        [PXButton(SpecialType = PXSpecialButtonType.Cancel)]
        public virtual IEnumerable sendCallNotification(PXAdapter adapter)
        {
            ARInvoice invoice = PXCache<ARInvoice>.CreateCopy(Base.Document.Current);
            PXLongOperation.StartOperation(Base, delegate
            {
                ARInvoiceEntry invGraph = PXGraph.CreateInstance<ARInvoiceEntry>();
                invGraph.Document.Current = invoice;
                ARInvoiceEntryPXExt invGraphExt = invGraph.GetExtension<ARInvoiceEntryPXExt>();
                invGraphExt.SendTwilioNotification(invGraph, TwilioNotificationType.OutBoundCall);
            });
            return adapter.Get();
        }

        #endregion

        #region Private helpers

        public void SendTwilioNotification(ARInvoiceEntry invGraph, string notificationType, bool isMassProcess = false)
        {
            ARInvoiceEntryPXExt invGraphExt = invGraph.GetExtension<ARInvoiceEntryPXExt>();

            //Raise error if Twilio Integration Setup not configured
            if (invGraphExt.TwilioSetupInfo.Current == null)
                throw new PXException(Messages.TwilioAccountNotSetup);

            //Get current billing contact
            if (invGraph.Billing_Contact.Current == null)
                invGraph.Billing_Contact.Current = invGraph.Billing_Contact.Select();
            ARContact contact = invGraph.Billing_Contact.Current;

            if (contact == null || String.IsNullOrEmpty(contact.Phone1))
                throw new PXException(Messages.InvoiceBillingContactNotExists);

            int? iNotificationID = (notificationType == TwilioNotificationType.SMS) ?
                                    invGraphExt.TwilioSetupInfo.Current?.SMSNotificationID :
                                    invGraphExt.TwilioSetupInfo.Current?.CallNotificationID;

            //Raise notification appropriate error message
            if (notificationType == TwilioNotificationType.SMS)
            {
                if (iNotificationID == null)
                    throw new PXException(Messages.SMSNotificationIDNotSpecified);
            }
            else if (notificationType == TwilioNotificationType.OutBoundCall)
            {
                if (iNotificationID == null)
                    throw new PXException(Messages.CallNotificationIDNotSpecified);
            }
            else
                throw new PXException(Messages.UNSpecifiedTwilioNotificationType);

            //Get the notification
            PX.SM.Notification notification = PXSelect<PX.SM.Notification,
                                    Where<PX.SM.Notification.notificationID, Equal<Required<PX.SM.Notification.notificationID>>>>.
                                    Select(invGraph, invGraphExt.TwilioSetupInfo.Current.SMSNotificationID);

            if (notification == null)
                throw new PXException(Messages.NotificationNotFound);

            //Process the datafields and get Subject and Notification Body ready.
            string subjectNotification = String.Format("{0} - {1}", (notificationType == TwilioNotificationType.SMS) ?
                                            Messages.SMSSubjectPrefix : Messages.CallSubjectPrefix,
                                            PX.Data.Wiki.Parser.PXTemplateContentParser.
                                            Instance.Process(notification.Subject, invGraph,
                                                             invGraph.Document.Current.GetType(), null));
            string bodyNotification = PX.Data.Wiki.Parser.PXTemplateContentParser.
                                         Instance.Process(notification.Body, invGraph,
                                                          invGraph.Document.Current.GetType(), null);

            //Create Twilio Notification
            var twilio = new TwilioNotification(invGraphExt.TwilioSetupInfo.Current.TwilioAccountSid,
                                                invGraphExt.TwilioSetupInfo.Current.TwilioAuthToken)
            {
                Origin = invGraphExt.TwilioSetupInfo.Current.TwilioFromPhoneNumber
            };

            if (notificationType == TwilioNotificationType.SMS)
            {
                twilio.SendSMS(contact.Phone1, PX.Data.Search.SearchService.Html2PlainText(bodyNotification));
            }
            else if (notificationType == TwilioNotificationType.OutBoundCall)
            {
                twilio.SendCall(contact.Phone1, PX.Data.Search.SearchService.Html2PlainText(bodyNotification));
            }
            else
                throw new PXException(Messages.UNSpecifiedTwilioNotificationType);

            //Create Activity in Acumataica
            CreateTwilioNotificationActivity(invGraph, subjectNotification, bodyNotification);
        }

        private void CreateTwilioNotificationActivity(ARInvoiceEntry invGraph, string Subject, string MessageBody)
        {
            try
            {
                invGraph.Actions["NewActivity"].Press();
            }
            catch (Exception ex)
            {
                if (ex is PXRedirectRequiredException)
                {
                    try
                    {
                        CRActivityMaint graph = (ex as PXRedirectRequiredException).Graph as CRActivityMaint;
                        if (graph != null)
                        {
                            CRActivity callActivity = graph.Activities.Current;
                            callActivity.Type = "P";
                            callActivity.Subject = Subject;
                            callActivity.Body = MessageBody;
                            callActivity = graph.Activities.Update(callActivity);

                            graph.Actions.PressSave();
                        }
                    }
                    catch (Exception graphErr)
                    {
                        throw graphErr;
                    }
                }
            }
        }

        #endregion
    }

    public class TwilioNotificationType
    {
        public const string SMS = "S";
        public const string OutBoundCall = "O";

        public class UI
        {
            public const string SMS = "SMS";
            public const string OutBoundCall = "Out-bound call";
        }

        public class sMS : Constant<String>
        {
            public sMS()
                : base(SMS)
            {
            }
        }

        public class outBoundCall : Constant<String>
        {
            public outBoundCall()
                : base(OutBoundCall)
            {
            }
        }
    }
}