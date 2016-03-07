using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Thinkgate.Enums
{
    public enum ItemSearchCriteriaKeys
    {
        StandardSet,
        Grade,
        Subject,
        StandardCourse,
        ItemBank,
        ItemReservation,
        StandardFilter,
        TextSearch 
    }

    public enum ItemSearchModes
    {
        Normal,
        MultiSelect,
        SingleSelect
    }

    public enum ItemFilterModes
    {
        Normal, 
        Unfiltered
    }
}