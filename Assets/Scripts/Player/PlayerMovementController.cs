using UnityEngine;

namespace Framework.Player
{
    public class PlayerMovementController : MonoBehaviour
    {
        [SerializeField] private Rigidbody _rigidbody;
        [SerializeField] private float _moveSpeed;

        private float _horizontalInput;
        private float _verticalInput;

        private void Update()
        {
            _horizontalInput = Input.GetAxisRaw("Horizontal");
            _verticalInput = Input.GetAxisRaw("Vertical");
        }
        
        private void FixedUpdate()
        {
            Vector3 movementDirection = new Vector3(_horizontalInput, 0, _verticalInput).normalized;
            _rigidbody.MovePosition(_rigidbody.position + movementDirection * _moveSpeed * Time.fixedDeltaTime);
        }
    }
}
