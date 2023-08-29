using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Documents;

namespace Minesweeper
{   
    internal class Board
    {
        public int NumRows { get; set; }
        public int NumCols { get; set; }
        public int NumBombs { get; set; }
        public int NumFlags { get; set; } = 0;

        private int[][]? bombs;
        public Box[][] BoardBox { get; set; }
        public bool GameOver { get; set; } = false;
        public bool Win { get; set; } = false;
        public bool firstMove = true;
        private List<int[]> allPositions = new();

        public Board(int numRows, int numCols, int numBombs)
        {
            NumRows = numRows;
            NumCols = numCols;
            NumBombs = numBombs;
            InitialCreate();
        }

        public Board(Difficulty difficulty)
        {
            ChangeDifficulty(difficulty);
        }

        public void ChangeDifficulty(Difficulty difficulty)
        {
            GetDifficulty(difficulty);
            InitialCreate();
        }

        private void GetDifficulty(Difficulty difficulty)
        {
            switch (difficulty)
            {
                case Difficulty.Easy:
                    NumRows = 10; NumCols = 10; NumBombs = 15;
                    break;
                case Difficulty.Medium:
                    NumRows = 12; NumCols = 20; NumBombs = 50;
                    break;
                case Difficulty.Hard:
                    NumRows = 16; NumCols = 32; NumBombs = 100;
                    break;
                default:
                    break;
            }
        }

        private void InitialCreate()
        {
            CreateAllPositions();

            BoardBox = new Box[NumRows][];
            for (int i = 0; i < NumRows; i++)
            {
                BoardBox[i] = new Box[NumCols];
                for (int j = 0; j < NumCols; j++)
                {
                    BoardBox[i][j] = new(i, j);
                }
            }
        }

        private void CreateAllPositions()
        {
            allPositions.Clear();
            for (int i = 0; i < NumRows; i++)
            {
                for (int j = 0; j < NumCols; j++)
                {
                    allPositions.Add(new int[] { i, j });
                }
            }
        }

        public void PutFlagsOnMinesLeft()
        {
            Box[] bombsLeft = Array.FindAll(BoardBox, (Box[] row) => Array.Exists(row, (Box box) => box.IsBomb && !box.IsFlag))
                                    .SelectMany(row => Array.FindAll(row, (Box box) => box.IsBomb && !box.IsFlag))
                                    .ToArray();

            Array.ForEach(bombsLeft, (Box bomb) => bomb.PutFlag());
        }

        public void Restart()
        {
            GameOver = false;
            Win = false;
            firstMove = true;
            NumFlags = 0;
            bombs = null;
            RestartBoxes();
        }

        private void RestartBoxes()
        {
            for (int i = 0; i < NumRows; i++)
            {
                for (int j = 0; j < NumCols; j++)
                {
                    BoardBox[i][j].Reset();
                }
            }
        }

        public void PutFlag(int[] pos)
        {
            BoardBox[pos[0]][pos[1]].PutFlag();
            NumFlags++;
        }

        public void RemoveFlag(int[] pos)
        {
            BoardBox[pos[0]][pos[1]].RemoveFlag();
            NumFlags--;
        }

        public void ShowBox(int[] position)
        {
            if (firstMove)
            {
                CreateBoard(position);
                firstMove = false;
            }

            Box box = BoardBox[position[0]][position[1]];

            if (!box.IsHidden)
            {
                return;
            }

            box.Show();

            if (box.IsBomb)
            {
                GameOver = true;
                ShowAllBombs();
                return;
            }
            else if (box.IsZero)
            {
                ClearZeros(position);
            }

            CheckWin();
        }

        private void CreateBoard(int[] initialPos)
        {
            int[][] bombs = GenerateBombs(initialPos);

            for (int i = 0; i < NumRows; i++)
            {
                for (int j = 0; j < NumCols; j++)
                {
                    int[] position = { i, j };
                    if (Array.Exists(bombs, element => element.SequenceEqual(position)))
                    {
                        BoardBox[i][j].Create(Box.BOMB);
                    }
                    else
                    {
                        int numBombsNearby = CountBombsNearby(position);
                        BoardBox[i][j].Create(numBombsNearby != 0 ? numBombsNearby.ToString() : String.Empty);
                    }
                }
            }
        }

        private int[][] GenerateBombs(int[] initialPos)
        {
            Random rnd = new();

            allPositions = allPositions.Where(pos => !IsNearby(initialPos, pos)).ToList();

            bombs = allPositions.OrderBy(_ => rnd.Next()).Take(NumBombs).ToArray();

            return bombs;
        }

        private static bool IsNearby(int[] initialPos, int[] pos)
        {
            return Math.Abs(initialPos[0] - pos[0]) < 2 && Math.Abs(initialPos[1] - pos[1]) < 2;
        }

        private int CountBombsNearby(int[] initialPosition)
        {
            int numBombs = 0;

            for (int i = initialPosition[0] - 1; i <= initialPosition[0] + 1; i++)
            {
                for (int j = initialPosition[1] - 1; j <= initialPosition[1] + 1; j++)
                {
                    int[] position = { i, j };
                    if (Array.Exists(bombs, (int[] element) => element.SequenceEqual(position)))
                    {
                        numBombs++;
                    }
                }
            }

            return numBombs;
        }

        private void ShowAllBombs()
        {
            for (int i = 0; i < bombs.Length; i++)
            {
                BoardBox[bombs[i][0]][bombs[i][1]].Show();
            }
        }

        private void ClearZeros(int[] initialPosition)
        {
            for (int i = initialPosition[0] - 1; i <= initialPosition[0] + 1; i++)
            {
                for (int j = initialPosition[1] - 1; j <= initialPosition[1] + 1; j++)
                {
                    int[] position = { i, j };
                    if (!IsValidPosition(position) || !BoardBox[i][j].IsHidden || (i == initialPosition[0] && j == initialPosition[1]))
                    {
                        continue;
                    }

                    if (!BoardBox[i][j].IsFlag)
                    {
                        BoardBox[i][j].Show();
                    }
                    if (BoardBox[i][j].IsZero) {
                        ClearZeros(position);
                    }
                }
            }
        }

        private void CheckWin()
        {
            GameOver = Win =
                !Array.Exists(BoardBox, (Box[] element) => Array.Exists(element, (Box box) => box.IsHidden && !box.IsBomb));
        }

        public int CountFlagsAround(int[] position)
        {
            int numFlags = 0;

            for (int i = position[0] - 1; i <= position[0] + 1; i++)
            {
                for (int j = position[1] - 1; j <= position[1] + 1; j++)
                {
                    if (!IsValidPosition(new int[] { i, j }) || (i == position[0] && j == position[1]))
                    {
                        continue;
                    }

                    if (BoardBox[i][j].IsFlag)
                    {
                        numFlags++;
                    }
                }
            }

            return numFlags;
        }

        public void ClearBoxesAround(int[] position)
        {
            for (int i = position[0] - 1; i <= position[0] + 1; i++)
            {
                for (int j = position[1] - 1; j <= position[1] + 1; j++)
                {
                    if (!IsValidPosition(new int[] { i, j }) || BoardBox[i][j].IsFlag || (i == position[0] && j == position[1]))
                    {
                        continue;
                    }

                    ShowBox(new int[] { i, j });
                }
            }
        }

        public bool IsValidPosition(int[] position)
        {
            return !(position[0] < 0 || position[0] >= NumRows || position[1] < 0 || position[1] >= NumCols);
        }
    }
}