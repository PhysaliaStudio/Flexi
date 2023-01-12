using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using static Physalia.Flexi.AbilityFlowStepper;

namespace Physalia.Flexi.Tests
{
    public class LifoQueueRunnerTests
    {
        // Runner always have at least 1 queue, prevent unnecessary common actions from 0 to 1
        [Test]
        public void Construct_CountOfQueueReturns1()
        {
            var runner = new LifoQueueRunner();
            Assert.AreEqual(1, runner.CountOfQueue);
        }

        [Test]
        public void Construct_PeekReturnsNull()
        {
            var runner = new LifoQueueRunner();
            Assert.AreEqual(null, runner.Peek());
        }

        [Test]
        public void EnqueueFlow_FlowA_PeekReturnsFlowA()
        {
            var runner = new LifoQueueRunner();

            var flowA = new FakeFlow();
            runner.EnqueueFlow(flowA);

            Assert.AreEqual(flowA, runner.Peek());
        }

        [Test]
        public void EnqueueFlow_FlowAThenFlowB_PeekReturnsFlowA()
        {
            var runner = new LifoQueueRunner();

            var flowA = new FakeFlow();
            runner.EnqueueFlow(flowA);

            var flowB = new FakeFlow();
            runner.EnqueueFlow(flowB);

            Assert.AreEqual(flowA, runner.Peek());
        }

        [Test]
        public void EnqueueFlow_FlowAThenAddNewQueueThenFlowB_PeekReturnsFlowB()
        {
            var runner = new LifoQueueRunner();

            var flowA = new FakeFlow();
            runner.EnqueueFlow(flowA);

            runner.AddNewQueue();

            var flowB = new FakeFlow();
            runner.EnqueueFlow(flowB);

            Assert.AreEqual(flowB, runner.Peek());
        }

        [Test]
        public void DequeueFlow_Empty_ReturnsNull()
        {
            var runner = new LifoQueueRunner();
            Assert.AreEqual(null, runner.DequeueFlow());
        }

        [Test]
        public void DequeueFlow_FlowA_ReturnsFlowA()
        {
            var runner = new LifoQueueRunner();

            var flowA = new FakeFlow();
            runner.EnqueueFlow(flowA);

            Assert.AreEqual(flowA, runner.DequeueFlow());
        }

        [Test]
        public void DequeueFlow_FlowAThenFlowB_ReturnsFlowA()
        {
            var runner = new LifoQueueRunner();

            var flowA = new FakeFlow();
            runner.EnqueueFlow(flowA);

            var flowB = new FakeFlow();
            runner.EnqueueFlow(flowB);

            Assert.AreEqual(flowA, runner.DequeueFlow());
        }

        [Test]
        public void DequeueFlow_FlowAThenAddNewQueueThenFlowB_ReturnsFlowBAndCountOfQueueReturns1()
        {
            var runner = new LifoQueueRunner();

            var flowA = new FakeFlow();
            runner.EnqueueFlow(flowA);

            runner.AddNewQueue();

            var flowB = new FakeFlow();
            runner.EnqueueFlow(flowB);

            Assert.AreEqual(flowB, runner.DequeueFlow());
            Assert.AreEqual(1, runner.CountOfQueue);
        }

        [Test]
        public void AddNewQueue_TheTopQueueIsEmpty_LogsWarningAndCountOfQueueReturns1()
        {
            var runner = new LifoQueueRunner();
            runner.AddNewQueue();
            Assert.AreEqual(1, runner.CountOfQueue);
            TestUtilities.LogAssertAnyString(LogType.Warning);
        }

        [Test]
        public void AddNewQueue_TheTopQueueIsNotEmpty_CountOfQueueReturns2()
        {
            var runner = new LifoQueueRunner();
            runner.EnqueueFlow(new FakeFlow());
            runner.AddNewQueue();
            Assert.AreEqual(2, runner.CountOfQueue);
        }

        [Test]
        public void Clear_FlowAThenAddNewQueueThenFlowB_PeekReturnsNullAndCountOfQueueReturns1()
        {
            var runner = new LifoQueueRunner();

            var flowA = new FakeFlow();
            runner.EnqueueFlow(flowA);

            runner.AddNewQueue();

            var flowB = new FakeFlow();
            runner.EnqueueFlow(flowB);

            runner.Clear();

            Assert.AreEqual(null, runner.Peek());
            Assert.AreEqual(1, runner.CountOfQueue);
        }

        [Test]
        public void Start_FlowAWith2NodesThenFlowBWith3Nodes_A0A1B0B1B2()
        {
            var runner = new LifoQueueRunner();
            var flowA = new FakeFlow(2);
            var flowB = new FakeFlow(3);

            runner.EnqueueFlow(flowA);
            runner.EnqueueFlow(flowB);

            var record = new List<StepResult>();
            runner.StepExecuted += x => record.Add(x);

            runner.Start();

            var expected = new List<StepResult> {
                new StepResult(flowA, flowA[0], ExecutionType.NODE_EXECUTION, ResultState.SUCCESS),
                new StepResult(flowA, flowA[1], ExecutionType.NODE_EXECUTION, ResultState.SUCCESS),
                new StepResult(flowA, null, ExecutionType.FLOW_FINISH, ResultState.SUCCESS),
                new StepResult(flowB, flowB[0], ExecutionType.NODE_EXECUTION, ResultState.SUCCESS),
                new StepResult(flowB, flowB[1], ExecutionType.NODE_EXECUTION, ResultState.SUCCESS),
                new StepResult(flowB, flowB[2], ExecutionType.NODE_EXECUTION, ResultState.SUCCESS),
                new StepResult(flowB, null, ExecutionType.FLOW_FINISH, ResultState.SUCCESS),
            };
            TestUtilities.AreListEqual(expected, record);
        }

        [Test]
        public void Start_FlowAWith2NodesAndEnqueueFlowBWith2NodesAfterA0_A0B0B1A1()
        {
            var runner = new LifoQueueRunner();
            var flowA = new FakeFlow(2);
            var flowB = new FakeFlow(2);

            runner.EnqueueFlow(flowA);

            var record = new List<StepResult>();
            runner.StepExecuted += x =>
            {
                record.Add(x);

                // Normally you shouldn't do this!!
                if (x.node == flowA[0])
                {
                    runner.AddNewQueue();
                    runner.EnqueueFlow(flowB);
                }
            };

            runner.Start();

            var expected = new List<StepResult> {
                new StepResult(flowA, flowA[0], ExecutionType.NODE_EXECUTION, ResultState.SUCCESS),
                new StepResult(flowB, flowB[0], ExecutionType.NODE_EXECUTION, ResultState.SUCCESS),
                new StepResult(flowB, flowB[1], ExecutionType.NODE_EXECUTION, ResultState.SUCCESS),
                new StepResult(flowB, null, ExecutionType.FLOW_FINISH, ResultState.SUCCESS),
                new StepResult(flowA, flowA[1], ExecutionType.NODE_EXECUTION, ResultState.SUCCESS),
                new StepResult(flowA, null, ExecutionType.FLOW_FINISH, ResultState.SUCCESS),
            };
            TestUtilities.AreListEqual(expected, record);
        }

        [Test]
        public void Start_FlowAWith3NodesAndPauseAfterA1_A0A1()
        {
            var runner = new LifoQueueRunner();
            var flowA = new FakeFlow(3);
            flowA.SetPauseCount(1, 1);

            runner.EnqueueFlow(flowA);

            var record = new List<StepResult>();
            runner.StepExecuted += x => record.Add(x);

            runner.Start();

            var expected = new List<StepResult> {
                new StepResult(flowA, flowA[0], ExecutionType.NODE_EXECUTION, ResultState.SUCCESS),
                new StepResult(flowA, flowA[1], ExecutionType.NODE_EXECUTION, ResultState.PAUSE),
            };
            TestUtilities.AreListEqual(expected, record);
        }

        [Test]
        public void StartAndResume_FlowAWith3NodesAndPauseAfterA1_A0A1A1A2()
        {
            var runner = new LifoQueueRunner();
            var flowA = new FakeFlow(3);
            flowA.SetPauseCount(1, 1);

            runner.EnqueueFlow(flowA);

            var record = new List<StepResult>();
            runner.StepExecuted += x => record.Add(x);

            runner.Start();
            runner.Resume(null);

            var expected = new List<StepResult> {
                new StepResult(flowA, flowA[0], ExecutionType.NODE_EXECUTION, ResultState.SUCCESS),
                new StepResult(flowA, flowA[1], ExecutionType.NODE_EXECUTION, ResultState.PAUSE),
                new StepResult(flowA, flowA[1], ExecutionType.NODE_RESUME, ResultState.SUCCESS),
                new StepResult(flowA, flowA[2], ExecutionType.NODE_EXECUTION, ResultState.SUCCESS),
                new StepResult(flowA, null, ExecutionType.FLOW_FINISH, ResultState.SUCCESS),
            };

            TestUtilities.AreListEqual(expected, record);
        }

        [Test]
        public void StartAndResume_FlowAWith3NodesAndPause2TimesAfterA1_A0A1A1A1A2()
        {
            var runner = new LifoQueueRunner();
            var flowA = new FakeFlow(3);
            flowA.SetPauseCount(1, 2);

            runner.EnqueueFlow(flowA);

            var record = new List<StepResult>();
            runner.StepExecuted += x => record.Add(x);

            runner.Start();
            runner.Resume(null);
            runner.Resume(null);

            var expected = new List<StepResult> {
                new StepResult(flowA, flowA[0], ExecutionType.NODE_EXECUTION, ResultState.SUCCESS),
                new StepResult(flowA, flowA[1], ExecutionType.NODE_EXECUTION, ResultState.PAUSE),
                new StepResult(flowA, flowA[1], ExecutionType.NODE_RESUME, ResultState.PAUSE),
                new StepResult(flowA, flowA[1], ExecutionType.NODE_RESUME, ResultState.SUCCESS),
                new StepResult(flowA, flowA[2], ExecutionType.NODE_EXECUTION, ResultState.SUCCESS),
                new StepResult(flowA, null, ExecutionType.FLOW_FINISH, ResultState.SUCCESS),
            };

            TestUtilities.AreListEqual(expected, record);
        }
    }
}
