using UnityEngine;
using Framework.Bomb;
using System.Collections.Generic;

namespace Framework.Player
{
    public class Player : MonoBehaviour
    {
        public PlayerStateEvent OnPlayerStateChanged;

        public int PlayerID = -1;
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
            BombController bombController = GetComponent<BombController>();

            playerMovementController.initialMoveSpeed = _playerRole.moveSpeed;

            bombController.bombPrefab = _playerRole.bombPrefab;
            bombController.bombRechargeTime = _playerRole.bombRechargeTime;

            PlayerAnimator = GetComponentInChildren<Animator>();
        }

        public void Die()
        {
            PlayerState = PlayerState.Dead;
            PlayerAnimator.SetTrigger("Die");
            GetComponent<PlayerMovementController>().enabled = false;
            GetComponent<BombController>().enabled = false;
        }

        public void SwitchPlayerModel(GameObject playerModel)
        {
            if (transform.childCount > 0)
            {
                // If the player model has already been swapped, destroy the old model
                Destroy(transform.GetChild(0).gameObject);
            }
            else
            {
                // First time swapping model, disable the default model
                GetComponent<MeshRenderer>().enabled = false;
            }

            Instantiate(playerModel, transform);
        }
    }
}
