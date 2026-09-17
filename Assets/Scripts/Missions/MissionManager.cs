using UnityEngine;

namespace RushLanes
{
    public readonly struct MissionDef
    {
        public readonly string Title;
        public readonly string Hint;
        public readonly int Target;

        public MissionDef(string title, string hint, int target)
        {
            Title = title;
            Hint = hint;
            Target = target;
        }
    }

    public class MissionManager : MonoBehaviour
    {
        public static readonly MissionDef[] Definitions =
        {
            new MissionDef("Coin Rush", "Collect 40 coins in one run", 40),
            new MissionDef("Distance Ace", "Run 250 meters in one run", 250),
            new MissionDef("Survivor", "Stay alive for 45 seconds", 45)
        };

        int _runCoins;
        float _runTime;
        float _runDistance;

        public string HudLine { get; private set; } = "";

        public void BeginRun()
        {
            _runCoins = 0;
            _runTime = 0f;
            _runDistance = 0f;
            RefreshHud();
        }

        public void Track(int coins, float distance, float dt)
        {
            _runCoins = coins;
            _runDistance = distance;
            _runTime += dt;
            TryComplete(0, _runCoins);
            TryComplete(1, Mathf.FloorToInt(_runDistance));
            TryComplete(2, Mathf.FloorToInt(_runTime));
            RefreshHud();
        }

        void TryComplete(int index, int value)
        {
            SaveSystem.Data.missionProgress[index] = Mathf.Max(SaveSystem.Data.missionProgress[index], value);
            if (!SaveSystem.Data.missionComplete[index] && value >= Definitions[index].Target)
            {
                SaveSystem.Data.missionComplete[index] = true;
                SaveSystem.Data.totalCoins += 40;
                SaveSystem.Save();
            }
        }

        void RefreshHud()
        {
            for (int i = 0; i < Definitions.Length; i++)
            {
                if (!SaveSystem.Data.missionComplete[i])
                {
                    int current = i == 0 ? _runCoins : i == 1 ? Mathf.FloorToInt(_runDistance) : Mathf.FloorToInt(_runTime);
                    HudLine = $"{Definitions[i].Title}  {Mathf.Min(current, Definitions[i].Target)}/{Definitions[i].Target}";
                    return;
                }
            }

            HudLine = "All missions complete";
        }
    }
}
