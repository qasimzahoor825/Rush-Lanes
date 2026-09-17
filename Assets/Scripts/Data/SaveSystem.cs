using System;
using UnityEngine;

namespace RushLanes
{
    [Serializable]
    public class SaveData
    {
        public int highScore;
        public int bestDistance;
        public int totalCoins;
        public int selectedCharacter;
        public bool soundOn = true;
        public bool tutorialSeen;
        public bool[] unlocked = { true, false, false };
        public int[] missionProgress = new int[3];
        public bool[] missionComplete = new bool[3];
    }

    public static class SaveSystem
    {
        const string Key = "RushLanes.Save.v1";
        public static SaveData Data { get; private set; } = new SaveData();

        public static void Load()
        {
            if (!PlayerPrefs.HasKey(Key))
            {
                Data = new SaveData();
                Save();
                return;
            }

            try
            {
                Data = JsonUtility.FromJson<SaveData>(PlayerPrefs.GetString(Key)) ?? new SaveData();
                if (Data.unlocked == null || Data.unlocked.Length < 3)
                    Data.unlocked = new[] { true, false, false };
                if (Data.missionProgress == null || Data.missionProgress.Length < 3)
                    Data.missionProgress = new int[3];
                if (Data.missionComplete == null || Data.missionComplete.Length < 3)
                    Data.missionComplete = new bool[3];
            }
            catch
            {
                Data = new SaveData();
            }
        }

        public static void Save()
        {
            PlayerPrefs.SetString(Key, JsonUtility.ToJson(Data));
            PlayerPrefs.Save();
        }

        public static void SubmitRun(int score, int distance, int coins)
        {
            Data.totalCoins += coins;
            if (score > Data.highScore)
                Data.highScore = score;
            if (distance > Data.bestDistance)
                Data.bestDistance = distance;
            Save();
        }
    }
}
