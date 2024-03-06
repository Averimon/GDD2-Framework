using UnityEngine;

namespace Framework.Bomb
{
    public class Bomb : MonoBehaviour
    {
        public int authorID;

        [SerializeField] private GameObject _explosionPrefab;
        [SerializeField] private float _fuseTimer;

        private void Awake()
        {
            Invoke(nameof(Explode), _fuseTimer);
        }

        private void Explode()
        {
            Vector3 updatedPosition = new Vector3(transform.position.x, 0f, transform.position.z);

            GameObject explosionObj =  Instantiate(_explosionPrefab, updatedPosition, Quaternion.identity);
            GameObject explosionMarkObj = explosionObj.GetComponent<Explosion>().ExplosionMarkPrefab;

            if (explosionMarkObj) explosionMarkObj.GetComponent<ExplosionMark>().authorID = authorID;
            Destroy(gameObject);
        }
    }
}
