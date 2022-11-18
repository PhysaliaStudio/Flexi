namespace Physalia.AbilityFramework
{
    [NodeCategory("Built-in/Stats")]
    public class StatCompareNode : ValueNode
    {
        public Inport<StatOwner> ownerPort;
        public Outport<bool> resultPort;
        public Variable<int> statId;
        public Variable<CompareOperator> op;
        public Variable<int> value;

        protected override void EvaluateSelf()
        {
            StatOwner owner = ownerPort.GetValue();
            Stat stat = owner.GetStat(statId.Value);
            if (stat == null)
            {
                resultPort.SetValue(false);
                return;
            }

            bool result = op.Value.Compare(stat.CurrentValue, value.Value);
            resultPort.SetValue(result);
        }
    }
}
