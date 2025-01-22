using System.Collections.Generic;

namespace Physalia.Flexi
{
    public class AbilityEventQueue
    {
        private readonly List<IEventContext> eventList = new();
        private int currentIndex = -1;

        public int Count => eventList.Count;

        public IEventContext Current
        {
            get
            {
                if (currentIndex < 0 || currentIndex >= eventList.Count)
                {
                    return null;
                }

                return eventList[currentIndex];
            }
        }

        public bool Next()
        {
            currentIndex++;
            if (currentIndex < 0 || currentIndex >= eventList.Count)
            {
                return false;
            }

            return true;
        }

        public void Reset()
        {
            currentIndex = -1;
        }

        public void Enqueue(IEventContext eventContext)
        {
            if (eventContext == null)
            {
                return;
            }

            eventList.Add(eventContext);
        }

        public IEventContext Dequeue()
        {
            if (eventList.Count == 0)
            {
                return null;
            }

            IEventContext eventContext = eventList[0];
            eventList.RemoveAt(0);
            return eventContext;
        }
    }
}
