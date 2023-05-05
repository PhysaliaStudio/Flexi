using System;

namespace Physalia.Flexi
{
    public static class AbilityFlowStepper
    {
        public struct StepResult
        {
            internal IAbilityFlow flow;
            internal FlowNode node;
            internal ExecutionType type;
            internal ResultState state;

            internal StepResult(IAbilityFlow flow, FlowNode node, ExecutionType type, ResultState state)
            {
                this.flow = flow;
                this.node = node;
                this.type = type;
                this.state = state;
            }
        }

        public enum ExecutionType
        {
            NODE_EXECUTION,
            NODE_RESUME,
            NODE_TICK,
            FLOW_FINISH,
        }

        public enum ResultState
        {
            SUCCESS, FAILED, ABORT, PAUSE
        }

        public static StepResult ExecuteStep(IAbilityFlow flow)
        {
            if (!flow.MoveNext())
            {
                // The graph is empty or has already reached the final node.
                // We keep it until resolving all flows pushed, and dequeue it at here.
                return new StepResult(flow, null, ExecutionType.FLOW_FINISH, ResultState.SUCCESS);
            }

            FlowNode node = flow.Current;

            AbilityState state;
            try
            {
                state = node.Run();
            }
            catch (Exception ex)
            {
                Logger.Fatal(ex);
                state = AbilityState.ABORT;
            }

            if (state == AbilityState.ABORT)
            {
                return new StepResult(flow, node, ExecutionType.NODE_EXECUTION, ResultState.ABORT);
            }
            else if (state == AbilityState.PAUSE)
            {
                return new StepResult(flow, node, ExecutionType.NODE_EXECUTION, ResultState.PAUSE);
            }
            else
            {
                return new StepResult(flow, node, ExecutionType.NODE_EXECUTION, ResultState.SUCCESS);
            }
        }

        public static StepResult ResumeStep(IAbilityFlow flow, IResumeContext resumeContext)
        {
            FlowNode node = flow.Current;

            bool success = node.CheckNodeContext(resumeContext);
            if (!success)
            {
                return new StepResult(flow, node, ExecutionType.NODE_RESUME, ResultState.FAILED);
            }

            AbilityState state;
            try
            {
                state = node.Resume(resumeContext);
            }
            catch (Exception ex)
            {
                Logger.Fatal(ex);
                state = AbilityState.ABORT;
            }

            if (state == AbilityState.ABORT)
            {
                return new StepResult(flow, node, ExecutionType.NODE_RESUME, ResultState.ABORT);
            }
            else if (state == AbilityState.PAUSE)
            {
                return new StepResult(flow, node, ExecutionType.NODE_RESUME, ResultState.PAUSE);
            }
            else
            {
                return new StepResult(flow, node, ExecutionType.NODE_RESUME, ResultState.SUCCESS);
            }
        }

        public static StepResult TickStep(IAbilityFlow flow)
        {
            FlowNode node = flow.Current;

            AbilityState state;
            try
            {
                state = node.Tick();
            }
            catch (Exception ex)
            {
                Logger.Fatal(ex);
                state = AbilityState.ABORT;
            }

            if (state == AbilityState.ABORT)
            {
                return new StepResult(flow, node, ExecutionType.NODE_TICK, ResultState.ABORT);
            }
            else if (state == AbilityState.PAUSE)
            {
                return new StepResult(flow, node, ExecutionType.NODE_TICK, ResultState.PAUSE);
            }
            else
            {
                return new StepResult(flow, node, ExecutionType.NODE_TICK, ResultState.SUCCESS);
            }
        }
    }
}
