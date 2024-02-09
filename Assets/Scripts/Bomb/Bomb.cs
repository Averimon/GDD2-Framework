using UnityEngine;

namespace Framework.Bomb
{
    public class Bomb : MonoBehaviour
    {
        [SerializeField] private float _explosionDuration;
        [SerializeField] private float _explosionRadius;
        [SerializeField] private Explosion explosion;

        private void Awake()
        {
            Invoke(nameof(Explode), _explosionDuration);
        }

        private void Explode()
        {
            Debug.Log("Boom!");
            Destroy(gameObject);
        }
    }
}