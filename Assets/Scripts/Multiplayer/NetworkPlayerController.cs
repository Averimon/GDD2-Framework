using Mirror;
using UnityEngine;
using Framework.Manager;
using Framework.Player;

namespace Framework.Multiplayer
{
    public class NetworkPlayerController : NetworkBehaviour, IPlayerMovement
    {
        private Player.Player _player;
        private CharacterController _controller;
        private Vector3 _targetDirection;
        private Vector3 _currentVelocity;

        public float InitialMoveSpeed { get; set; } = 3;
        public float CurrentMoveSpeed { get; set; }
        public float SlidingFactor { get; set; }
        public float DirectionChangeSpeed { get; set; }

        public override void OnStartLocalPlayer()
        {
            _player = GetComponent<Player.Player>();
            _player.PlayerID = 1;
            _player.PlayerAnimator.SetBool("IsInGame", true);
            PlayerManager.Instance.AddPlayer(_player);

            _controller = GetComponent<CharacterController>();
            _controller.enabled = true;

            CurrentMoveSpeed = InitialMoveSpeed;
            SlidingFactor = 0.0f;
            DirectionChangeSpeed = 100.0f;
            _targetDirection = Vector3.zero;
        }

        private void Update()
        {
            if (!isLocalPlayer || _player == null || _controller == null || !_controller.enabled)
                return;

            _targetDirection.x = Input.GetAxisRaw($"Horizontal P{_player.PlayerID}");
            _targetDirection.z = Input.GetAxisRaw($"Vertical P{_player.PlayerID}");
            _targetDirection.Normalize();

            bool isMoving = _targetDirection.magnitude != 0;
            _player.PlayerAnimator.SetBool("IsMoving", isMoving);
        }
        private void FixedUpdate()
        {
            if (!isLocalPlayer || _player == null || _controller == null || !_controller.enabled)
                return;

            if (_targetDirection.magnitude != 0)
            {
                Vector3 desiredVelocity = _targetDirection * CurrentMoveSpeed;
                _currentVelocity = Vector3.MoveTowards(_currentVelocity, desiredVelocity, DirectionChangeSpeed * Time.fixedDeltaTime);
            }
            else
            {
                if (SlidingFactor > 0.0f)
                {
                    _currentVelocity *= SlidingFactor;
                }
                else
                {
                    _currentVelocity = Vector3.zero;
                }
            }

            if (_currentVelocity.magnitude != 0)
            {
                _controller.Move(_currentVelocity * Time.fixedDeltaTime);
            }

            if (_targetDirection != Vector3.zero)
            {
                transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(_targetDirection), 800 * Time.fixedDeltaTime);
            }
        }
    }
}