
namespace Thinkgate.Interfaces
{
    using Telerik.Web.UI;
    using Classes;
    
    public interface IBannerControl
    {
        void AddMenu(Banner.ContextMenu contextMenu, RadContextMenu menu);
        void HideContextMenu(Banner.ContextMenu contextMenu);
    }
}
