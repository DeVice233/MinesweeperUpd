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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Minesweeper
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Random random;
        private Frame frame;
        private Grid Field;
        List<FieldTale> ListTales;
        private int bombCount;
        private int fieldSize;
        private const int fieldUnitSize = 40;
        private bool firstClick;

        public MainWindow()
        {
            InitializeComponent();
            GenerateMap();
        }
        public void GenerateMap()
        {
            frame = new Frame();
            Field = new Grid();
            ListTales = new List<FieldTale>(this.bombCount);
            fieldSize = 10;
            bombCount = 20;

            this.frame.Width = fieldUnitSize * fieldSize;
            this.frame.Height = fieldUnitSize * fieldSize;
            this.MinWidth = this.frame.Width + 100;
            this.MinHeight = this.frame.Height + 200;
            CreateGrid();
            PrepareField();
        }
        private void CreateGrid()
        {
            for (int i = 0; i < this.fieldSize; i++)
            {
                Field.ColumnDefinitions.Add(new ColumnDefinition());
                Field.RowDefinitions.Add(new RowDefinition());
            }
            Border border = new Border
            {
                BorderThickness = new Thickness(2),
                BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#303030")),
                Child = Field
            };

            this.frame.Content = border;
            this.frame.Background = new SolidColorBrush(Colors.White);
            Grid.SetRow(this.frame, 1);
            Grid.SetColumn(this.frame, 0);
            Grid.SetColumnSpan(this.frame, 3);
            mainGrid.Children.Add(this.frame);

        }
        private void PrepareField()
        {
            this.firstClick = true;
            for (int i = 0; i < this.fieldSize; i++)
            {
                for (int j = 0; j < this.fieldSize; j++)
                {
                    FieldTale tale = new FieldTale(false, 0, i, j);
                    ListTales.Add(tale);
                    tale.Style = FindResource("FieldTaleStyle") as Style;
                    Grid.SetRow(tale, i);
                    Grid.SetColumn(tale, j);
                    Field.Children.Add(tale);

                    tale.Click += FieldUnit_click;
                    tale.MouseRightButtonUp += FieldTale_right_click;
                }
            }
        }

        private void InitializeField(int indexOfFirstFieldTale)
        {
            this.random = new Random();
            int fieldTaleCount = (int)Math.Pow(this.fieldSize, 2);
            List<int> ListBombs = new List<int>(this.fieldSize);
            ListBombs.Add(indexOfFirstFieldTale);
            int rng;

            for (int k = 0; k < this.bombCount; k++)
            {
                rng = random.Next(0, fieldTaleCount - 1);
                if (ListBombs.Contains(rng))
                {
                    k--;
                    continue;
                }
                else
                {
                    ListBombs.Add(rng);
                    ListTales[rng].Bomb = true;
                    ListTales[rng].AroundBombs = -1;
                    ListTales[rng].IsOpened = false;
                    int row = Grid.GetRow(ListTales[rng]);
                    int colloumn = Grid.GetColumn(ListTales[rng]);

                    for (int i = -1; i < 2; i++)
                    {
                        for (int j = -1; j < 2; j++)
                        {
                            int r = row + i;
                            int c = colloumn + j;

                            if (r < 0 || r > fieldSize - 1 || c < 0 || c > fieldSize - 1)
                                continue;

                            FieldTale tale = FieldTale.GetTale(ListTales, r, c);

                            if (tale.Bomb || (i == 0 && j == 0))
                                continue;

                            tale.AroundBombs++;
                        }
                    }
                }
            }
        }

        private void FieldUnit_click(object sender, EventArgs e)
        {
            FieldTale tale = sender as FieldTale;
            if (firstClick)
            {
                firstClick = false;
                int indexOfFirstFieldTale = Field.Children.IndexOf(tale);
                InitializeField(indexOfFirstFieldTale);
            }
            if (tale.Flag)
                return;

            OpenField(tale);

            if (AllOpened())
                GameWon();
        }

        private void FieldTale_right_click(object sender, System.EventArgs e)
        {
            FieldTale tale = sender as FieldTale;

            if (tale.Flag)
            {
                tale.Flag = false;
                tale.Content = "";
                return;
            }

            tale.Flag = true;

            tale.Content = new Image
            {
                Source = new BitmapImage(new Uri("Resources/flag.png", UriKind.Relative)),
                VerticalAlignment = VerticalAlignment.Center
            };
        }

      
        private void OpenField(FieldTale tale)
        {
            tale.IsOpened = true;

            int row = Grid.GetRow(tale);
            int column = Grid.GetColumn(tale);

            TextBlock textblock = new TextBlock
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };

            if (tale.Bomb)
            {
                GameOver(tale);
                return;
            }
            else
            {
                textblock.Text = tale.AroundBombs.ToString();
                textblock.FontSize = 30;
            }
            SetTextblockStyle(textblock);
            Field.Children.Remove(tale);
            Border border = new Border
            {
                BorderThickness = new Thickness(1),
                BorderBrush = new SolidColorBrush(Colors.LightGray),
                Child = textblock
            };
            Grid.SetRow(border, row);
            Grid.SetColumn(border, column);
            Field.Children.Add(border);

            if (tale.AroundBombs == 0)
            {
                for (int i = -1; i < 2; i++)
                {
                    for (int j = -1; j < 2; j++)
                    {
                        int r = row + i;
                        int c = column + j;

                        if (r < 0 || r > fieldSize - 1 || c < 0 || c > fieldSize - 1 ||
                            (i == 0 && j == 0))
                            continue;

                        FieldTale opentale = FieldTale.GetTale(ListTales, r, c);

                        if (opentale.IsOpened || opentale.Flag)
                            continue;
                        else
                            OpenField(opentale);
                    }
                }
            }
            else
                return;
        }

        private void GameOver(FieldTale tale = null)
        {
           
           
                tale.IsEnabled = false;
               
                tale.Content = new Image
                {
                    Source = new BitmapImage(new Uri("Resources/mine_blown.png", UriKind.Relative)),
                    VerticalAlignment = VerticalAlignment.Center
                };
          

            foreach (FieldTale ftale in ListTales)
            {
                ftale.IsEnabled = false;
                if (ftale.Bomb)
                {
                    if (ftale == tale || ftale.Flag)
                        continue;

                    ftale.Content = new Image
                    {
                        Source = new BitmapImage(new Uri("Resources/mine.png", UriKind.Relative)),
                        VerticalAlignment = VerticalAlignment.Center
                    };
                }

                else
                {
                    if (ftale.Flag)
                    {
                        ftale.Content = new Image
                        {
                            Source = new BitmapImage(new Uri("Resources/wrong_mine.png", UriKind.Relative)),
                            VerticalAlignment = VerticalAlignment.Center
                        };
                    }
                }
            }
        }

        private bool AllOpened()
        {
            foreach (FieldTale tale in ListTales)
            {
                if (tale.Bomb)
                    continue;
                if (!tale.IsOpened)
                    return false;
            }
            return true;
        }

        private void GameWon()
        {
            foreach (FieldTale tale in ListTales)
            {
                tale.IsEnabled = false;
                if (tale.Bomb && !tale.Flag)
                {
                    tale.Flag = true;
                   
                    tale.Content = new Image
                    {
                        Source = new BitmapImage(new Uri("Resources/flag.png", UriKind.Relative)),
                        VerticalAlignment = VerticalAlignment.Center
                    };
                }
            }
        }

        private void SetTextblockStyle(TextBlock tb)
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
