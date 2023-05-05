namespace Physalia.Flexi.Samples.CardGame
{
    [NodeCategory("Card Game Sample")]
    public class ClearManaNode : ProcessNode
    {
        public Inport<Player> playerPort;

        protected override AbilityState DoLogic()
        {
            Player player = playerPort.GetValue();
            int mana = player.GetStat(StatId.MANA).CurrentValue;
            player.ModifyStat(StatId.MANA, -mana);

            EnqueueEvent(new ManaChangeEvent
            {
                modifyValue = -mana,
                newAmount = 0,
            });

            return AbilityState.RUNNING;
        }
    }
}
