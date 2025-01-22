using System;

namespace Physalia.Flexi.Tests
{
    [NodeCategoryForTests]
    public class CustomExceptionNode : DefaultProcessNode
    {
        protected override FlowState OnExecute()
        {
            throw new Exception("This is for testing");
        }
    }
}
