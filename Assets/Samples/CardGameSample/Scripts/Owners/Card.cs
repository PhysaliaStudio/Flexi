namespace Physalia.Flexi.Samples.CardGame
{
    public class Card : Actor
    {
        private readonly CardData cardData;

        public string Name => cardData.Name;
        public string Text => cardData.Text;

        public Card(CardData cardData)
        {
            this.cardData = cardData;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
