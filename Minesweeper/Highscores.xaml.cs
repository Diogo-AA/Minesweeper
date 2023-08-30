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

namespace Minesweeper
{
    /// <summary>
    /// Lógica de interacción para Highscores.xaml
    /// </summary>
    public partial class Highscores : Window
    {
        private const string HighscoresFilePath = "highscores.json";

        public Highscores(int score, Difficulty difficulty)
        {
            InitializeComponent();

            if (score < GetHighScore(difficulty) || GetHighScore(difficulty) == 0)
            {
                UpdateHighScore(score, difficulty);
            }

            labelBestScore.Content = "Your best time was: " + Timer.ConvertMilisecondsToTime(GetHighScore(difficulty));
            labelActualScore.Content = "Your time was: " + Timer.ConvertMilisecondsToTime(score);
        }

        public static void UpdateHighScore(int score, Difficulty difficulty)
        {
            var highscores = LoadHighscores();
            highscores[difficulty.ToString()] = score;
            SaveHighscores(highscores);
        }

        public static int GetHighScore(Difficulty difficulty)
        {
            var highscores = LoadHighscores();
            return highscores.TryGetValue(difficulty.ToString(), out int score) ? score : 0;
        }

        private static void SaveHighscores(Dictionary<string, int> highscores)
        {
            string json = JsonConvert.SerializeObject(highscores, Formatting.Indented);
            File.WriteAllText(HighscoresFilePath, json);
        }

        private static Dictionary<string, int> LoadHighscores()
        {
            if (File.Exists(HighscoresFilePath))
            {
                string json = File.ReadAllText(HighscoresFilePath);
                return JsonConvert.DeserializeObject<Dictionary<string, int>>(json);
            }
            return new Dictionary<string, int>();
        }
    }
}
