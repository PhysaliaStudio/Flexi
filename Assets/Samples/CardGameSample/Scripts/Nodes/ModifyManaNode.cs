namespace Physalia.Flexi.Samples.CardGame
{
    [NodeCategory("Card Game Sample")]
    public class ModifyManaNode : DefaultProcessNode
    {
        public Inport<Player> playerPort;
        public Inport<int> amountPort;

        protected override AbilityState OnExecute()
        {
            Player player = playerPort.GetValue();
            int amount = amountPort.GetValue();
            player.Mana += amount;

            EnqueueEvent(new ManaChangeEvent
            {
                modifyValue = amount,
                newAmount = player.Mana,
            });

            return AbilityState.RUNNING;
        }
    }
}
