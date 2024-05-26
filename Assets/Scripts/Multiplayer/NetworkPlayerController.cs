using Mirror;
using UnityEngine;

namespace Framework.Multiplayer
{
    public class NetworkPlayerController : NetworkBehaviour
    {
        public float initialMoveSpeed = 3;
        public float currentMoveSpeed;
        public float slidingFactor;
        public float directionChangeSpeed;

        private Player.Player _player;
        private CharacterController _controller;
        private Vector3 _targetDirection;
        private Vector3 _currentVelocity;

        public override void OnStartLocalPlayer()
        {
            _player = GetComponent<Player.Player>();
            _player.PlayerAnimator.SetBool("IsInGame", true);

            _controller = GetComponent<CharacterController>();
            _controller.enabled = true;

            currentMoveSpeed = initialMoveSpeed;
            slidingFactor = 0.0f;
            directionChangeSpeed = 100.0f;
            _targetDirection = Vector3.zero;
        }

        private void Update()
        {
            if (!isLocalPlayer || _player == null || _controller == null || !_controller.enabled)
                return;

            _targetDirection.x = Input.GetAxisRaw($"Horizontal P1");
            _targetDirection.z = Input.GetAxisRaw($"Vertical P1");
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
                Vector3 desiredVelocity = _targetDirection * currentMoveSpeed;
                _currentVelocity = Vector3.MoveTowards(_currentVelocity, desiredVelocity, directionChangeSpeed * Time.fixedDeltaTime);
            }
            else
            {
                if (slidingFactor > 0.0f)
                {
                    _currentVelocity *= slidingFactor;
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