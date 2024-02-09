using System;
using UnityEngine;
using UnityEngine.Events;

namespace Framework.Player
{
    [Serializable]
    public class PlayerStateEvent : UnityEvent<PlayerState>{ }

    public enum PlayerState
    {
        Alive,
        Dead
    }

    public class PlayerInteractionController : MonoBehaviour
    {
        public PlayerStateEvent OnPlayerStateChanged;

        private PlayerState _playerState;

        public PlayerState PlayerState
        {
            get => _playerState;
            set
            {
                _playerState = value;
                OnPlayerStateChanged?.Invoke(_playerState);
            }
        }

        private void OnTriggerEnter(Collider collider)
        {
            if (collider.CompareTag("Explosion"))
            {
                // Check if the player is in line of sight (not covered)
                bool isCovered = Physics.Linecast(transform.position, collider.transform.position, out RaycastHit hitInfo, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore);

                // Debug.Log(hitInfo.collider.gameObject.name + " is in the way!");
                // Debug.Log(hitInfo.collider.tag);
                if (!isCovered || !hitInfo.collider.CompareTag("Indestructible"))
                {
                    Debug.Log("Player is hit by explosion!");
                    PlayerState = PlayerState.Dead;
                }
                else
                {
                    Debug.Log("Player is safe!");
                }
            }
        }
    }
}
