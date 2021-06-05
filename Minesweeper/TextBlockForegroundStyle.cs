using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;

namespace Minesweeper
{
    public class TextBlockForegroundStyle
    {

        public void SetTextblockStyle(TextBlock tb)
        {
            switch (tb.Text)
            {
                case "0":
                    tb.Text = "";
                    break;
                case "1":
                    tb.Foreground = Brushes.Blue;
                    break;
                case "2":
                    tb.Foreground = Brushes.Green;
                    break;
                case "3":
                    tb.Foreground = Brushes.Red;
                    break;
                case "4":
                    tb.Foreground = Brushes.DarkBlue;
                    break;
                case "5":
                    tb.Foreground = Brushes.Brown;
                    break;
                case "6":
                    tb.Foreground = Brushes.Aqua;
                    break;
                case "7":
                    tb.Foreground = Brushes.Black;
                    break;
                case "8":
                    tb.Foreground = Brushes.LightGray;
                    break;
                default:
                    break;
            }
        }
    }
}
