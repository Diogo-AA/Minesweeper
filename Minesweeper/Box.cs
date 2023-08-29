using System;
using System.Windows.Controls;
using System.Windows.Media;

namespace Minesweeper
{
    internal class Box
    {
        public static string FLAG { get; } = "🚩";
        public static string BOMB { get; } = "💣";

        public Button button = new();
        public string Content { get; set; }
        public bool IsBomb { get; set; }
        public bool IsZero { get; set; }
        public bool IsFlag { get; set; } = false;
        public bool IsHidden { get; set; }

        public Box(int row, int col)
        {
            IsHidden = true;
            Content = String.Empty;
            InitializeButton(row, col);
        }

        private void InitializeButton(int row, int col)
        {
            int[] position = { row, col };
            button.Tag = position;
            button.Width = 40;
            button.Height = 40;
            button.FontSize = 20;
            button.Content = String.Empty;
        }

        public void Create(string content)
        {
            button.Foreground = Brushes.Black;
            Content = content;
            IsZero = content == String.Empty;
            IsBomb = content == BOMB;
        }

        public void Reset()
        {
            Content = String.Empty;
            button.Content = String.Empty;
            button.Background = new SolidColorBrush(Color.FromRgb(221, 221, 221));
            button.Foreground = Brushes.Black;
            IsHidden = true;
            IsFlag = false;
        }

        public void Show()
        {
            IsHidden = false;
            button.Background = Brushes.White;
            button.Content = Content.ToString();

            if (!IsFlag && !IsBomb && !IsZero)
            {
                button.Foreground = GetColorFromNumBombs(int.Parse(Content));
            }
        }

        private static SolidColorBrush GetColorFromNumBombs(int numBombs)
        {
            return numBombs switch
            {
                1 => Brushes.Blue,
                2 => Brushes.Green,
                3 => Brushes.Red,
                4 => Brushes.Orange,
                5 => Brushes.Orchid,
                6 => Brushes.Purple,
                7 => Brushes.Yellow,
                _ => Brushes.Black,
            };
        }

        public void PutFlag()
        {
            IsFlag = true;
            button.Content = FLAG;
        }

        public void RemoveFlag()
        {
            IsFlag = false;
            button.Content = String.Empty;
        }
    }
}