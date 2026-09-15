using System;
using UnityEngine;
namespace Deucarian.WorldNavigation
{
    public sealed class MovementTrigger : MonoBehaviour
    {
        [SerializeField] private MovementAgentHost agent;
        [SerializeField] private Transform destination;
        private MovementAgentHost Agent => agent != null ? agent : throw new InvalidOperationException("Assign a MovementAgentHost to this MovementTrigger.");
        public void Move()
        {
            if (destination == null) throw new InvalidOperationException("Assign a destination Transform to this MovementTrigger.");
            Agent.MoveTo(destination.position);
        }
        public void Stop() => Agent.Stop();
        public void Pause() => Agent.Pause();
        public void Resume() => Agent.Resume();
    }
}
