using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System;
using System.Windows.Controls;
using System.Configuration;
using System.Windows.Navigation;
using System.IO;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Minesweeper
{
    internal class Highscores
    {
        private const string HighscoresFilePath = "highscores.json";

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
