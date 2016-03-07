// -----------------------------------------------------------------------
// <copyright file="Banner.cs" company="Standpoint">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Thinkgate.Classes
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using Telerik.Web.UI;

    /// <summary>
    /// TODO: Update summary.
    /// TODO: This should be refactored out, but for now it is all in one class.
    /// </summary>
    public class Banner
    {
        private readonly Dictionary<ContextMenu, RadContextMenu> menuItems = new Dictionary<ContextMenu, RadContextMenu>();

        #region Enumerations

        public enum ContextMenu
        {
            Help = 0,
            Actions

        }

        #endregion

        #region Properties

        [Description("The banner menu object for any type of banner that could be displayed.")]
        public Dictionary<ContextMenu, RadContextMenu> Menu
        {
            get
            {
                return this.menuItems;
            }
        }

        #endregion

        public void AddOnClientItemClicked(ContextMenu contextMenu, string onClientItemClicked)
        {
            AddContextMenuIfDoesNotExist(contextMenu);

            this.menuItems[contextMenu].OnClientItemClicked = onClientItemClicked;
        }

        public void AddMenuItem(ContextMenu contextMenu, RadMenuItem menuItem)
        {
            AddContextMenuIfDoesNotExist(contextMenu);

            this.menuItems[contextMenu].Items.Add(menuItem);
        }

        public void RemoveMenuItem(ContextMenu contextMenu, RadMenuItem menuItem)
        {
            this.menuItems[contextMenu].Items.Remove(menuItem);
        }

        public void AddMenuItem(ContextMenu contextMenu, string parentMenuItemText, RadMenuItem menuItem)
        {
            if (!this.menuItems.ContainsKey(contextMenu))
            {
                return;
            }

            var item = this.menuItems[contextMenu].FindItemByText(parentMenuItemText);
            if (item == null)
            {
                return;
            }

            item.Items.Add(menuItem);
        }

        private void AddContextMenuIfDoesNotExist(ContextMenu contextMenu)
        {
            if (!this.menuItems.ContainsKey(contextMenu))
            {
                this.menuItems.Add(contextMenu, new RadContextMenu());
            }
        }
    }
}
