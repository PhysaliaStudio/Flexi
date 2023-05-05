namespace Physalia.Flexi.Samples.CardGame
{
    public class Card : Actor
    {
        private readonly CardData cardData;

        public string Name => cardData.Name;
        public string Text => cardData.Text;

        public Card(CardData cardData, AbilitySystem abilitySystem) : base(abilitySystem)
        {
            this.cardData = cardData;
        }

        public override string ToString()
        {
            return $"{OwnerId}-{Name}";
        }
    }
}
