using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Thinkgate.Enums.ImprovementPlan
{
    public enum ActionType
    {
        None,
        Edit,
        View
    }

    public enum EventTargets
    {
        None,
        btnSave,
        btnCover,
        btnDelete,
        btnExcel,
        btnSaveAndAdd,
        ddlStrategy,
        btnPDF
    }
}
