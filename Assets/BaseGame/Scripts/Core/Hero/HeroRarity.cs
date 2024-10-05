namespace Core
{
    public partial class Hero
    {
        public enum Rarity
        {
            Common = 0,
            Rare = 1,
            Epic = 2,
            Legendary = 3,
            Mythic = 4
        }
    }

    public static class HeroRarityExtension
    {
        public static bool IsMaxRarity(this Hero.Rarity rarity)
        {
            return rarity == Hero.Rarity.Mythic;
        }
    }
}