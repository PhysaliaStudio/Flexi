namespace Physalia.Flexi.Samples.CardGame
{
    [NodeCategory("Card Game Sample")]
    public class ClearManaNode : ProcessNode
    {
        public Inport<Player> playerPort;

        protected override AbilityState DoLogic()
        {
            Player player = playerPort.GetValue();
            int mana = player.Mana;
            player.Mana = 0;

            EnqueueEvent(new ManaChangeEvent
            {
                modifyValue = -mana,
                newAmount = 0,
            });

            return AbilityState.RUNNING;
        }
    }
}
