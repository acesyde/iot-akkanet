using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Akka.Actor;
using BuildingMonitor.Messages;

namespace BuildingMonitor.Actors
{
    public class Floor : UntypedActor
    {
        private readonly string _floorId;
        private readonly Dictionary<string, IActorRef> _sensorIdToActorRefs = new Dictionary<string, IActorRef>();

        public Floor(string floorId)
        {
            _floorId = floorId;
        }

        protected override void OnReceive(object message)
        {
            switch (message)
            {
                case RequestRegisterTemperatureSensor m when m.FloorId == _floorId:
                    if (_sensorIdToActorRefs.TryGetValue(m.SensorId, out var existingSensorActorRef))
                    {
                        existingSensorActorRef.Forward(m);
                    }
                    else
                    {
                        var newSensorActor = Context.ActorOf(TemperatureSensor.Props(_floorId, m.SensorId),
                            $"temperature-sensor-{m.SensorId}");
                        Context.Watch(newSensorActor);
                        _sensorIdToActorRefs.Add(m.SensorId, newSensorActor);
                        newSensorActor.Forward(m);
                    }
                    break;
                case RequestTemperatureSensorIds m:
                    Sender.Tell(new RespondTemperatureSensorIds(m.RequestId, ImmutableHashSet.CreateRange(_sensorIdToActorRefs.Keys)));
                    break;
                case Terminated m:
                    var terminatedTemperatureSensorId = _sensorIdToActorRefs.First(x => x.Value == m.ActorRef).Key;
                    _sensorIdToActorRefs.Remove(terminatedTemperatureSensorId);
                    break;
                default:
                    Unhandled(message);
                    break;
            }
        }

        public static Props Props(string floorId) => Akka.Actor.Props.Create(() => new Floor(floorId));
    }
}
