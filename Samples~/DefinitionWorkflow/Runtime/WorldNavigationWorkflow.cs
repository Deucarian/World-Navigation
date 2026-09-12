using System;
using UnityEngine;

namespace Deucarian.WorldNavigation.Samples.DefinitionWorkflow
{
    /// <summary>Small caller example. The configured scene hosts own services and resource lifetimes.</summary>
    public sealed class WorldNavigationWorkflow : MonoBehaviour
    {
        [SerializeField] private MovementAgentHost agent;
        [SerializeField] private Transform destination;
        [SerializeField] private MovementTrigger trigger;
        private string status = "Ready. Choose an action below.";
        public string Status => status;
        public void Move() { agent.MoveTo(destination.position); status = "Moving using the selected movement preset."; }
        public void MoveComponent() { trigger.Move(); status = "Moving through MovementTrigger."; }
        public void Pause() { agent.Pause(); status = "Paused."; }
        public void Resume() { agent.Resume(); status = "Resumed."; }
        public void Stop() { agent.Stop(); status = "Stopped."; }
        private void OnGUI()
        {
            GUILayout.BeginArea(new Rect(24, 24, Math.Min(540, Screen.width - 48), Screen.height - 48), GUI.skin.box);
            GUILayout.Label("World-Navigation — definition workflow");
            GUILayout.Label("The preset supplies speed; the scene supplies a destination. One WorldNavigationHost ticks every registered agent in this scope.");
            GUILayout.Space(12);
            if (GUILayout.Button("Move with C#", GUILayout.Height(32))) { try { Move(); } catch (Exception error) { status = error.Message; } }
            if (GUILayout.Button("Move with component", GUILayout.Height(32))) { try { MoveComponent(); } catch (Exception error) { status = error.Message; } }
            if (GUILayout.Button("Pause", GUILayout.Height(32))) { try { Pause(); } catch (Exception error) { status = error.Message; } }
            if (GUILayout.Button("Resume", GUILayout.Height(32))) { try { Resume(); } catch (Exception error) { status = error.Message; } }
            if (GUILayout.Button("Stop", GUILayout.Height(32))) { try { Stop(); } catch (Exception error) { status = error.Message; } }
            GUILayout.Space(12);
            GUILayout.Label(status);
            GUILayout.EndArea();
        }
    }
}
