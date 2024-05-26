using UnityEngine;
using Framework.Bomb;

namespace Framework.Player
{
    [RequireComponent(typeof(Player), typeof(PlayerMovementController))]
    public class PlayerInteractionController : MonoBehaviour
    {
        private Player _player;
        private IPlayerMovement _playerMovement;
        private int _activeBombs;

        private void Start()
        {
            _player = GetComponent<Player>();
            _playerMovement = GetComponent<IPlayerMovement>();
        }

        private void Update()
        {
            if (Input.GetButtonDown($"Action P{_player.PlayerID}"))
            {
                DropBomb();
            }
        }

        private void DropBomb()
        {
            int maxBombCount = _player.PlayerRole.maxBombCount;
            if (_activeBombs >= maxBombCount) return;

            GameObject bombPrefab = _player.PlayerRole.bombPrefab;
            Vector3 bombPosition = new Vector3(transform.position.x, 0.25f, transform.position.z);
            GameObject bombObj = Instantiate(bombPrefab, bombPosition, Quaternion.identity);
            Bomb.Bomb bomb = bombObj.GetComponent<Bomb.Bomb>();
            bomb.authorID = _player.PlayerID;
            
            bomb.OnBombExploded.AddListener(() => _activeBombs--);
            _activeBombs++;
            
            bomb.DropBomb();
        }

        private void OnTriggerEnter(Collider collider)
        {
            if (collider.CompareTag("Explosion"))
            {
                // Check if the player is in line of sight (not covered)
                bool isCovered = Physics.Linecast(transform.position, collider.transform.position, out RaycastHit hitInfo, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore);

                if (!isCovered || !hitInfo.collider.CompareTag("Indestructible"))
                {
                    _player.Die();

                    Vector3 deathLookRotation = collider.transform.position - transform.position;

                    transform.rotation = Quaternion.LookRotation(deathLookRotation);
                    transform.eulerAngles = new Vector3(0f, transform.rotation.y, 0f);
                }
            }
            else if (collider.CompareTag("ExplosionMark"))
            {
                ExplosionMark explosionMark = collider.GetComponent<ExplosionMark>();
                if (_player.PlayerID == explosionMark.authorID) return;
                
                _player.explosionMarksAffectingPlayer.Add(explosionMark);

                if (explosionMark.isSticky)
                {
                    _playerMovement.CurrentMoveSpeed = _playerMovement.InitialMoveSpeed * 0.5f;
                }
                if (explosionMark.isSlippery)
                {
                    _playerMovement.SlidingFactor = 0.97f;
                    _playerMovement.DirectionChangeSpeed = 5f;
                }
            }
        }

        public void OnTriggerExit(Collider collider)
        {
            if (collider.CompareTag("ExplosionMark"))
            {
                ExplosionMark explosionMark = collider.GetComponent<ExplosionMark>();
                _player.explosionMarksAffectingPlayer.Remove(explosionMark);

                if (explosionMark.isSticky)
                {
                    if (_player.explosionMarksAffectingPlayer.Find(mark => mark.isSticky) == null)
                    {
                        _playerMovement.CurrentMoveSpeed = _playerMovement.InitialMoveSpeed;
                    }
                }
                if (explosionMark.isSlippery)
                {
                    if (_player.explosionMarksAffectingPlayer.Find(mark => mark.isSlippery) == null)
                    {
                        _playerMovement.SlidingFactor = 0.0f;
                        _playerMovement.DirectionChangeSpeed = 100.0f;
                    }
                }
            }
            else if (collider.CompareTag("Bomb"))
            {
                collider.isTrigger = false;
            }
        }

        private void OnControllerColliderHit(ControllerColliderHit hit)
        {
            if (hit.collider.CompareTag("Bomb"))
            {
                Rigidbody body = hit.collider.attachedRigidbody;

                if (_player.PlayerID != hit.collider.GetComponent<Bomb.Bomb>().authorID) return;
                if (body == null || body.isKinematic) return;

                float bombPushForce = _player.PlayerRole.bombPushForce;

                Vector3 direction = hit.moveDirection.normalized;
                direction.y = 0;

                body.AddForce(direction * bombPushForce, ForceMode.Impulse);
            }
        }
    }
}
