using System;
using PX.Data;
using PX.TM;

namespace PX.Objects.TW
{
    [Serializable]
    public class ARTwilioNotificationProcessFilter : IBqlTable
    {
        #region CurrentOwnerID
        public abstract class currentOwnerID : IBqlField { }

        [PXDBGuid]
        public virtual Guid? CurrentOwnerID
        {
            get
            {
                return PXAccess.GetUserID();
            }
        }
        #endregion
        #region OwnerID
        public abstract class ownerID : IBqlField { }

        protected Guid? _OwnerID;
        [PXDBGuid]
        [PXUIField(DisplayName = "Assigned To")]
        [PX.TM.PXSubordinateOwnerSelector]
        public virtual Guid? OwnerID
        {
            get
            {
                return (_MyOwner == true) ? CurrentOwnerID : _OwnerID;
            }
            set
            {
                _OwnerID = value;
            }
        }
        #endregion
        #region MyOwner
        public abstract class myOwner : IBqlField { }

        protected Boolean? _MyOwner;
        [PXDBBool]
        [PXDefault(false)]
        [PXUIField(DisplayName = "Me")]
        public virtual Boolean? MyOwner
        {
            get
            {
                return _MyOwner;
            }
            set
            {
                _MyOwner = value;
            }
        }
        #endregion
        #region WorkGroupID
        public abstract class workGroupID : IBqlField { }

        protected Int32? _WorkGroupID;
        [PXDBInt]
        [PXUIField(DisplayName = "Workgroup")]
        [PXSelector(typeof(Search<EPCompanyTree.workGroupID,
            Where<EPCompanyTree.workGroupID, Owned<Current<AccessInfo.userID>>>>),
         SubstituteKey = typeof(EPCompanyTree.description))]
        public virtual Int32? WorkGroupID
        {
            get
            {
                return (_MyWorkGroup == true) ? null : _WorkGroupID;
            }
            set
            {
                _WorkGroupID = value;
            }
        }
        #endregion
        #region MyWorkGroup
        public abstract class myWorkGroup : PX.Data.IBqlField
        {
        }
        protected Boolean? _MyWorkGroup;
        [PXDefault(false)]
        [PXDBBool]
        [PXUIField(DisplayName = "My", Visibility = PXUIVisibility.Visible)]
        public virtual Boolean? MyWorkGroup
        {
            get
            {
                return _MyWorkGroup;
            }
            set
            {
                _MyWorkGroup = value;
            }
        }
        #endregion
        #region FilterSet
        public abstract class filterSet : IBqlField { }

        [PXDefault(false)]
        [PXDBBool]
        public virtual Boolean? FilterSet
        {
            get
            {
                return
                    this.OwnerID != null ||
                    this.WorkGroupID != null ||
                    this.MyWorkGroup == true;
            }
        }
        #endregion
        #region Action
        public abstract class action : IBqlField { }

        protected string _Action;

        [PXStringList(
                    new[] { "<SELECT>", TwilioNotificationType.SMS, TwilioNotificationType.OutBoundCall },
                    new[] { "<SELECT>", TwilioNotificationType.UI.SMS, TwilioNotificationType.UI.OutBoundCall })]
        [PXDefault("<SELECT>")]
        [PXUIField(DisplayName = "Action")]
        public virtual string Action
        {
            get
            {
                return this._Action;
            }
            set
            {
                this._Action = value;
            }
        }
        #endregion

        #region BeginDate
        public abstract class beginDate : IBqlField { }

        protected DateTime? _BeginDate;
        [PXDate()]
        [PXUIField(DisplayName = "Start Date", Visibility = PXUIVisibility.Visible)]
        [PXDefault(typeof(AccessInfo.businessDate), PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual DateTime? BeginDate
        {
            get
            {
                return this._BeginDate;
            }
            set
            {
                this._BeginDate = value;
            }
        }
        #endregion
        #region EndDate
        public abstract class endDate : IBqlField { }

        protected DateTime? _EndDate;
        [PXDate()]
        [PXDefault(typeof(AccessInfo.businessDate), PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIField(DisplayName = "End Date", Visibility = PXUIVisibility.Visible)]
        public virtual DateTime? EndDate
        {
            get
            {
                return this._EndDate;
            }
            set
            {
                this._EndDate = value;
            }
        }
        #endregion
    }
}