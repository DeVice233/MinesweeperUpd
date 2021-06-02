using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Minesweeper
{
    class FieldTale : Button
    {
        public FieldTale(bool bomb, int aroundBombs, int i, int j)
        {
            Bomb = bomb;
            AroundBombs = aroundBombs;
            Flag = false;
            IsOpened = false;
            Row = i;
            Collumn = j;
        }

        public bool Bomb { get; set; }
        public bool Flag { get; set; }
        public int AroundBombs { get; set; }
        public bool IsOpened { get; set; }
        public int Row { get; }
        public int Collumn { get; }

        public static FieldTale GetTale(List<FieldTale> ListTales, int i, int j)
        {
            foreach (FieldTale fu in ListTales)
            {
                if (fu.Row == i && fu.Collumn == j)
                    return fu;
            }
            return null;
        }
    }
}
