using UnityEngine;

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
                    transform.rotation = Quaternion.LookRotation(collider.transform.position - transform.position);
                }
            }
        }
    }
}
