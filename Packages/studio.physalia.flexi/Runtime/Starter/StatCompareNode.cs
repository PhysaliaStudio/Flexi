namespace Physalia.Flexi
{
    [NodeCategory(BuiltInCategory.Stats)]
    public class StatCompareNode : ValueNode
    {
        public Inport<Actor> actorPort;
        public Outport<bool> resultPort;
        public Variable<int> statId;
        public Variable<CompareOperator> op;
        public Variable<int> value;

        protected override void EvaluateSelf()
        {
            Actor actor = actorPort.GetValue();
            Stat stat = actor.GetStat(statId.Value);
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
