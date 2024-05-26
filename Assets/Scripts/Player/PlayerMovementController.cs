using UnityEngine;

namespace Framework.Player
{
    [RequireComponent(typeof(Player))]
    public class PlayerMovementController : MonoBehaviour, IPlayerMovement
    {
        private Player _player;
        private CharacterController _controller;
        private Vector3 _targetDirection;
        private Vector3 _currentVelocity;

        public float InitialMoveSpeed { get; set; }
        public float CurrentMoveSpeed { get; set; }
        public float SlidingFactor { get; set; }
        public float DirectionChangeSpeed { get; set; }


        private void Start()
        {
            _player = GetComponent<Player>();
            _controller = GetComponent<CharacterController>();

            CurrentMoveSpeed = InitialMoveSpeed;
            SlidingFactor = 0.0f;
            DirectionChangeSpeed = 100.0f;
            _targetDirection = Vector3.zero;
        }

        private void Update()
        {
            if (_player == null || _controller == null || !_controller.enabled)
                return;

            _targetDirection.x = Input.GetAxisRaw($"Horizontal P{_player.PlayerID}");
            _targetDirection.z = Input.GetAxisRaw($"Vertical P{_player.PlayerID}");
            _targetDirection.Normalize();

            bool isMoving = _targetDirection.magnitude != 0;
            _player.PlayerAnimator.SetBool("IsMoving", isMoving);
        }
        
        private void FixedUpdate()
        {
            if (_player == null || _controller == null || !_controller.enabled)
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
