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
            NodeExecution,
            NodeResume,
            NodeTick,
            FlowFinish,
        }

        public enum ResultState
        {
            Success, Fail, Abort, Pause
        }

        public static StepResult ExecuteStep(IAbilityFlow flow)
        {
            if (!flow.MoveNext())
            {
                // The graph is empty or has already reached the final node.
                // We keep it until resolving all flows pushed, and dequeue it at here.
                return new StepResult(flow, null, ExecutionType.FlowFinish, ResultState.Success);
            }

            FlowNode node = flow.Current;

            FlowState state;
            try
            {
                state = node.Execute();
            }
            catch (Exception ex)
            {
                Logger.Fatal(ex);
                state = FlowState.Abort;
            }

            if (state == FlowState.Abort)
            {
                return new StepResult(flow, node, ExecutionType.NodeExecution, ResultState.Abort);
            }
            else if (state == FlowState.Pause)
            {
                return new StepResult(flow, node, ExecutionType.NodeExecution, ResultState.Pause);
            }
            else
            {
                return new StepResult(flow, node, ExecutionType.NodeExecution, ResultState.Success);
            }
        }

        public static StepResult ResumeStep(IAbilityFlow flow, IResumeContext resumeContext)
        {
            FlowNode node = flow.Current;

            bool success = node.CanResume(resumeContext);
            if (!success)
            {
                return new StepResult(flow, node, ExecutionType.NodeResume, ResultState.Fail);
            }

            FlowState state;
            try
            {
                state = node.Resume(resumeContext);
            }
            catch (Exception ex)
            {
                Logger.Fatal(ex);
                state = FlowState.Abort;
            }

            if (state == FlowState.Abort)
            {
                return new StepResult(flow, node, ExecutionType.NodeResume, ResultState.Abort);
            }
            else if (state == FlowState.Pause)
            {
                return new StepResult(flow, node, ExecutionType.NodeResume, ResultState.Pause);
            }
            else
            {
                return new StepResult(flow, node, ExecutionType.NodeResume, ResultState.Success);
            }
        }

        public static StepResult TickStep(IAbilityFlow flow)
        {
            FlowNode node = flow.Current;

            FlowState state;
            try
            {
                state = node.Tick();
            }
            catch (Exception ex)
            {
                Logger.Fatal(ex);
                state = FlowState.Abort;
            }

            if (state == FlowState.Abort)
            {
                return new StepResult(flow, node, ExecutionType.NodeTick, ResultState.Abort);
            }
            else if (state == FlowState.Pause)
            {
                return new StepResult(flow, node, ExecutionType.NodeTick, ResultState.Pause);
            }
            else
            {
                return new StepResult(flow, node, ExecutionType.NodeTick, ResultState.Success);
            }
        }
    }
}
