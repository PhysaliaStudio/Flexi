namespace Physalia.Flexi.Samples.CardGame
{
    public class Player : Actor
    {
        private int mana;
        private int coin;

        public int Mana { get => mana; set => mana = value; }
        public int Coin { get => coin; set => coin = value; }

        public Player(AbilitySystem abilitySystem) : base(abilitySystem)
        {

        }
    }
}
