using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Collections;

namespace SupportLogSheet
{
    public class ListViewItemsComparer : IComparer
    {
        private int col;
        private bool m_Asc;
        public ListViewItemsComparer()
        {
        }
        public ListViewItemsComparer(int column, bool p_Asc)
        {
            col = column;
            m_Asc = p_Asc;
        }
        public int Compare(object x, object y)
        {
            string compStrX = ((ListViewItem)x).SubItems[col].Text.Trim(' ');
            string compStrY = ((ListViewItem)y).SubItems[col].Text.Trim(' ');
            if (((ListViewItem)x).BackColor == LV_OP.Color_EditItem || ((ListViewItem)x).BackColor == LV_OP.Color_NewItem || ((ListViewItem)y).BackColor == LV_OP.Color_EditItem || ((ListViewItem)y).BackColor == LV_OP.Color_NewItem)
            {
                return -1;
            }

            if (m_Asc)
            {
                return myStrCmp(compStrX.Trim(' '), compStrY.Trim(' '));
            }
            else
            {
                return myStrCmp(compStrY.Trim(' '), compStrX.Trim(' '));
            }
        }
        private int myStrCmp(string strA, string strB)
        {
            try
            {
                int A = Int32.Parse(strA);
                int B = Int32.Parse(strB);
                if (A <= B)
                {
                    return 0;
                }
                else
                {
                    return 1;
                }
            }
            catch
            {
                if (String.Compare(strA, strB) != 0)
                {
                    return String.Compare(strA, strB);
                }
                else
                {
                    return 0;
                }
            }
        }
    }
}
