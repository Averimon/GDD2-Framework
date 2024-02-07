using UnityEngine;

namespace Framework.Bomb
{
    public class Bomb : MonoBehaviour
    {
        [SerializeField] private float _explosionCooldown;
        [SerializeField] private float _explosionRadius;

        public float explosionCooldown { get => _explosionCooldown; }
        public float explosionRadius { get => _explosionRadius; }

        private void Awake()
        {
            Invoke(nameof(Explode), _explosionCooldown);
        }

        private void Explode()
        {
            Debug.Log("Boom!");
            Destroy(gameObject);
        }
    }
}