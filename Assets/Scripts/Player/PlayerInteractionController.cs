using UnityEngine;
using Framework.Bomb;

namespace Framework.Player
{
    public class PlayerInteractionController : MonoBehaviour
    {
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
                // TODO: Check if player has that kind of bomb -> ignores effect
                ExplosionMark explosionMark = collider.GetComponent<ExplosionMark>();
                GetComponent<Player>().explosionMarksAffectingPlayer.Add(explosionMark);

                if (explosionMark.isSticky)
                {
                    GetComponent<PlayerMovementController>().currentMoveSpeed = GetComponent<PlayerMovementController>().initialMoveSpeed * 0.5f;
                }

                // TODO: Add slippery effect
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
            }
        }
    }
}
