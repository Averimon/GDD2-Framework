using UnityEngine;

namespace Framework.Player
{
    public class PlayerMovementController : MonoBehaviour
    {
        public float initialMoveSpeed;
        public float currentMoveSpeed;

        [SerializeField] private Rigidbody _rigidbody;

        private float _horizontalInput;
        private float _verticalInput;

        void Start()
        {
            currentMoveSpeed = initialMoveSpeed;
        }

        private void Update()
        {
            _horizontalInput = Input.GetAxisRaw("Horizontal");
            _verticalInput = Input.GetAxisRaw("Vertical");

            bool isMoving = _horizontalInput != 0 || _verticalInput != 0;
            GetComponent<Player>().PlayerAnimator.SetBool("IsMoving", isMoving);
        }
        
        private void FixedUpdate()
        {
            Vector3 movementDirection = new Vector3(_horizontalInput, 0, _verticalInput).normalized;
            _rigidbody.MovePosition(_rigidbody.position + movementDirection * currentMoveSpeed * Time.fixedDeltaTime);
            if (movementDirection != Vector3.zero)
            {
                transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(movementDirection), 800 * Time.fixedDeltaTime);
            }
        }
    }
}
