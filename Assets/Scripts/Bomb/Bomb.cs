using UnityEngine;

namespace Framework.Bomb
{
    public class Bomb : MonoBehaviour
    {
        [SerializeField] private float _fuseTimer;
        [SerializeField] private GameObject _explosionPrefab;

        private void Awake()
        {
            Invoke(nameof(Explode), _fuseTimer);
        }

        private void Explode()
        {
            Instantiate(_explosionPrefab, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
}