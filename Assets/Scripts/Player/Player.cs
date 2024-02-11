using UnityEngine;
using Framework.Bomb;

namespace Framework.Player
{
    public class Player : MonoBehaviour
    {
        private PlayerRole _playerRole;
        private PlayerState _playerState;

        public PlayerStateEvent OnPlayerStateChanged;

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
                ApplyRole();
            }
        }

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }

        private void ApplyRole()
        {
            PlayerMovementController playerMovementController = GetComponent<PlayerMovementController>();
            BombController bombController = GetComponent<BombController>();

            playerMovementController.moveSpeed = _playerRole.moveSpeed;

            bombController.bombPrefab = _playerRole.bombPrefab;
            bombController.bombRechargeTime = _playerRole.bombRechargeTime;
        }
    }
}
