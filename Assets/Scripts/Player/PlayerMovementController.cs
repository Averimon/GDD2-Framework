using UnityEngine;

namespace Framework.Player
{
    [RequireComponent(typeof(Player))]
    public class PlayerMovementController : MonoBehaviour
    {
        public float initialMoveSpeed;
        public float currentMoveSpeed;
        public float slidingFactor;
        public float directionChangeSpeed;

        private Player _player;
        private CharacterController _controller;
        private Vector3 _targetDirection;
        private Vector3 _currentVelocity;

        private void Start()
        {
            _player = GetComponent<Player>();
            _controller = GetComponent<CharacterController>();

            currentMoveSpeed = initialMoveSpeed;
            slidingFactor = 0.0f;
            directionChangeSpeed = 100.0f;
            _targetDirection = Vector3.zero;
        }

        private void Update()
        {
            _targetDirection.x = Input.GetAxisRaw($"Horizontal P{_player.PlayerID}");
            _targetDirection.z = Input.GetAxisRaw($"Vertical P{_player.PlayerID}");
            _targetDirection.Normalize();

            bool isMoving = _targetDirection.magnitude != 0;
            _player.PlayerAnimator.SetBool("IsMoving", isMoving);
        }
        
        private void FixedUpdate()
        {
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
