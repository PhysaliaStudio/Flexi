using System.Collections.Generic;

namespace Physalia.Flexi.Samples.CardGame
{
    public class Card : StatOwner
    {
        private readonly CardData cardData;
        private readonly List<AbilityContainer> containers = new();

        public string Name => cardData.Name;
        public string Text => cardData.Text;
        public IReadOnlyList<AbilityContainer> AbilityContainers => containers;

        public Card(CardData cardData)
        {
            this.cardData = cardData;
        }

        public override string ToString()
        {
            return Name;
        }

        public void AppendAbilityContainer(AbilityContainer container)
        {
            container.card = this;
            containers.Add(container);
        }

        public void RemoveAbilityContainer(AbilityContainer container)
        {
            container.CleanUp();
            containers.Remove(container);
        }

        public void ClearAbilityContainers()
        {
            for (var i = 0; i < containers.Count; i++)
            {
                containers[i].CleanUp();
            }
            containers.Clear();
        }
    }
}
