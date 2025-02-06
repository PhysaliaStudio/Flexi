namespace Physalia.Flexi.Samples.CardGame
{
    [NodeCategory("Card Game Sample")]
    public class ModifyManaNode : DefaultProcessNode
    {
        public Inport<int> amountPort;

        protected override FlowState OnExecute()
        {
            Player player = Container.Game.Player;
            int amount = amountPort.GetValue();
            player.Mana += amount;

            EnqueueEvent(new ManaChangeEvent
            {
                modifyValue = amount,
                newAmount = player.Mana,
            });

            return FlowState.Success;
        }
    }
}
