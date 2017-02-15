using System;
using PX.Data;
using PX.Common;
using PX.Objects.AR;

namespace PX.Objects.TW
{
    public class ARInvoicePXExt : PXCacheExtension<ARInvoice>
    {
        #region UsrDocBalanceAmountToWords

        public abstract class usrDocBalanceAmountToWords : IBqlField { }

        [PX.Objects.AP.ToWords(typeof(ARInvoice.curyDocBal))]
        [PXUIField(DisplayName = "Balance Due (in words)")]
        public virtual string UsrDocBalanceAmountToWords { get; set; }

        #endregion
    }
}