using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SupportLogSheet
{
    class DGV_Op
    {
        public static void initialDGV(DataGridViewNF dgv, string[] keys,List<message> notices)
        {
            for (int i = 0; i < notices.Count; i++)
            {
                dgv.Rows.Insert(0, notices[i].getValuesFromKeys(keys));
            }
        }
    }
}
