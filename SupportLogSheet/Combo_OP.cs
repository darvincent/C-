// tool comboBox/comboBox 操作类
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace SupportLogSheet
{
    class Combo_OP
    {
        public delegate void Del_updateComboBoxes(List<ComboBox> comboBoxList, string content, string splitSymbol);
        public delegate void Del_updateComboBox(ComboBox combo, string content, string splitSymbol);
        public delegate void Del_updateComboBoxNoSplit(ComboBox combo, object[] content);

        public static void initialComboBox(ComboBox combo, object[] contents)
        {
            combo.BeginUpdate();
            combo.Text = "";
            combo.Items.Clear();
            if (contents != null)
            {
                combo.Items.AddRange(contents);
            }
            combo.EndUpdate();
        }

        public static void initialComboBox(ComboBox combo, string content, string splitSymbol)
        {
            combo.BeginUpdate();
            combo.Text = "";
            combo.Items.Clear();
            if (!content.Trim(' ').Equals(""))
            {
                combo.Items.AddRange(Regex.Split(content, splitSymbol));
            }
            combo.EndUpdate();
        }

        public static void updateToolComboBox(ToolStripComboBox combo, string content, string splitSymbol)
        {
            combo.BeginUpdate();
            combo.Text = "";
            combo.Items.Clear();
            if (!content.Trim(' ').Equals(""))
            {
                combo.Items.AddRange(Regex.Split(content, splitSymbol));
            }
            combo.EndUpdate();
        }

        public static void initialComboBox(ComboBox combo, List<string> content)
        {
            combo.BeginUpdate();
            combo.Text = "";
            combo.Items.Clear();
            if (content != null)
            {
                combo.Items.AddRange(content.ToArray());
            }
            combo.EndUpdate();
        }

        public static void initialToolComboBox(ToolStripComboBox combo, List<string> content)
        {
            combo.BeginUpdate();
            combo.Text = "";
            combo.Items.Clear();
            if (content != null)
            {
                combo.Items.AddRange(content.ToArray());
            }
            combo.EndUpdate();
        }

        public static void initialToolComboBox(ToolStripComboBox combo, string content, string splitSymbol)
        {
            combo.BeginUpdate();
            combo.Text = "";
            combo.Items.Clear();
            if (!content.Trim(' ').Equals(""))
            {
                combo.Items.AddRange(Regex.Split(content, splitSymbol));
            }
            combo.EndUpdate();
        }

    }
}
