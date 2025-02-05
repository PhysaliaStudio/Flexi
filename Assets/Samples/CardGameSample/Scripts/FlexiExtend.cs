using System;

namespace Physalia.Flexi.Samples.CardGame
{
    public class DefaultAbilityContainer : AbilityContainer
    {
        private readonly Game game;
        public Unit Unit;
        public Card Card;

        public Game Game => game;
        public Random Random => game.Random;

        public DefaultAbilityContainer(Game game, AbilityData abilityData, int groupIndex)
            : base(abilityData, groupIndex)
        {
            this.game = game;
        }

        public void CleanUp()
        {
            Unit = null;
            Card = null;
        }
    }

    public abstract class DefaultEntryNode : EntryNode<DefaultAbilityContainer>
    {

    }

    public abstract class DefaultEntryNode<TEventContext> : EntryNode<DefaultAbilityContainer, TEventContext>
        where TEventContext : IEventContext
    {

    }

    public abstract class DefaultProcessNode : ProcessNode<DefaultAbilityContainer>
    {

    }

    public abstract class DefaultModifierNode : ModifierNode<DefaultAbilityContainer>
    {

    }

    public abstract class DefaultValueNode : ValueNode<DefaultAbilityContainer>
    {

    }
}
