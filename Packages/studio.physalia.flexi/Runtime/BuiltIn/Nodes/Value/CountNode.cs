using System.Collections;

namespace Physalia.Flexi
{
    [NodeCategory(BuiltInCategory.Value)]
    public class CountNode : ValueNode
    {
        public Inport<ICollection> collectionPort;
        public Outport<int> countPort;

        protected override void EvaluateSelf()
        {
            ICollection collection = collectionPort.GetValue();
            if (collection == null)
            {
                countPort.SetValue(0);
                return;
            }

            countPort.SetValue(collection.Count);
        }
    }
}
