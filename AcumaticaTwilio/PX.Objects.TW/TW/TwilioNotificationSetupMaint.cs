using System;
using PX.Data;

namespace PX.Objects.TW
{
    public class TwilioNotificationSetupMaint : PXGraph<TwilioNotificationSetupMaint>
    {
        public PXSave<TwilioNotificationSetup> Save;
        public PXCancel<TwilioNotificationSetup> Cancel;

        public PXSelect<TwilioNotificationSetup> Setup;
    }
}