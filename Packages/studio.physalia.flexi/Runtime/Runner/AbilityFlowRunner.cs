using System;
using static Physalia.Flexi.AbilityFlowStepper;

namespace Physalia.Flexi
{
    public abstract class AbilityFlowRunner
    {
        public enum RunningState
        {
            IDLE, RUNNING, PAUSE
        }

        public enum EventTriggerMode
        {
            EACH_NODE,
            EACH_FLOW,
            NEVER,
        }

        internal event Action<IAbilityFlow> FlowFinished;
        internal event Action<StepResult> StepExecuted;

        internal AbilitySystem abilitySystem;

        protected EventTriggerMode eventTriggerMode = EventTriggerMode.EACH_NODE;


        internal void SetEventTriggerMode(EventTriggerMode eventTriggerMode)
        {
            this.eventTriggerMode = eventTriggerMode;
        }

        public abstract void AddFlow(IAbilityFlow flow);
        public abstract bool IsFlowRunning(IAbilityFlow flow);

        public abstract void Start();
        public abstract void Resume(IResumeContext resumeContext);
        public abstract void Tick();

        public virtual void BeforeTriggerEvents()
        {

        }

        public virtual void AfterTriggerEvents()
        {

        }

        public virtual void Clear()
        {

        }

        protected void NotifyFlowFinished(IAbilityFlow flow)
        {
            try
            {
                FlowFinished?.Invoke(flow);
            }
            catch (Exception e)
            {
                Logger.Fatal(e);
            }
        }

        protected void NotifyStepResult(StepResult result)
        {
            StepExecuted?.Invoke(result);
        }
    }
}
