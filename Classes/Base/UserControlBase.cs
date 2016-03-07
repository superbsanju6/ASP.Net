using System.Xml;
using Standpoint.Core.Classes;
using System;
using System.Linq;
using System.Net;
using System.Web.UI;
using Thinkgate.Base.Classes;
using Thinkgate.Utilities;

namespace Thinkgate.Classes.Base
{
    public class UserControlBase : UserControl
    {
        #region Variables
        private SessionObject _sessionObject = default(SessionObject);
        private DistrictParms _districtParms = default (DistrictParms);
        #endregion Variables

        #region Protected Methods
        /// <summary>
        /// Get the sessionobject
        /// </summary>
        protected SessionObject SessionObject
        {
            get
            {
                return _sessionObject ?? (_sessionObject = (SessionObject)Session["SessionObject"]);
            }
            set { _sessionObject = value; }
        }

        /// <summary>
        /// Get the districtParams
        /// </summary>
        protected DistrictParms DistrictParms
        {
            get { return _districtParms ?? (_districtParms = DistrictParms.LoadDistrictParms()); }
            set { _districtParms = value; }

        }
       
        #endregion Protected Methods
    }
}