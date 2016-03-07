using Thinkgate.Classes;
using System.Web.UI.WebControls;
using System.Web.UI;

namespace Thinkgate.Interfaces
{
    public interface IRotatorControl
    {
        void ChangeCarouselTwoVisibility(bool visible);

        void ClearRotators();

        Control GetRotator1();
        Control GetRotator2();
        Panel GetButtonsContainer1();
        Panel GetButtonsContainer2();

        void AddItemToRotator1(Container container);
        void AddItemToRotator2(Container container);

        void AddNavigationButtons(Folder folder = null);
    }
}