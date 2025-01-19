namespace Physalia.Flexi.Tests
{
    [NodeCategoryForTests]
    public class ActorNode : ValueNode
    {
        public Outport<Actor> actor;

        protected override void EvaluateSelf()
        {
            if (Flow != null)
            {
                actor.SetValue(Container.Actor);
            }
            else
            {
                actor.SetValue(null);
            }
        }
    }
}
