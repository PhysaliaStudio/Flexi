namespace Physalia.Flexi.Samples.CardGame
{
    [NodeCategory("Card Game Sample")]
    public class OnDamagedEventNode : EntryNode<DamageContext>
    {
        public Outport<Unit> attackerPort;

        public override bool CanExecute(DamageContext context)
        {
            for (var i = 0; i < context.targets.Count; i++)
            {
                if (context.targets[i] == Actor)
                {
                    return true;
                }
            }

            return false;
        }

        protected override AbilityState DoLogic()
        {
            var context = GetPayload<DamageContext>();
            attackerPort.SetValue(context.attacker);
            return AbilityState.RUNNING;
        }
    }
}
