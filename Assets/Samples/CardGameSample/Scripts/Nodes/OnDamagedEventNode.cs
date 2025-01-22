namespace Physalia.Flexi.Samples.CardGame
{
    [NodeCategory("Card Game Sample")]
    public class OnDamagedEventNode : DefaultEntryNode<DamageContext>
    {
        public Outport<Unit> attackerPort;

        public override bool CanExecute(DamageContext context)
        {
            for (var i = 0; i < context.targets.Count; i++)
            {
                if (context.targets[i] == Container.Unit)
                {
                    return true;
                }
            }

            return false;
        }

        protected override FlowState OnExecute(DamageContext context)
        {
            attackerPort.SetValue(context.attacker);
            return FlowState.Success;
        }
    }
}
