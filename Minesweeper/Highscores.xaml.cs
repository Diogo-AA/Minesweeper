using Newtonsoft.Json;
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
using System.IO;
using System.Data.SQLite;
using System.Data;

namespace Minesweeper
{
    /// <summary>
    /// Lógica de interacción para Highscores.xaml
    /// </summary>
    public partial class Highscores : Window
    {
        private readonly SQLiteConnection conn;

        public Highscores(int score, Difficulty difficulty)
        {
            InitializeComponent();

            conn = new SQLiteConnection("Data Source=highscores.db;New=True;");
            conn.Open();

            CreateTable();

            int bestScore = GetHighScore(difficulty);
            if (score < bestScore || bestScore == 0)
            {
                UpdateHighScore(score, difficulty);
                bestScore = score;
            }

            labelBestScore.Content = "Your best time was: " + Timer.ConvertMilisecondsToTime(bestScore);
            labelActualScore.Content = "Your time was: " + Timer.ConvertMilisecondsToTime(score);
            conn.Close();
        }

        private void CreateTable()
        {
            var command = conn.CreateCommand();
            command.CommandText =
                @"
                CREATE TABLE IF NOT EXISTS highscores (
                    time int,
                    difficulty int
                );
                INSERT OR IGNORE INTO highscores (time, difficulty) VALUES (0, 1), (0, 2), (0, 3);
                ";
            command.ExecuteNonQuery();
        }

        private void UpdateHighScore(int score, Difficulty difficulty)
        {
            var command = conn.CreateCommand();
            command.CommandText =
                @"
                UPDATE highscores
                SET time = $score
                WHERE difficulty = $difficulty
                ";
            command.Parameters.AddWithValue("$difficulty", difficulty);
            command.Parameters.AddWithValue("$score", score);

            command.ExecuteNonQuery();
        }

        private int GetHighScore(Difficulty difficulty)
        {
            var command = conn.CreateCommand();
            command.CommandText =
                @"
                SELECT time
                FROM highscores
                WHERE difficulty = $difficulty
                ";
            command.Parameters.AddWithValue("$difficulty", difficulty);

            var reader = command.ExecuteReader();

            reader.Read();
            int score = reader.GetInt32(0);

            return score;
        }
    }
}
