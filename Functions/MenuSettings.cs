using System.Windows.Forms;

namespace LSSyncApp.Functions
{
    public class MenuSettings
    {
        public static void EnableMenuItem(Form mdiForm, string MenuName)
        {
            ToolStrip ms = (ToolStrip)mdiForm.Controls["toolStrip"];
            //Access The First Level Menu Items
            ToolStripButton ti = (ToolStripButton)ms.Items[MenuName];
            //Then I use This control As my needs
            ti.Enabled = true;
        }
        public static void EnableMenuItem(Form mdiForm, string MenuName, string MenuItemName)
        {
            MenuStrip ms = (MenuStrip)mdiForm.Controls["toolStrip"];
            //Access The First Level Menu Items
            ToolStripMenuItem ti = (ToolStripMenuItem)ms.Items[MenuName];
            //Then I use This control As my needs
            ti.DropDownItems[MenuItemName].Enabled = true;
        }
        public static void EnableMenuItem(Form mdiForm, string L1MenuItem, string L2MenuItem, string MenuItemName)
        {
            MenuStrip ms = (MenuStrip)mdiForm.Controls["toolStrip"];
            //Access The First Level Menu Items
            ToolStripMenuItem ti1 = (ToolStripMenuItem)ms.Items[L1MenuItem];
            //Access The Sconed Level Menu Items
            ToolStripMenuItem ti2 = (ToolStripMenuItem)ti1.DropDownItems[L2MenuItem];
            //Then I use This control As my needs
            ti2.DropDownItems[MenuItemName].Enabled = true;
        }
    }
}
