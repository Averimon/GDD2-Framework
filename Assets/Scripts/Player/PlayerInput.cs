using UnityEngine;

namespace Framework.Player
{
    public class PlayerInput : MonoBehaviour
    {
        [Header("Movement Settings")]
        [SerializeField] private Rigidbody _rigidbody;
        [SerializeField] private float _moveSpeed;

        [Header("Player Settings")]
        [SerializeField] private PlayerRoleSO _playerRole;

        private float _horizontalInput;
        private float _verticalInput;

        private void OnEnable()
        {
            _playerRole.OnEnable();
        }

        private void Update()
        {
            _playerRole.Update();

            _horizontalInput = Input.GetAxisRaw("Horizontal");
            _verticalInput = Input.GetAxisRaw("Vertical");

            if (Input.GetKeyDown(KeyCode.Space))
            {
                _playerRole.DropBomb(transform.position);
            }
        }
        
        private void FixedUpdate()
        {
            Vector3 movementDirection = new Vector3(_horizontalInput, 0, _verticalInput).normalized;
            _rigidbody.MovePosition(_rigidbody.position + movementDirection * _moveSpeed * Time.fixedDeltaTime);
        }
    }
}