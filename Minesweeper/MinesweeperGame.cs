using System;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using static System.Formats.Asn1.AsnWriter;

namespace Minesweeper
{
    internal class MinesweeperGame
    {
        private readonly Board board;
        private readonly Grid gridBoard = new();
        private readonly Label labelGame;
        private readonly Label labelScore;
        private readonly Timer timer;
        private Difficulty difficulty;

        public MinesweeperGame(int numRows, int numCols, int numBombs, Label labelGame, Label labelTimer, Label labelScore) {
            this.labelGame = labelGame;
            board = new(numRows, numCols, numBombs);
            timer = new(labelTimer);
            this.labelScore = labelScore;
            InitiateScore();
            CreateBoard();
        }

        public MinesweeperGame(Difficulty difficulty, Label labelGame, Label labelTimer, Label labelScore)
        {
            this.labelGame = labelGame;
            this.difficulty = difficulty;
            board = new(difficulty);
            timer = new(labelTimer);
            this.labelScore = labelScore;
            InitiateScore();
            CreateBoard();
        }

        private void CreateBoard()
        {
            gridBoard.HorizontalAlignment = HorizontalAlignment.Center;
            gridBoard.VerticalAlignment = VerticalAlignment.Center;
            gridBoard.RowDefinitions.Clear();
            gridBoard.ColumnDefinitions.Clear();
            gridBoard.Children.Clear();
            gridBoard.Margin = new Thickness(10);

            for (int i = 0; i < board.NumRows; i++)
            {
                gridBoard.RowDefinitions.Add(new RowDefinition());
            }
            for (int j = 0; j < board.NumCols; j++)
            {
                gridBoard.ColumnDefinitions.Add(new ColumnDefinition());
            }

            for (int i = 0; i < board.NumRows; i++)
            {
                for (int j = 0; j < board.NumCols; j++)
                {
                    board.BoardBox[i][j].button.PreviewMouseDown += ButtonBoxClick;
                    Grid.SetRow(board.BoardBox[i][j].button, i);
                    Grid.SetColumn(board.BoardBox[i][j].button, j);
                    gridBoard.Children.Add(board.BoardBox[i][j].button);
                }
            }

            UpdateLabel();
        }

        private void ButtonBoxClick(object sender, MouseButtonEventArgs e)
        {
            Button button = (Button)sender;
            int[] pos = (int[])button.Tag;
            Box buttonBox = board.BoardBox[pos[0]][pos[1]];

            if (e.ChangedButton == MouseButton.Left)
            {
                if (buttonBox.IsFlag)
                {
                    return;
                }
                if (board.firstMove)
                {
                    timer.StartTimer();
                }

                if (buttonBox.IsHidden)
                {
                    board.ShowBox(pos);
                }
                else
                {
                    if (buttonBox.IsZero)
                    {
                        return;
                    }
                    if (board.CountFlagsAround(pos) == int.Parse(buttonBox.Content))
                    {
                        board.ClearBoxesAround(pos);
                    }
                }

                if (board.GameOver)
                {
                    gridBoard.IsEnabled = false;
                }
            }
            else if (e.ChangedButton == MouseButton.Right)
            {
                if (!buttonBox.IsHidden || board.firstMove)
                {
                    return;
                }

                if (buttonBox.IsFlag)
                {
                    board.RemoveFlag(pos);
                }
                else
                {
                    board.PutFlag(pos);
                }
            }

            UpdateLabel();
        }

        private void UpdateLabel()
        {
            if (!board.GameOver)
            {
                labelGame.Content = Box.BOMB + " " + GetMinesLeft().ToString();
            }
            else
            {
                timer.StopTimer();
                gridBoard.IsEnabled = false;
                if (board.Win)
                {
                    int score = timer.GetTotalMiliseconds();
                    int highscore = Highscores.GetHighScore(difficulty);

                    if (score < highscore || highscore == 0)
                    {
                        Highscores.UpdateHighScore(score, difficulty);
                        labelScore.Content = "Highscore: " + Timer.ConvertMilisecondsToTime(score);
                    }

                    board.PutFlagsOnMinesLeft();
                    labelGame.Content = "You won!";
                }
                else
                {
                    labelGame.Content = "GameOver";
                }
            }
        }

        private int GetMinesLeft()
        {
            return board.NumBombs - board.NumFlags;
        }

        private void InitiateScore()
        {
            int score = Highscores.GetHighScore(difficulty);
            labelScore.Content = "Highscore: " + Timer.ConvertMilisecondsToTime(score);
        }

        public void Restart()
        {
            board.Restart();
            timer.RestartTimer();
            gridBoard.IsEnabled = true;
            UpdateLabel();
            InitiateScore();
        }

        public Grid GetGrid()
        {
            return gridBoard;
        }

        public void ChangeDifficulty(Difficulty difficulty)
        {
            if (this.difficulty != difficulty)
            {
                board.Win = false;
                this.difficulty = difficulty;
                board.ChangeDifficulty(difficulty);
                CreateBoard();
                Restart();
            }
        }
    }
}