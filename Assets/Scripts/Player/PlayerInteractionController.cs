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

                // Debug.Log(hitInfo.collider.gameObject.name + " is in the way!");
                // Debug.Log(hitInfo.collider.tag);
                if (!isCovered || !hitInfo.collider.CompareTag("Indestructible"))
                {
                    Debug.Log("Player is hit by explosion!");
                    gameObject.GetComponent<Player>().PlayerState = PlayerState.Dead;
                }
                else
                {
                    Debug.Log("Player is safe!");
                }
            }
        }
    }
}
