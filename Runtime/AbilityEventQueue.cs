using System.Collections.Generic;

namespace Physalia.AbilityFramework
{
    public class AbilityEventQueue
    {
        private readonly List<object> eventList = new();
        private int currentIndex = -1;

        public int Count => eventList.Count;

        public object Current
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

        public void Enqueue(object payload)
        {
            if (payload == null)
            {
                return;
            }

            eventList.Add(payload);
        }

        public object Dequeue()
        {
            if (eventList.Count == 0)
            {
                return null;
            }

            object payload = eventList[0];
            eventList.RemoveAt(0);
            return payload;
        }
    }
}
