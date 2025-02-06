namespace Physalia.Flexi.Samples.CardGame
{
    [NodeCategory("Card Game Sample")]
    public class ClearManaNode : DefaultProcessNode
    {
        protected override FlowState OnExecute()
        {
            Player player = Container.Game.Player;
            int mana = player.Mana;
            player.Mana = 0;

            EnqueueEvent(new ManaChangeEvent
            {
                modifyValue = -mana,
                newAmount = 0,
            });

            return FlowState.Success;
        }
    }
}
