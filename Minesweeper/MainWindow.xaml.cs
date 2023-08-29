using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Minesweeper
{
    public enum Difficulty
    {
        Easy = 1,
        Medium = 2,
        Hard = 3
    }

    public partial class MainWindow : Window
    {
        private MinesweeperGame game;

        public MainWindow()
        {
            InitializeComponent();

            game = new(Difficulty.Hard, labelNumBombs, labelTimer, labelHighscore);
            GridContent.Children.Add(game.GetGrid());
        }

        private void buttonRestart_Click(object sender, RoutedEventArgs e)
        {
            game.Restart();
        }

        private void comboBoxDifficulty_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox difficulty = (ComboBox)sender;
            game?.ChangeDifficulty((Difficulty)difficulty.SelectedIndex + 1);
        }

        public void MouseEnterRestartButton(object sender, RoutedEventArgs e)
        {
            Border border = (Border)sender;
            border.Background = Brushes.LightBlue;
        }

        public void MouseLeaveRestartButton(object sender, RoutedEventArgs e)
        { 
            Border border = (Border)sender;
            border.Background = Brushes.White;
        }
    }
}
