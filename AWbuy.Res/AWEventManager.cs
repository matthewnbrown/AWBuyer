using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace AWbuy.AWbuy.Res
{
    class AWEventManager
    {

        private bool gettingEvents = false;
        private List<awEvent> eventList = new List<awEvent>();

        public struct awEvent
        {
            private string eventName;
            private Int64 startTime;
            private Int64 endTime;

            public awEvent(string eventName, Int64 startTime, Int64 endTime)
            {
                this.eventName = eventName;
                this.startTime = startTime;
                this.endTime = endTime;
            }

            public string GetName() => this.eventName;

            public Int64 TimeTillStart()
            {
                return startTime - currentTime();
                
            }
            public Int64 EventLength()
            {
                return endTime - startTime;
            }
            public Int64 TimeTillEnd()
            {
                return endTime - currentTime();
            }
            public Boolean IsEventActive()
            {
                return (currentTime() > startTime) && (endTime > currentTime());
            }

            public Boolean IsExpired()
            {
                if(endTime < currentTime())
                {
                    return true;
                }

                return false;
            }

            private Int64 currentTime()
            {
                return (Int64)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;
            }
        }

        /// <summary>
        /// Adds the given event to the event list if it is not already inside
        /// </summary>
        /// <param name="eventName">Name of the event</param>
        /// <param name="startTime">Time the event will starts (seconds sinch epoch)</param>
        /// <param name="endTime">Time the event will end(seconds sinch epoch)</param>
        /// <returns></returns>
        public Boolean AddEvent(string eventName, Int64 startTime, Int64 endTime)
        {
            if(endTime < startTime || endTime < 1471500281 || startTime < 1471500281)
            {
                return false;
            }

            awEvent newEvent = new awEvent(eventName, startTime, endTime);

            if(eventList.Contains(newEvent))
            {
                return false;
            }

            while(gettingEvents)
            {
                Thread.Sleep(50);
            }

            eventList.Add(newEvent);

            return true;
        }

        /// <summary>
        /// Gets the events which should be currently running
        /// </summary>
        /// <returns>An array of awEvents</returns>
        public awEvent [] GetCurrentEvents()
        {
            if(eventList.Count == 0)
            {
                return null;
            }
            gettingEvents = true;
            CleanEvents();
            List<awEvent> curEvents = new List<awEvent>();

            try
            {
                foreach (awEvent awe in eventList)
                {
                    if (awe.IsEventActive())
                    {
                        curEvents.Add(awe);
                    }
                }
            }
            catch(Exception e)
            {

            }

            gettingEvents = false;
            return curEvents.ToArray();
        }

        private void CleanEvents()
        {
            var list = eventList.ToList();

            for(int i = 0; i < list.Count; i++)
            {
                if(list[i].IsExpired())
                {
                    eventList.Remove(list[i]);
                }
            }
        }
    }
}
