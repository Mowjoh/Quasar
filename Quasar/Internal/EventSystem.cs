using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quasar.Internal
{
    /// <summary>
    /// Static EventAggregator
    /// </summary>
    public static class EventSystem
    {
        private static IEventAggregator _current;
        public static IEventAggregator Current
        {
            get
            {
                return _current ?? (_current = new EventAggregator());
            }
        }

        private static PubSubEvent<TEvent> GetEvent<TEvent>()
        {
            return Current.GetEvent<PubSubEvent<TEvent>>();
        }

        public static void Publish<TEvent>()
        {
            Publish<TEvent>(default(TEvent));
        }

        public static void Publish<TEvent>(TEvent @event)
        {
            GetEvent<TEvent>().Publish(@event);
        }

        public static SubscriptionToken Subscribe<TEvent>(Action action, ThreadOption threadOption = ThreadOption.PublisherThread, bool keepSubscriberReferenceAlive = false)
        {
            return Subscribe<TEvent>(e => action(), threadOption, keepSubscriberReferenceAlive);
        }

        public static SubscriptionToken Subscribe<TEvent>(Action<TEvent> action, ThreadOption threadOption = ThreadOption.PublisherThread, bool keepSubscriberReferenceAlive = false, Predicate<TEvent> filter = null)
        {
            return GetEvent<TEvent>().Subscribe(action, threadOption, keepSubscriberReferenceAlive, filter);
        }

        public static void Unsubscribe<TEvent>(SubscriptionToken token)
        {
            GetEvent<TEvent>().Unsubscribe(token);
        }
        public static void Unsubscribe<TEvent>(Action<TEvent> subscriber)
        {
            GetEvent<TEvent>().Unsubscribe(subscriber);
        }
    }
}