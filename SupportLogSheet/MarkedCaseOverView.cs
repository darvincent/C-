//收藏case 分类概览
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace SupportLogSheet
{
    public partial class MarkedCaseOverView : Form
    {
        private MarkedCases FORM_MyMarkedCase;
        private bool SortType = true;
        public MarkedCaseOverView(MarkedCases myMarkedCase)
        {
            InitializeComponent();
            List<string> categories = myMarkedCase.mf.MarkedCateCases.Keys.ToList();
            listView1.BeginUpdate();
            for (int i = 0; i < categories.Count; i++)
            {
                ListViewItem lvi = new ListViewItem();
                lvi.SubItems.Add(categories[i]);
                lvi.SubItems.Add(myMarkedCase.mf.getCateDescription(categories[i]));
                listView1.Items.Add(lvi);
            }
            listView1.EndUpdate();
            utility.setFont(this, Config.Font_Content);
            FORM_MyMarkedCase = new MarkedCases(1);
            FORM_MyMarkedCase = myMarkedCase;
        }

        private void listView1_DoubleClick(object sender, EventArgs e)
        {
            FORM_MyMarkedCase.TabControl1.SelectedIndex = listView1.SelectedItems[0].Index;
            this.Dispose();
        }

        private void listView1_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            if (e.Column != 0)
            {
                listView1.ListViewItemSorter = new ListViewItemsComparer(e.Column, SortType);
                listView1.Sort();
                SortType = !SortType;
            }
        }
    }
}
