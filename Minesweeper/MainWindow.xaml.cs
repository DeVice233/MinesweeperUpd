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
        TextBlockForegroundStyle TextBlockForegroundStyle = new TextBlockForegroundStyle();
        private DispatcherTimer dTimer;
        private Random random;
        private Frame frame;
        private Grid Field;
        List<FieldTale> ListTales;
        private int bombCount;
        private int fieldSize;
        private const int fieldUnitSize = 40;
        private bool firstClick;
        private int mineCounter;
        private int timer;

        public MainWindow()
        {
            InitializeComponent();
           
        }
        public MainWindow(string dificulty) : this()
        {
            InitializeComponent();
            GenerateMap(dificulty);
        }
        public void GenerateMap(string dificulty)
        {
            frame = new Frame();
            Field = new Grid();
            ListTales = new List<FieldTale>(this.bombCount);
            switch (dificulty)
            {
                case "Easy":
                    fieldSize = 9;
                    bombCount = 10;
                    break;
                case "Medium":
                    fieldSize = 12;
                    bombCount = 20;
                    break;
                case "Hard":
                    fieldSize = 15;
                    bombCount = 40;
                    break;
                default:
                    break;
            }
            dTimer = new DispatcherTimer();
            dTimer.Tick += DispatcherTimer_Tick;
            dTimer.Interval = new TimeSpan(0, 0, 1);
            timer = 0;
            mineCounter = bombCount;
            txtMineCounter.Text = mineCounter.ToString();
            this.frame.Width = fieldUnitSize * fieldSize;
            this.frame.Height = fieldUnitSize * fieldSize;
            this.MinWidth = this.frame.Width + 50;
            this.MinHeight = this.frame.Height + 150;
            CreateGrid();
           
        }

        private void DispatcherTimer_Tick(object sender, EventArgs e)
        {
            timer++;
            txtTimer.Text = timer.ToString();
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
            mainGrid.Children.Add(this.frame);
            PrepareField();
        }
        private void PrepareField()
        {
            firstClick = true;
            for (int i = 0; i < fieldSize; i++)
            {
                for (int j = 0; j < fieldSize; j++)
                {
                    FieldTale tale = new FieldTale(false, 0, i, j);
                    ListTales.Add(tale);
                    tale.Style = FindResource("FieldTaleStyle") as Style;
                    Grid.SetRow(tale, i);
                    Grid.SetColumn(tale, j);
                    Field.Children.Add(tale);
                    tale.Click += FieldTale_click;
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

        private void FieldTale_click(object sender, EventArgs e)
        {
            FieldTale tale = sender as FieldTale;
            if (firstClick)
            {
                firstClick = false;
                dTimer.Start();
                int indexOfFirstFieldTale = Field.Children.IndexOf(tale);
                InitializeField(indexOfFirstFieldTale);
            }
            if (tale.Flag) return;
            OpenField(tale);
            if (AllOpened()) GameWon();
        }

        private void FieldTale_right_click(object sender, EventArgs e)
        {
            FieldTale tale = sender as FieldTale;
            if (tale.Flag)
            {
                tale.Flag = false;
                tale.Content = "";
                mineCounter++;
                txtMineCounter.Text = mineCounter.ToString();
                return;
            }
            if (mineCounter == 0)
                return;
            tale.Flag = true;
            tale.Content = new Image
            {
                Source = new BitmapImage(new Uri("Resources/flag.png", UriKind.Relative)),
                VerticalAlignment = VerticalAlignment.Center
            };
            mineCounter--;
            txtMineCounter.Text = mineCounter.ToString();
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
            TextBlockForegroundStyle.SetTextblockStyle(textblock);
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
            dTimer.Stop();
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
            MessageBox.Show("Вы проиграли");
            new StartWindow().Show();
            Close();
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
            dTimer.Stop();
            foreach (FieldTale tale in ListTales)
            {
                tale.IsEnabled = false;
                if (tale.Bomb && !tale.Flag)
                {
                    tale.Flag = true;
                    mineCounter--;
                    txtMineCounter.Text = mineCounter.ToString();
                    tale.Content = new Image
                    {
                        Source = new BitmapImage(new Uri("Resources/flag.png", UriKind.Relative)),
                        VerticalAlignment = VerticalAlignment.Center
                    };
                }
            }
            MessageBox.Show($"Вы выиграли\nВаше вермя - {timer} сек");
            new StartWindow().Show();
            Close();
        }
    }
}
