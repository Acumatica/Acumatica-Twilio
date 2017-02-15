using System;
using System.Collections;
using PX.Data;
using PX.Objects.AR;
using PX.Objects.CS;

namespace PX.Objects.TW
{
    public class ARProcessTwilioNotifications : PXGraph<ARProcessTwilioNotifications>
    {
        public PXFilter<ARTwilioNotificationProcessFilter> Filter;
        public PXCancel<ARTwilioNotificationProcessFilter> Cancel;
        public PXAction<ARTwilioNotificationProcessFilter> EditDetail;

        [PXFilterable]
        public PXFilteredProcessing<ARInvoice, ARTwilioNotificationProcessFilter> ARDocumentList;
        public PXSetup<ARSetup> arsetup;

        [PXUIField(DisplayName = "", MapEnableRights = PXCacheRights.Update, MapViewRights = PXCacheRights.Update)]
        [PXEditDetailButton]
        public virtual IEnumerable editDetail(PXAdapter adapter)
        {
            if (ARDocumentList.Current != null)
            {
                PXRedirectHelper.TryRedirect(ARDocumentList.Cache, ARDocumentList.Current, "Document", PXRedirectHelper.WindowMode.NewWindow);
            }
            return adapter.Get();
        }

        public ARProcessTwilioNotifications()
        {
            ARSetup setup = arsetup.Current;
            PXUIFieldAttribute.SetEnabled(ARDocumentList.Cache, null, false);
            PXUIFieldAttribute.SetEnabled<ARInvoice.selected>(ARDocumentList.Cache, null, true);
            ARDocumentList.Cache.AllowInsert = false;
            ARDocumentList.Cache.AllowDelete = false;

            ARDocumentList.SetSelected<ARInvoice.selected>();
            ARDocumentList.SetProcessCaption(CR.Messages.Send);
            ARDocumentList.SetProcessAllCaption(CR.Messages.SendAll);
        }

        public virtual IEnumerable ardocumentlist(PXAdapter adapter)
        {
            Type select = GetBQLStatement();

            PXView view = new PXView(this, false, BqlCommand.CreateInstance(select));
            var startRow = PXView.StartRow;
            int totalRows = 0;
            var list = view.
             Select(null, null, PXView.Searches, PXView.SortColumns, PXView.Descendings, PXView.Filters,
                 ref startRow, PXView.MaximumRows, ref totalRows);
            PXView.StartRow = 0;
            return list;
        }

        protected virtual Type GetBQLStatement()
        {
            Type where = PX.TM.OwnedFilter.ProjectionAttribute.ComposeWhere(
                typeof(ARTwilioNotificationProcessFilter),
                typeof(ARInvoice.workgroupID),
                typeof(ARInvoice.ownerID));

            Type Where =
                typeof(Where<ARInvoice.hold, Equal<False>,
                                And<ARInvoice.released, Equal<True>,
                                And<ARInvoice.voided, Equal<False>,
                                And<ARInvoice.docType, Equal<ARInvoiceType.invoice>,
                                And<ARInvoice.curyDocBal, Greater<decimal0>,
                                 And<ARInvoice.docDate, LessEqual<Current<ARTwilioNotificationProcessFilter.endDate>>,
                                 And<ARInvoice.docDate, GreaterEqual<Current<ARTwilioNotificationProcessFilter.beginDate>>>>>>>>>);

            Type whereAnd;

            whereAnd = Filter.Current.Action == "<SELECT>" ? typeof(Where<True, Equal<False>>) :
                                                             Where;

            Type select =
                BqlCommand.Compose(
                    typeof(Select2<,,>), typeof(ARInvoice),
                    typeof(InnerJoinSingleTable<Customer, On<Customer.bAccountID, Equal<ARInvoice.customerID>>>),
                    typeof(Where2<,>),
                    typeof(Match<Customer, Current<AccessInfo.userName>>),
                    typeof(And2<,>), whereAnd,
                    typeof(And<>), where);
            return select;
        }

        public override bool IsDirty
        {
            get
            {
                return false;
            }
        }

        protected virtual void ARTwilioNotificationProcessFilter_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
        {
            ARTwilioNotificationProcessFilter filter = (ARTwilioNotificationProcessFilter)e.Row;

            if (filter != null && !String.IsNullOrEmpty(filter.Action) && (filter.Action != "<SELECT>"))
            {
                ARDocumentList.SetProcessDelegate<ARInvoiceEntry>(
                                delegate (ARInvoiceEntry graph, ARInvoice invoice)
                                {
                                    graph.Clear();
                                    graph.Document.Current = invoice;
                                    ARInvoiceEntryPXExt invGraphExt = graph.GetExtension<ARInvoiceEntryPXExt>();
                                    invGraphExt.SendTwilioNotification(graph, filter.Action);
                                });
            }
        }

        protected virtual void ARTwilioNotificationProcessFilter_Action_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
        {
            foreach (ARInvoice document in ARDocumentList.Cache.Updated)
            {
                ARDocumentList.Cache.SetDefaultExt<ARInvoice.selected>(document);
            }
        }

        protected virtual void ARTwilioNotificationProcessFilter_BeginDate_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
        {
            e.NewValue = Accessinfo.BusinessDate.Value.AddMonths(-1);
        }
    }
}