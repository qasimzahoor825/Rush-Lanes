using UnityEngine;

namespace RushLanes
{
    public readonly struct CharacterDef
    {
        public readonly string Name;
        public readonly string Blurb;
        public readonly Color Body;
        public readonly Color Accent;
        public readonly int Cost;

        public CharacterDef(string name, string blurb, Color body, Color accent, int cost)
        {
            Name = name;
            Blurb = blurb;
            Body = body;
            Accent = accent;
            Cost = cost;
        }
    }

    public static class CharacterCatalog
    {
        public static readonly CharacterDef[] All =
        {
            new CharacterDef("Spark", "Balanced street racer", new Color(0.15f, 0.92f, 0.95f), Color.white, 0),
            new CharacterDef("Ember", "Heat-trail sprinter", new Color(1f, 0.45f, 0.18f), new Color(1f, 0.85f, 0.3f), 80),
            new CharacterDef("Nova", "Night-circuit ace", new Color(0.78f, 0.28f, 1f), new Color(0.4f, 0.9f, 1f), 160)
        };

        public static CharacterDef Selected => All[Mathf.Clamp(SaveSystem.Data.selectedCharacter, 0, All.Length - 1)];

        public static bool Unlock(int index)
        {
            var def = All[index];
            if (SaveSystem.Data.unlocked[index])
                return true;
            if (SaveSystem.Data.totalCoins < def.Cost)
                return false;
            SaveSystem.Data.totalCoins -= def.Cost;
            SaveSystem.Data.unlocked[index] = true;
            SaveSystem.Data.selectedCharacter = index;
            SaveSystem.Save();
            return true;
        }
    }
}
