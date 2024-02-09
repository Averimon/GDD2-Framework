using UnityEngine;

namespace Framework.Bomb
{
    public class Explosion : MonoBehaviour
    {
        [SerializeField] private float _explosionRadius;
        private float _explosionLifetime;
        private int _playerHits;
        // Layer mask for the explosion collider (only objects on Layer 8 "Reactive" are hit by the explosion)
        private int _colliderLayerMask = 1 << 8;

        private void Awake()
        {
            // Get the longest lifetime of a particle
            GetExplosionLifetime();

            // Set the explosion radius for the sphere collider
            GetComponent<SphereCollider>().radius = _explosionRadius;
        }

        private void Start()
        {
            // Find all objects in the explosion radius
            Collider[] colliders = Physics.OverlapSphere(transform.position, _explosionRadius, _colliderLayerMask, QueryTriggerInteraction.Ignore);
            
            // Check if the objects are destructible or players
            foreach (Collider collider in colliders)
            {
                // Check if the object is in line of sight (not covered)
                Vector3 direction = (collider.transform.position - transform.position).normalized;
                float distance = Vector3.Distance(transform.position, collider.transform.position);
                RaycastHit[] hits = Physics.RaycastAll(transform.position, direction, distance, ~Physics.IgnoreRaycastLayer, QueryTriggerInteraction.Ignore);
                
                // Check if the object is covered by an indestructible object
                bool foundTag = false;
                foreach (RaycastHit hit in hits)
                {
                    if (hit.collider.CompareTag("Indestructible"))
                    {
                        foundTag = true;
                        break;
                    }
                }

                // If the object is not covered, register a hit
                if (!foundTag)
                {
                    if (collider.CompareTag("Destructible"))
                    {
                        Destroy(collider.gameObject);
                    }
                    else if (collider.CompareTag("Player"))
                    {
                        _playerHits++;
                    }
                }
            }

            // Check if players are even hit by the explosion
            CheckPlayerHits();

            // Destroy explosion object on particle completion
            Destroy(gameObject, _explosionLifetime);
        }

        private void OnTriggerEnter(Collider collider)
        {
            if (collider.CompareTag("Player"))
            {
                // Check if the player is in line of sight (not covered)
                bool isCovered = Physics.Linecast(transform.position, collider.transform.position, out RaycastHit hitInfo, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore);

                if (!isCovered || !hitInfo.collider.CompareTag("Indestructible"))
                {
                    RegisterPlayerHit();
                }
            }
        }

        public void RegisterPlayerHit()
        {
            _playerHits--;
            CheckPlayerHits();
        }

        private void CheckPlayerHits()
        {
            if (_playerHits == 0)
            {
                Debug.Log("All players have been hit by the explosion.");
                GetComponent<SphereCollider>().enabled = false;
            }
            else if (_playerHits < 0)
            {
                Debug.LogError("There are more players registering a hit than actually hit by the explosion.");
            }
        }

        private void GetExplosionLifetime()
        {
            ParticleSystem[] particleSystems = GetComponentsInChildren<ParticleSystem>();
            foreach (ParticleSystem particleSystem in particleSystems)
            {
                if (particleSystem.main.startLifetime.constant > _explosionLifetime)
                {
                    _explosionLifetime = particleSystem.main.startLifetime.constant;
                }
            }
        }
    }
}
