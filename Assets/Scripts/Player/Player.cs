using UnityEngine;
using Framework.Bomb;
using System.Collections.Generic;

namespace Framework.Player
{
    public class Player : MonoBehaviour
    {
        public PlayerStateEvent OnPlayerStateChanged;

        public int PlayerID = -1;
        // List of explosion marks that are affecting the player
        // Checks if the player is still effected by an explosion mark (of same type) when leaving another mark
        // Used for checks on effect removal
        public List<ExplosionMark> explosionMarksAffectingPlayer = new List<ExplosionMark>();

        public Animator PlayerAnimator { get; private set; }

        private PlayerRole _playerRole;
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

        public PlayerRole PlayerRole
        {
            get => _playerRole;
            set
            {
                _playerRole = value;
                if (_playerRole) ApplyRole();
            }
        }
        
        private void ApplyRole()
        {
            PlayerMovementController playerMovementController = GetComponent<PlayerMovementController>();

            playerMovementController.initialMoveSpeed = _playerRole.moveSpeed;
            PlayerAnimator = GetComponentInChildren<Animator>();
        }

        public void Die()
        {
            PlayerState = PlayerState.Dead;
            PlayerAnimator.SetTrigger("Die");
            GetComponent<PlayerMovementController>().enabled = false;
            GetComponent<PlayerInteractionController>().enabled = false;
            GetComponent<CharacterController>().enabled = false;
        }

        public void SwitchPlayerModel(GameObject playerModel)
        {
            // Destroy current player avatar selection
            Transform avatar = transform.GetChild(0);
            foreach (Transform avatarContent in avatar.transform)
                Destroy(avatarContent.gameObject);

            // Spawn the new player avatar model
            Instantiate(playerModel, avatar);
        }
    }
}
