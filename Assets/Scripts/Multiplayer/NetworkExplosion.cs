using Framework.Bomb;
using Mirror;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.Multiplayer
{
    public class NetworkExplosion : NetworkBehaviour
    {
        public GameObject ExplosionMarkPrefab;

        [SerializeField] private float _explosionRadius;
        [SerializeField] private bool _ignoresWall;
        [SerializeField] private LayerMask _colliderLayerMask;

        private float _explosionLifetime;
        private int _playerHits;

        private Transform _airGrid;

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
            List<Transform> hitObjects = new List<Transform>();
            List<Transform> hitPlayers = new List<Transform>();

            _airGrid = GameObject.Find("AirGrid").transform;

            // Check if the objects are destructible or players
            foreach (Collider collider in colliders)
            {
                // Check if the object is in line of sight (not covered)
                RaycastHit[] hits = new RaycastHit[0];

                if (!_ignoresWall)
                {
                    Vector3 direction = (collider.transform.position - transform.position).normalized;
                    float distance = Vector3.Distance(transform.position, collider.transform.position);
                    hits = Physics.RaycastAll(transform.position, direction, distance, ~Physics.IgnoreRaycastLayer, QueryTriggerInteraction.Ignore);
                }

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
                    GameObject airPrefab = Resources.Load<GameObject>("Models/Environment/Air");

                    if (collider.CompareTag("Destructible"))
                    {
                        hitObjects.Add(collider.transform);
                        Destroy(collider.gameObject);

                        GameObject air = Instantiate(airPrefab, collider.transform.position, Quaternion.identity);
                        air.transform.SetParent(_airGrid);
                    }
                    else if (collider.CompareTag("Player"))
                    {
                        hitPlayers.Add(collider.transform);
                        _playerHits++;
                    }
                    else if (collider.CompareTag("Air"))
                    {
                        hitObjects.Add(collider.transform);
                    }
                    // TODO: Add ability to bomb explosion marks
                }
            }

            if (ExplosionMarkPrefab != null)
            {
                ApplyExplosionMarks(hitObjects, hitPlayers);
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

        public void ApplyExplosionMarks(List<Transform> hitObjects, List<Transform> hitPlayers)
        {
            Transform marksGrid = GameObject.Find("MarksGrid").transform;
            ExplosionMark _explosionMark = ExplosionMarkPrefab.GetComponent<ExplosionMark>();

            foreach (Transform hitObject in hitObjects)
            {
                if (_explosionMark.isOnGround)
                {
                    Vector3 updatedPosition = new Vector3(hitObject.position.x, 0f, hitObject.position.z);
                    float randomLifetime = Random.Range(_explosionMark.lifetime - _explosionMark.lifetimeEpsilon, _explosionMark.lifetime + _explosionMark.lifetimeEpsilon);

                    GameObject mark = Instantiate(ExplosionMarkPrefab, updatedPosition, Quaternion.identity);
                    ExplosionMark explosionMark = mark.GetComponent<ExplosionMark>();

                    mark.transform.SetParent(marksGrid);
                    explosionMark.ToBeDestroyed(randomLifetime);
                }
            }
        }
    }
}