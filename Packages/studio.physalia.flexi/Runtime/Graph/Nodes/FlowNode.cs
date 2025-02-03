using System.Collections.Generic;

namespace Physalia.Flexi
{
    public enum FlowState
    {
        Success,
        Pause,
        Abort,
    }

    public abstract class FlowNode : Node
    {
        public abstract FlowNode Next { get; }

        /// <summary>
        /// This property only works when the FlowRunner is in EventTriggerMode.EACH_NODE.
        /// </summary>
        public virtual bool ShouldTriggerChainEvents => true;

        // Note: FlowNode shouldn't be inherited outside of this assembly
        internal FlowNode()
        {

        }

        internal FlowState Execute()
        {
            EvaluateInports();
            return ExecuteInternal();
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

        private protected abstract FlowState ExecuteInternal();

        protected internal virtual bool CanResume(IResumeContext resumeContext)
        {
            return false;
        }

        internal FlowState Resume(IResumeContext resumeContext)
        {
            return OnResume(resumeContext);
        }

        protected virtual FlowState OnResume(IResumeContext resumeContext)
        {
            return FlowState.Success;
        }

        protected internal virtual FlowState Tick()
        {
            return FlowState.Pause;
        }

        protected void PushSelf()
        {
            Flow?.Push(this);
        }

        protected void EnqueueEvent(IEventContext eventContext)
        {
            Flow.Core?.EnqueueEvent(eventContext);
        }
    }
}
