using UnityEngine;
using Framework.Bomb;
using UnityEngine.SceneManagement;

namespace Framework.Player
{
    public class PlayerInteractionController : MonoBehaviour
    {
        private bool wasPressedLastFrame = false;
        private int _activeBombs;

        private void Update()
        {
            if (Input.GetButtonDown("Action P" + (GetComponent<Player>().PlayerID + 1)))
            {
                if (SceneManager.GetActiveScene().name == "GameScene")
                {
                    DropBomb();
                }
                else if (SceneManager.GetActiveScene().name == "SelectionScene")
                {
                    GetComponentInParent<PlayerSelection>().TogglePlayerConfirmation();
                }
            }
            else if (SceneManager.GetActiveScene().name == "SelectionScene")
            {
                float horizontalInput = Input.GetAxisRaw("Horizontal P" + (GetComponent<Player>().PlayerID + 1));
                // Check if the axis just transitioned from not pressed to pressed
                if (!wasPressedLastFrame && horizontalInput != 0)
                {
                    if (horizontalInput > 0)
                    {
                        GetComponentInParent<PlayerSelection>().GoRight();
                    }
                    else
                    {
                        GetComponentInParent<PlayerSelection>().GoLeft();
                    }
                    wasPressedLastFrame = true;
                }
                else if (horizontalInput == 0)
                {
                    wasPressedLastFrame = false;
                }
            }
        }

        private void DropBomb()
        {
            int maxBombCount = GetComponent<Player>().PlayerRole.maxBombCount;
            if (_activeBombs >= maxBombCount) return;

            GameObject bombPrefab = GetComponent<Player>().PlayerRole.bombPrefab;
            GameObject bombObj = Instantiate(bombPrefab, transform.position, Quaternion.identity);
            Bomb.Bomb bomb = bombObj.GetComponent<Bomb.Bomb>();
            
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
                    GetComponent<Player>().Die();

                    Vector3 deathLookRotation = collider.transform.position - transform.position;

                    transform.rotation = Quaternion.LookRotation(deathLookRotation);
                    transform.eulerAngles = new Vector3(0f, transform.rotation.y, 0f);
                }
            }
            else if (collider.CompareTag("ExplosionMark"))
            {
                if (GetComponent<Player>().PlayerID == collider.GetComponent<ExplosionMark>().authorID) return;

                ExplosionMark explosionMark = collider.GetComponent<ExplosionMark>();
                GetComponent<Player>().explosionMarksAffectingPlayer.Add(explosionMark);
                PlayerMovementController playerMovementController = GetComponent<PlayerMovementController>();

                if (explosionMark.isSticky)
                {
                    playerMovementController.currentMoveSpeed = playerMovementController.initialMoveSpeed * 0.5f;
                }
                if (explosionMark.isSlippery)
                {
                    playerMovementController.slidingFactor = 0.97f;
                    playerMovementController.directionChangeSpeed = 5f;
                }
            }
        }

        public void OnTriggerExit(Collider collider)
        {
            if (collider.CompareTag("ExplosionMark"))
            {
                ExplosionMark explosionMark = collider.GetComponent<ExplosionMark>();
                GetComponent<Player>().explosionMarksAffectingPlayer.Remove(explosionMark);

                if (explosionMark.isSticky)
                {
                    if (GetComponent<Player>().explosionMarksAffectingPlayer.Find(mark => mark.isSticky) == null)
                    {
                        GetComponent<PlayerMovementController>().currentMoveSpeed = GetComponent<PlayerMovementController>().initialMoveSpeed;
                    }
                }
                if (explosionMark.isSlippery)
                {
                    if (GetComponent<Player>().explosionMarksAffectingPlayer.Find(mark => mark.isSlippery) == null)
                    {
                        GetComponent<PlayerMovementController>().slidingFactor = 0.0f;
                        GetComponent<PlayerMovementController>().directionChangeSpeed = 100.0f;
                    }
                }
            }
            else if (collider.CompareTag("Bomb"))
            {
                SphereCollider explosionCollider = collider.GetComponent<SphereCollider>();
                explosionCollider.isTrigger = false;
            }
        }

        private void OnControllerColliderHit(ControllerColliderHit hit)
        {
            if (hit.collider.CompareTag("Bomb"))
            {
                Rigidbody body = hit.collider.attachedRigidbody;

                if (GetComponent<Player>().PlayerID != hit.collider.GetComponent<Bomb.Bomb>().authorID) return;
                if (body == null || body.isKinematic) return;

                float bombPushForce = GetComponent<Player>().PlayerRole.bombPushForce;

                Vector3 direction = hit.moveDirection.normalized;
                direction.y = 0;

                body.AddForce(direction * bombPushForce, ForceMode.Impulse);
            }
        }
    }
}
