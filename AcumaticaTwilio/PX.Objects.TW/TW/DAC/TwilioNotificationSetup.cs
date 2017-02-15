using System;
using PX.Data;
using PX.SM;

namespace PX.Objects.TW
{
    [PXPrimaryGraph(typeof(TwilioNotificationSetupMaint))]
    [Serializable]
    public class TwilioNotificationSetup : IBqlTable
    {
        #region TwilioAccountSid

        public abstract class twilioAccountSid : IBqlField { }

        [PXRSACryptString(255)]
        [PXDefault]
        [PXUIField(DisplayName = "Twilio Account Sid")]
        public string TwilioAccountSid { get; set; }

        #endregion

        #region TwilioAuthToken

        public abstract class twilioAuthToken : IBqlField { }

        [PXRSACryptString(255)]
        [PXDefault]
        [PXUIField(DisplayName = "Twilio Auth Token")]
        public string TwilioAuthToken { get; set; }

        #endregion

        #region TwilioFromPhoneNumber

        public abstract class twilioFromPhoneNumber : IBqlField { }

        [PXDBString(255, IsUnicode = true, InputMask = "")]
        [PXDefault]
        [PXUIField(DisplayName = "Twilio From Phone Number")]
        public string TwilioFromPhoneNumber { get; set; }

        #endregion

        #region SMSNotificationID

        public abstract class sMSNotificationID : IBqlField { }

        [PXDBInt]
        [PXDefault]
        [PXSelector(typeof(Notification.notificationID),
            DescriptionField = typeof(Notification.name), ValidateValue = true)]
        [PXUIField(DisplayName = "Notification ID")]
        public int? SMSNotificationID { get; set; }

        #endregion

        #region CallNotificationID

        public abstract class callNotificationID : IBqlField { }

        [PXDBInt]
        [PXDefault]
        [PXSelector(typeof(Notification.notificationID),
            DescriptionField = typeof(Notification.name), ValidateValue = true)]
        [PXUIField(DisplayName = "Call Notification ID")]
        public int? CallNotificationID { get; set; }

        #endregion

        #region tstamp

        public abstract class Tstamp : IBqlField { }

        [PXDBTimestamp()]
        public virtual Byte[] tstamp { get; set; }

        #endregion

        #region CreatedByID

        public abstract class createdByID : IBqlField { }

        [PXDBCreatedByID()]
        public virtual Guid? CreatedByID { get; set; }

        #endregion

        #region CreatedByScreenID

        public abstract class createdByScreenID : IBqlField { }

        [PXDBCreatedByScreenID()]
        public virtual String CreatedByScreenID { get; set; }

        #endregion

        #region CreatedDateTime

        public abstract class createdDateTime : IBqlField { }

        [PXDBCreatedDateTime()]
        public virtual DateTime? CreatedDateTime { get; set; }

        #endregion

        #region LastModifiedByID

        public abstract class lastModifiedByID : IBqlField { }

        [PXDBLastModifiedByID()]
        public virtual Guid? LastModifiedByID { get; set; }

        #endregion

        #region LastModifiedByScreenID

        public abstract class lastModifiedByScreenID : IBqlField { }

        [PXDBLastModifiedByScreenID()]
        public virtual String LastModifiedByScreenID { get; set; }

        #endregion

        #region LastModifiedDateTime

        public abstract class lastModifiedDateTime : IBqlField { }

        [PXDBLastModifiedDateTime()]
        public virtual DateTime? LastModifiedDateTime { get; set; }

        #endregion
    }
}