using System;

namespace Dominator.WebDriver.Models
{
    public class EventReceivedEventArgs : EventArgs
    {
        public string EventData { get; private set; }

        public EventReceivedEventArgs(string eventData)
        {
            EventData = eventData;
        }
    }
}
