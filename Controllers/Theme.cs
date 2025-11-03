using LSSyncApp.Objects;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace LSSyncApp.Controllers
{
    public class Theme
    {
        [Description("Indicates Theme Code -> 0 Indicates Light Theme, 1 Indicates Dark Theme")]
        private int iiTheme;
        private string isName;
        private readonly Color icWhite = Color.White, icGrey = Color.FromArgb(220, 221, 225);
        private readonly Color icPanelBlue = Color.FromArgb(21, 21, 21), icFormBlue = Color.FromArgb(80, 80, 80);
        //private readonly Color icPanelBlue = Color.FromArgb(51, 51, 97), icFormBlue = Color.FromArgb(41, 41, 75);
        private readonly Color icGreen = Color.FromArgb(0, 192, 0), icGold = Color.Gold;
        private readonly Color icViolet = Color.FromArgb(127, 127, 254), icNavWhite = Color.NavajoWhite;
        private readonly Color icDarkViolet = Color.FromArgb(26, 26, 64), icSilver = Color.FromArgb(238, 238, 238);
        private readonly Color icGreenTheme = Color.FromArgb(239, 254, 249);

        //aiTheme ==> 0-Light, 1-Dark
        public void SetTheme(Form afForm, int aiTheme)
        {
            iiTheme = aiTheme;
            isName = afForm.Name;
            //if (isName == "MainForm") return;

            afForm.BackColor = iiTheme == 0 ? icGrey : icFormBlue;
            foreach (Control control in afForm.Controls)
            {
                Colors(control);
                if (control.Controls != null) ChildControls(control);
            }
        }

        private void ChildControls(Control acCtrl)
        {
            foreach (Control acChildCtrl in acCtrl.Controls)
            {
                Colors(acChildCtrl);
                if (acChildCtrl.Controls != null) ChildControls(acChildCtrl);
            }
        }

        private void Colors(Control acCtrl)
        {
            if (acCtrl.TabIndex >= 1000 && acCtrl.TabIndex <= 10000) return;
            switch(acCtrl.GetType().Name)
            {
                case "Panel":
                    if(isName == "Dashboard")
                    {
                        acCtrl.BackColor = iiTheme == 0 ? icPanelBlue : icWhite;
                    }
                    else
                    {
                        acCtrl.BackColor = iiTheme == 0 ? icWhite : icPanelBlue;
                    }
                    break;
                case "CircularProgressBar":
                    ((CircularProgressBar)acCtrl).TextColor = iiTheme == 0 ? icPanelBlue : icWhite;
                    acCtrl.BackColor = iiTheme == 0 ? icWhite : icPanelBlue;
                    break;
                case "SplitContainer":
                    acCtrl.Paint += Panel_Paint;
                    /*Color splitterColor = iiTheme == 0 ? icPanelBlue : icWhite;
                    acCtrl.BackColor = splitterColor;
                    using (SolidBrush brush = new SolidBrush(splitterColor))
                    {
                        //splitContainer.BackColor = splitterColor;
                        // Determine the rectangle of the splitter area
                        if (((SplitContainer)acCtrl).Orientation == Orientation.Vertical)
                        {
                            // For vertical splitters
                            e.Graphics.FillRectangle(brush, ((SplitContainer)acCtrl).SplitterDistance, 0, ((SplitContainer)acCtrl).SplitterWidth, ((SplitContainer)acCtrl).Height);
                        }
                        else
                        {
                            // For horizontal splitters
                            e.Graphics.FillRectangle(brush, 0, ((SplitContainer)acCtrl).SplitterDistance, ((SplitContainer)acCtrl).Width, ((SplitContainer)acCtrl).SplitterWidth);
                        }
                    }*/
                    break;
                case "SplitterPanel":
                    acCtrl.BackColor = iiTheme == 0 ? icWhite : icPanelBlue;
                    if (acCtrl.Tag != null)
                    {
                        if (int.TryParse(acCtrl.Tag.ToString(), out _))
                        {
                            if (int.Parse(acCtrl.Tag.ToString()) == 1000)
                            {
                                acCtrl.BackgroundImage = iiTheme == 0 ? Properties.Resources.CompanyLogo : Properties.Resources.CompanyLogoW;
                            }
                        }
                    }
                    break;
                case "Chart":
                    acCtrl.BackColor = iiTheme == 0 ? icWhite : icPanelBlue;
                    int liCount = ((Chart)acCtrl).Legends.Count;
                    for (int i = 0; i < liCount; i++) ((Chart)acCtrl).Legends[i].ForeColor = iiTheme == 0 ? icPanelBlue : icWhite;
                    liCount = ((Chart)acCtrl).ChartAreas.Count;
                    for (int i = 0; i < liCount; i++)
                    {
                        ((Chart)acCtrl).ChartAreas[i].AxisX.LabelStyle.ForeColor = iiTheme == 0 ? icPanelBlue : icWhite;
                        ((Chart)acCtrl).ChartAreas[i].AxisY.LabelStyle.ForeColor = iiTheme == 0 ? icPanelBlue : icWhite;
                        ((Chart)acCtrl).ChartAreas[i].AxisX.TitleForeColor = iiTheme == 0 ? icPanelBlue : icWhite;
                        ((Chart)acCtrl).ChartAreas[i].AxisY.TitleForeColor = iiTheme == 0 ? icPanelBlue : icWhite;
                        ((Chart)acCtrl).ChartAreas[i].BackColor = iiTheme == 0 ? icWhite : icPanelBlue;
                    }
                    break;
                case "DataGridView":
                    //if (isName == "SyncData") return;
                    ((DataGridView)acCtrl).RowTemplate.DefaultCellStyle = new DataGridViewCellStyle
                    {
                        ForeColor = iiTheme == 0 ? icPanelBlue : icWhite
                    };
                    
                    ((DataGridView)acCtrl).RowsDefaultCellStyle.BackColor = iiTheme == 0 ? icWhite : icPanelBlue;
                    ((DataGridView)acCtrl).RowsDefaultCellStyle.ForeColor = iiTheme == 0 ?  icPanelBlue : icWhite;
                    ((DataGridView)acCtrl).AlternatingRowsDefaultCellStyle.BackColor = iiTheme == 0 ? icGreenTheme : icDarkViolet;
                    //((DataGridView)acCtrl).Controls[1].ForeColor = iiTheme == 0 ? icGreenTheme : icDarkViolet;
                    

                    int liSortColIndex;
                    ListSortDirection listSortDirection;
                    if (((DataGridView)acCtrl).SortedColumn != null)
                    {
                        liSortColIndex = ((DataGridView)acCtrl).SortedColumn.Index;
                        if (((DataGridView)acCtrl).SortOrder == SortOrder.Ascending)
                            listSortDirection = ListSortDirection.Ascending;
                        else
                            listSortDirection = ListSortDirection.Descending;
                    }
                    else
                    {
                        liSortColIndex = 0;
                        listSortDirection = ListSortDirection.Ascending;
                    }

                    if (((DataGridView)acCtrl).Rows.Count > 0 && ((DataGridView)acCtrl).SortedColumn != null)
                        ((DataGridView)acCtrl).Sort(((DataGridView)acCtrl).Columns[liSortColIndex], 
                            listSortDirection == ListSortDirection.Ascending ? ListSortDirection.Descending : ListSortDirection.Ascending);

                    ((DataGridView)acCtrl).EnableHeadersVisualStyles = true;
                    
                    ((DataGridView)acCtrl).ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
                    { 
                        BackColor = iiTheme == 0 ? icGreenTheme : icViolet,
                        ForeColor = iiTheme == 0 ? icPanelBlue : icWhite,
                        Font = new Font("Ebrima", 9, FontStyle.Bold),
                        Alignment = DataGridViewContentAlignment.MiddleCenter
                    };
                    
                    ((DataGridView)acCtrl).EnableHeadersVisualStyles = false;
                    ((DataGridView)acCtrl).BackgroundColor = iiTheme == 0 ? icWhite : icPanelBlue;

                    ((DataGridView)acCtrl).GridColor = iiTheme == 0 ? icWhite : icPanelBlue;

                    if (((DataGridView)acCtrl).Rows.Count > 0 && ((DataGridView)acCtrl).SortedColumn != null)
                        ((DataGridView)acCtrl).Sort(((DataGridView)acCtrl).Columns[liSortColIndex], listSortDirection);

                    ((DataGridView)acCtrl).ClearSelection();
                    ((DataGridView)acCtrl).Refresh();
                    break;
                case "Button":
                    if (isName == "Dashboard")
                    {
                        acCtrl.BackColor = iiTheme == 0 ? icWhite : icPanelBlue;
                        acCtrl.ForeColor = iiTheme == 0 ? icGreen : icGold;
                    }
                    else
                    {
                        if (acCtrl.BackgroundImage == null)
                        {
                            acCtrl.BackColor = iiTheme == 0 ? icWhite : icPanelBlue;
                            acCtrl.ForeColor = iiTheme == 0 ? icPanelBlue : icWhite;
                        }
                        if (acCtrl.Text == "" || acCtrl.Text == null)
                        {
                            acCtrl.BackColor = iiTheme == 0 ? icGrey : icPanelBlue;
                        }
                    }
                    break;
                case "GroupBox":
                case "RichTextBox":
                case "Label":
                    if(acCtrl.TabIndex > 10000)
                    {
                        acCtrl.BackColor = iiTheme == 0 ? icWhite : icPanelBlue;
                    }
                    else
                    {
                        acCtrl.ForeColor = iiTheme == 0 ? icPanelBlue : icWhite;
                        acCtrl.BackColor = iiTheme == 0 ? icWhite : icPanelBlue;
                    }
                    break;
                case "Shapes":
                    ((Shapes)acCtrl).FillColor = iiTheme == 0 ? icWhite : icPanelBlue;
                    ((Shapes)acCtrl).LineColor = iiTheme == 0 ? icWhite : icPanelBlue;
                    break;
                case "ToolStrip":
                    //acCtrl.BackgroundImage = iiTheme == 0 ? Properties.Resources.CyanBG : Properties.Resources.PurpleBG;
                    //acCtrl.BackgroundImageLayout = ImageLayout.Stretch;
                    acCtrl.BackColor = iiTheme == 0 ? icWhite : icPanelBlue;
                    break;
                case "StatusStrip":
                    acCtrl.BackColor = iiTheme == 0 ? icWhite : icPanelBlue;
                    foreach (ToolStripStatusLabel lbl in ((StatusStrip)acCtrl).Items)
                    {
                        lbl.ForeColor = iiTheme == 0 ? icPanelBlue : icWhite;
                    }
                    break;
                case "PictureBox":
                    acCtrl.BackColor = iiTheme == 0 ? icWhite : icPanelBlue;
                    break;
            }
        }

        private void Panel_Paint(object sender, PaintEventArgs e)
        {
            SplitContainer splitContainer = (SplitContainer)sender;
            Color splitterColor = iiTheme == 0 ? icGrey : icFormBlue;
            splitContainer.BackColor = splitterColor;
            using (SolidBrush brush = new SolidBrush(splitterColor))
            {
                //splitContainer.BackColor = splitterColor;
                // Determine the rectangle of the splitter area
                if (splitContainer.Orientation == Orientation.Vertical)
                {
                    // For vertical splitters
                    e.Graphics.FillRectangle(brush, splitContainer.SplitterDistance, 0, splitContainer.SplitterWidth, splitContainer.Height);
                }
                else
                {
                    // For horizontal splitters
                    e.Graphics.FillRectangle(brush, 0, splitContainer.SplitterDistance, splitContainer.Width, splitContainer.SplitterWidth);
                }
            }
        }
    }
}
