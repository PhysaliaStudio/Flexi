using System.Collections.Generic;

namespace Physalia.Flexi
{
    public abstract class FlowNode : Node
    {
        public abstract FlowNode Previous { get; }
        public abstract FlowNode Next { get; }

        public AbilityState Run()
        {
            EvaluateInports();
            return DoLogic();
        }

        private protected void EvaluateInports()
        {
            IReadOnlyList<Inport> inports = Inports;
            for (var i = 0; i < inports.Count; i++)
            {
                Inport inport = inports[i];
                IReadOnlyList<Port> connections = inport.GetConnections();
                for (var j = 0; j < connections.Count; j++)
                {
                    var outport = connections[j] as Outport;
                    if (outport.Node is ValueNode valueNode)
                    {
                        valueNode.Evaluate();
                    }
                }
            }
        }

        protected virtual AbilityState DoLogic()
        {
            return AbilityState.RUNNING;
        }

        public AbilityState Resume(IResumeContext resumeContext)
        {
            return ResumeLogic(resumeContext);
        }

        protected virtual AbilityState ResumeLogic(IResumeContext resumeContext)
        {
            return AbilityState.RUNNING;
        }

        protected internal virtual AbilityState Tick()
        {
            return AbilityState.PAUSE;
        }

        protected void PushSelf()
        {
            Flow?.Push(this);
        }

        protected AbilityState WaitAndChoice(IChoiceContext context)
        {
            Flow.System?.TriggerChoice(context);
            return AbilityState.PAUSE;
        }

        protected void EnqueueEvent(IEventContext eventContext)
        {
            Flow.System?.EnqueueEvent(eventContext);
        }
    }
}
