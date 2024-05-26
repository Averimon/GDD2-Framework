using Mirror;
using UnityEngine;
using Framework.Player;
using Framework.Bomb;

namespace Framework.Multiplayer
{
    [RequireComponent(typeof(Player.Player), typeof(IPlayerMovement))]
    public class NetworkPlayerInteractionController : NetworkBehaviour, IPlayerInteraction
    {
        private Player.Player _player;
        private IPlayerMovement _playerMovement;
        private int _activeBombs;

        public bool Enabled
        {
            get => enabled;
            set => enabled = value;
        }

        public void Start()
        {
            _player = GetComponent<Player.Player>();
            _playerMovement = GetComponent<IPlayerMovement>();
        }

        private void Update()
        {
            if (!isLocalPlayer || _player == null)
                return;

            if (Input.GetButtonDown($"Action P1"))
            {
                CmdDropBomb();
            }
        }

        [Command]
        private void CmdDropBomb()
        {
            int maxBombCount = _player.PlayerRole.maxBombCount;
            if (_activeBombs >= maxBombCount) return;

            var bombPrefab = _player.PlayerRole.bombPrefab;
            var bombPosition = new Vector3(transform.position.x, 0.25f, transform.position.z);
            var bombObj = Instantiate(bombPrefab, bombPosition, Quaternion.identity);

            NetworkServer.Spawn(bombObj, connectionToClient);

            var bomb = bombObj.GetComponent<NetworkBomb>();
            bomb.authorID = _player.PlayerID;

            bomb.OnBombExploded.AddListener(() => _activeBombs--);
            _activeBombs++;
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
                    if (TryGetComponent<NetworkAnimator>(out var animator))
                        animator.SetTrigger("Die");

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

                if (_player.PlayerID != hit.collider.GetComponent<NetworkBomb>().authorID) return;
                if (body == null || body.isKinematic) return;

                float bombPushForce = _player.PlayerRole.bombPushForce;

                Vector3 direction = hit.moveDirection.normalized;
                direction.y = 0;

                body.AddForce(direction * bombPushForce, ForceMode.Impulse);
            }
        }
    }
}
