namespace Physalia.AbilityFramework
{
    [NodeCategory("Built-in/Common")]
    public class ActorNode : ValueNode
    {
        public Outport<Actor> actor;

        protected override void EvaluateSelf()
        {
            if (Instance != null)
            {
                actor.SetValue(Actor);
            }
            else
            {
                actor.SetValue(null);
            }
        }
    }
}
