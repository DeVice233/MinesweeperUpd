using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Minesweeper
{
    /// <summary>
    /// Логика взаимодействия для StartWindow.xaml
    /// </summary>
    public partial class StartWindow : Window
    {
        MainWindow MainWindow = new MainWindow();
        private string dificulty = "Easy";
        public StartWindow()
        {
            InitializeComponent();

        }

        private void Radiobutton_dificulty_change(object sender, EventArgs e)
        {
            RadioButton rdb = sender as RadioButton;
            dificulty = rdb.Content.ToString();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            new MainWindow(dificulty).Show();
            Close();
        }
    }
}
