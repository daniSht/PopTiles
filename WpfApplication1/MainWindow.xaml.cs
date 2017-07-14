using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

using System.Windows.Media;


namespace WpfApplication1
{

    public partial class MainWindow : Window
    {
        const int MaxX = 12;
        const int MaxY = 10;
        private SolidColorBrush[] colorsArr = new SolidColorBrush[] { Brushes.Salmon, (SolidColorBrush)(new BrushConverter().ConvertFrom("#b636e3")), Brushes.Gold, Brushes.MediumSpringGreen, Brushes.SlateGray }; //(Color)ColorConverter.ConvertFromString("#faffff");
        private PopField PopFieldInstance { get; set; }
        private Score ScoreInstance { get; set; }

        private class Score
        {            
            const int Mul = 5;
            private int[] targetScores = new int[] { 1000, 3000, 5000, 7500, 10000, 13000, 16000, 19000, 22000, 25000, 29000, 33000, 37000, 41000, 45000, 50000, 55000, 60000, 65000, 70000 };
            private int[] bonus = new int[] { 2000, 1920, 1820, 1680, 1500, 1280, 1020, 720, 380};
            public int Level { get; set; }
            public int CurrentScore { get; set; }
            public int TargetScore { get; set; }
            public List<int> LstBonus { get; set; }
            public Score()
            {
                Level = 0;
                CurrentScore = 0;
                TargetScore = targetScores[Level];
                LstBonus = new List<int>();
            }
            public string GetLevelString()
            {
                return "Level: " + (this.Level + 1);
            }
            public int GetNewScore(int n)
            {
                return n * n * Mul;
            }
            public void setNewScore(int n)
            {
                this.CurrentScore += this.GetNewScore(n);
            }
            public bool isGameOver()
            {
                return CurrentScore < TargetScore;
            }
            public bool setNextLevel()
            {
                if (targetScores.Count() <= Level + 1)
                {
                    return false;
                }
                Level++;
                TargetScore = targetScores[Level];
                return true;
            }
            public void setReceivedBonus(int remainingItems)
            {
                int currentBonus = 0;
                if(remainingItems < this.bonus.Count()) 
                {
                    currentBonus = this.bonus[remainingItems];
                }
                this.CurrentScore += currentBonus;
                LstBonus.Add(currentBonus);
            }
            public int GetLastLevelBonus() 
            {
                return this.LstBonus.LastOrDefault();
            }
            public int GetTotalBonus()
            {
                return this.LstBonus.Sum();
            }
        }

        private class ButtonCoords : IEquatable<ButtonCoords>
        {
            public int X { get; set; }
            public int Y { get; set; }
            public ButtonCoords(int x, int y)
            {
                this.X = x;
                this.Y = y;
            }

            public ButtonCoords(Button button)
            {
                int tagInt = (int)button.Tag;
                this.X = tagInt % 100;
                this.Y = tagInt / 100;
            }

            public static void setCoords(Button button, int x, int y)
            {
                button.Tag = y * 100 + x;
            }
            public override string ToString()
            {
                return this.X + ", " + this.Y;
            }
            public override bool Equals(object obj)
            {
                if (obj == null) return false;
                ButtonCoords objAsBC = obj as ButtonCoords;
                if (objAsBC == null) return false;
                else return Equals(objAsBC);
            }
            public override int GetHashCode()
            {
                return this.Y * 100 + this.X;
            }
            public bool Equals(ButtonCoords other)
            {
                if (other == null) return false;
                return (this.X.Equals(other.X) && this.Y.Equals(other.Y));
            }
        }

        private class PopItem
        {
            public int Color { get; set; }
            public bool IsVisible { get; set; }
            public bool IsSelected { get; set; }            
            public PopItem()
            {
                this.IsVisible = false;
                this.IsSelected = false;
            }
            public PopItem(int color)
            {
                this.Color = (int)color;
                this.IsVisible = true;
                this.IsSelected = false;
            }
        }

        private class PopField
        {
            public PopItem[,] FieldArray { get; set; }
            public List<ButtonCoords> LstSelectedItems { get; set; }
            public PopField()
            {
                var rnd = new Random();
                FieldArray = new PopItem[MaxX, MaxY];
                LstSelectedItems = new List<ButtonCoords>();
                for (int i = 0; i < MaxX; ++i)
                {
                    for (int j = 0; j < MaxY; ++j)
                    {
                        FieldArray[i, j] = new PopItem(rnd.Next(5));
                    }
                }
            }
            public PopItem getPopItem(int x, int y)
            {
                return FieldArray[x, y];
            }
            public PopItem getPopItem(ButtonCoords coords)
            {
                return FieldArray[coords.X, coords.Y];
            }
            private bool getAdjacentItems(ButtonCoords coords, bool isTestMode = false)
            {
                int x = coords.X;
                int y = coords.Y;
                int x1 = coords.X - 1;
                int x2 = coords.X + 1;
                int y1 = coords.Y - 1;
                int y2 = coords.Y + 1;
                if (x1 >= 0 && FieldArray[x1, y].IsVisible == true && FieldArray[x1, y].IsSelected != true && FieldArray[x1, y].Color == FieldArray[x, y].Color)
                {
                    if(isTestMode)
                    {
                        return true;
                    }
                    var tempCoord = new ButtonCoords(x1, y);
                    FieldArray[x1, y].IsSelected = true;
                    LstSelectedItems.Add(tempCoord);
                    this.getAdjacentItems(tempCoord);
                }
                if (x2 < MaxX && FieldArray[x2, y].IsVisible == true && FieldArray[x2, y].IsSelected != true && FieldArray[x2, y].Color == FieldArray[x, y].Color)
                {
                    if(isTestMode)
                    {
                        return true;
                    }
                    var tempCoord = new ButtonCoords(x2, y);
                    FieldArray[x2, y].IsSelected = true;
                    LstSelectedItems.Add(tempCoord);
                    this.getAdjacentItems(tempCoord);
                }
                if (y1 >= 0 && FieldArray[x, y1].IsVisible == true && FieldArray[x, y1].IsSelected != true && FieldArray[x, y1].Color == FieldArray[x, y].Color)
                {
                    if(isTestMode)
                    {
                        return true;
                    }
                    var tempCoord = new ButtonCoords(x, y1);
                    FieldArray[x, y1].IsSelected = true;
                    LstSelectedItems.Add(tempCoord);
                    this.getAdjacentItems(tempCoord);
                }
                if (y2 < MaxY && FieldArray[x, y2].IsVisible == true && FieldArray[x, y2].IsSelected != true && FieldArray[x, y2].Color == FieldArray[x, y].Color)
                {
                    if(isTestMode)
                    {
                        return true;
                    }
                    var tempCoord = new ButtonCoords(x, y2);
                    FieldArray[x, y2].IsSelected = true;
                    LstSelectedItems.Add(tempCoord);
                    this.getAdjacentItems(tempCoord);
                }
                return false;
            }
            public void setSelectedGroup(ButtonCoords coords)
            {
                for (int i = 0; i < MaxX; ++i)
                {
                    for (int j = 0; j < MaxY; ++j)
                    {
                        FieldArray[i, j].IsSelected = false;
                    }
                }
                FieldArray[coords.X, coords.Y].IsSelected = true;
                LstSelectedItems.Clear();
                LstSelectedItems.Add(coords);
                this.getAdjacentItems(coords);
                if (LstSelectedItems.Count == 1)
                {
                    LstSelectedItems.Clear();
                }
            }
            private void popItemsFall()
            {
                var lstDistinctX = LstSelectedItems.Select(c => c.X).Distinct().ToList();
                foreach (var x in lstDistinctX)
                {
                    int totalItemsInColumn = LstSelectedItems.Where(c => c.X == x).Count();
                    var lstEmptyY = LstSelectedItems.Where(c => c.X == x).Select(c => c.Y);
                    int startingY = lstEmptyY.Max(y => y) - 1;
                    int movePlaces = 1;
                    for (int j = startingY; j >= 0; j--)
                    {
                        if (lstEmptyY.Contains(j))
                        {
                            movePlaces++;
                            continue;
                        }
                        FieldArray[x, j + movePlaces] = FieldArray[x, j];
                        FieldArray[x, j] = new PopItem();
                    }
                }
            }
            private void rowsToTheLeft()
            {
                var lstDistinctX = LstSelectedItems.Select(c => c.X).Distinct().ToList();
                var lstEmptyX = new List<int>();
                foreach (var x in lstDistinctX)
                {
                    if (FieldArray[x, MaxY - 1].IsVisible == false)
                    {
                        lstEmptyX.Add(x);
                    }
                }
                if (lstEmptyX.Count > 0)
                {
                    int startingX = lstEmptyX.Min(x => x) + 1;
                    int movePlaces = 1;
                    for (int i = startingX; i < MaxX; i++)
                    {
                        if (lstEmptyX.Contains(i))
                        {
                            movePlaces++;
                            continue;
                        }
                        for (int j = 0; j < MaxY; j++)
                        {
                            FieldArray[i - movePlaces, j] = FieldArray[i, j];
                            FieldArray[i, j] = new PopItem();
                        }
                    }
                }
            }
            public bool isLevelEnded()
            {
                bool isThereAnyGroups = false;
                for (int i = 0; i < MaxX; ++i)
                {
                    for (int j = 0; j < MaxY; ++j)
                    {
                        if (FieldArray[i, j].IsVisible)
                        {
                            isThereAnyGroups = isThereAnyGroups || this.getAdjacentItems(new ButtonCoords(i, j), true);
                            if (isThereAnyGroups)
                            {
                                break;
                            }
                        }
                    }
                    if (isThereAnyGroups)
                    {
                        break;
                    }
                }
                return !isThereAnyGroups;
            }
            public void destroySelected()
            {
                foreach (ButtonCoords coord in LstSelectedItems)
                {
                    FieldArray[coord.X, coord.Y].IsVisible = false;
                }
                this.popItemsFall();
                this.rowsToTheLeft();

                LstSelectedItems.Clear();
            }
            public int getVisibleItemsCount()
            {
                List<PopItem> lstFieldItems = this.FieldArray.Cast<PopItem>().ToList();
                return lstFieldItems.Where(i => i.IsVisible == true).Count();
            }
            public int getTotalSelecetedItems()
            {
                return this.LstSelectedItems.Count();
            }

            public bool isGroupSelected(ButtonCoords coords)
            {
                return this.LstSelectedItems.Contains(coords);
            }
        }

        private void SyncWithDisplay()
        {
            foreach (Button button in MainGrid.Children.Cast<UIElement>().ToList())
            {
                var coords = new ButtonCoords((Button)button);
                var popItem = PopFieldInstance.getPopItem(coords);
                var isSelected = PopFieldInstance.LstSelectedItems.Contains(coords);
                button.Visibility = popItem.IsVisible ? Visibility.Visible : Visibility.Hidden;
                button.Background = colorsArr[popItem.Color];
                button.BorderThickness = new Thickness(1);
                button.BorderBrush = Brushes.Transparent;
                button.FontWeight = FontWeights.Bold;
                button.FontSize = 16;
                button.Content = isSelected ? "X" : "";
            }
            LblLevel.Content = ScoreInstance.GetLevelString();
            TxtCurrentScore.Text = ScoreInstance.CurrentScore.ToString();
            TxtTargetScore.Text = ScoreInstance.TargetScore.ToString();
            TxtBonus.Text = this.ScoreInstance.LstBonus.LastOrDefault().ToString();
            TxtTotalBonus.Text = this.ScoreInstance.LstBonus.Sum().ToString();
        }

        private void GenerateButtons()
        {
            for (int i = 0; i < MaxX; ++i)
            {
                for (int j = 0; j < MaxY; ++j)
                {
                    Button button = new Button();
                    button.SetValue(Grid.ColumnProperty, i);
                    button.SetValue(Grid.RowProperty, j);
                    ButtonCoords.setCoords(button, i, j);
                    button.Style = (Style)FindResource("NoHoverButton");
                    button.Click += ClickButton;
                    MainGrid.Children.Add(button);
                }
            }
        }
        
        private void GenerateNewRandomField()
        {
            this.PopFieldInstance = new PopField();
        }

        private void StartNewGame()
        {
            LblHighestScore.Visibility = Visibility.Hidden;
            LblGameOver.Visibility = Visibility.Hidden;
            BtnStartNewGame.Visibility = Visibility.Hidden;
            this.GenerateNewRandomField();
            ScoreInstance = new Score();
            this.SyncWithDisplay();    
        }

        public MainWindow()
        {
            InitializeComponent();
            this.GenerateButtons();
            this.StartNewGame();
        }

        public void ClickButton(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            var coords = new ButtonCoords(button);
            LblScoreToReceive.Content = null;
            if(this.PopFieldInstance.isGroupSelected(coords))
            {
                this.ScoreInstance.setNewScore(this.PopFieldInstance.getTotalSelecetedItems());
                this.PopFieldInstance.destroySelected();                
                if (this.PopFieldInstance.isLevelEnded())
                {
                    this.ScoreInstance.setReceivedBonus(this.PopFieldInstance.getVisibleItemsCount());
                    TxtBonus.Text = this.ScoreInstance.GetLastLevelBonus().ToString();
                    TxtTotalBonus.Text = this.ScoreInstance.GetTotalBonus().ToString();
                    if (ScoreInstance.isGameOver())
                    {
                        LblGameOver.Visibility = Visibility.Visible;
                        BtnStartNewGame.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        if (!ScoreInstance.setNextLevel())
                        {
                            LblHighestScore.Visibility = Visibility.Visible;
                            BtnStartNewGame.Visibility = Visibility.Visible;
                        }
                        this.GenerateNewRandomField();
                    }
                }
            }
            else 
            {
                this.PopFieldInstance.setSelectedGroup(coords);
                var scoreToReceive = this.PopFieldInstance.getTotalSelecetedItems();
                if (scoreToReceive != 0)
                {
                    LblScoreToReceive.Content = scoreToReceive.ToString() + " tiles reward score " + this.ScoreInstance.GetNewScore(scoreToReceive).ToString();
                }
            }
            
            this.SyncWithDisplay();
        }

        private void StartNewGame_Click(object sender, RoutedEventArgs e)
        {
            this.StartNewGame();
        }

        private void MnuItemAbout_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("This game has been created by Daniel Shterev. All rights resereved!®\n\n\nDedication:\n\nCreated for you, my dear Christina, with all the love, that you spark inside me! ♥", "About...", MessageBoxButton.OK);
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Are you sure you want to abandon the current game?", "About...", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                this.StartNewGame();
            }
        }
    }
}
