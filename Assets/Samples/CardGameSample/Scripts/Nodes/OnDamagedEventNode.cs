namespace Physalia.Flexi.Samples.CardGame
{
    [NodeCategory("Card Game Sample")]
    public class OnDamagedEventNode : EntryNode
    {
        public Outport<Unit> attackerPort;

        public override bool CanExecute(IEventContext payload)
        {
            if (payload is DamageEvent damageEvent)
            {
                for (var i = 0; i < damageEvent.targets.Count; i++)
                {
                    if (damageEvent.targets[i] == Actor)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        protected override AbilityState DoLogic()
        {
            var payload = GetPayload<DamageEvent>();
            attackerPort.SetValue(payload.attacker);
            return AbilityState.RUNNING;
        }
    }
}
