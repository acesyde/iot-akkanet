using System;
using System.Collections.Generic;
using Akka.Actor;

namespace BuildingMonitor.Actors
{
    public class FloorQuery : UntypedActor
    {
        private readonly Dictionary<IActorRef, string> _actorToSensorId;
        private readonly int _requestId;
        private readonly IActorRef _requester;
        private readonly TimeSpan _timeout;
        public static readonly long TemperatureRequestCorrelationId = 42;

        public FloorQuery(Dictionary<IActorRef, string> actorToSensorId, int requestId, IActorRef requester, TimeSpan timeout)
        {
            _actorToSensorId = actorToSensorId;
            _requestId = requestId;
            _requester = requester;
            _timeout = timeout;
        }

        protected override void OnReceive(object message)
        {
            throw new NotImplementedException();
        }

        public static Props Props(Dictionary<IActorRef, string> actorToSensorId, int requestId, IActorRef requester, TimeSpan timeout)
            => Akka.Actor.Props.Create(() => new FloorQuery(actorToSensorId, requestId, requester, timeout));
    }
}
