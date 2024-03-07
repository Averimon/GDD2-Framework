using UnityEngine;
using UnityEngine.Events;

namespace Framework.Bomb
{
    public class Bomb : MonoBehaviour
    {
        public int authorID;
        public float fuseTimer;

        [SerializeField] private GameObject _explosionPrefab;

        public UnityEvent OnBombExploded;

        public void DropBomb()
        {
            Invoke(nameof(Explode), fuseTimer);
        }

        private void Explode()
        {
            Vector3 updatedPosition = new Vector3(transform.position.x, 0f, transform.position.z);

            GameObject explosionObj =  Instantiate(_explosionPrefab, updatedPosition, Quaternion.identity);
            GameObject explosionMarkObj = explosionObj.GetComponent<Explosion>().ExplosionMarkPrefab;

            if (explosionMarkObj) explosionMarkObj.GetComponent<ExplosionMark>().authorID = authorID;
            OnBombExploded?.Invoke();
            Destroy(gameObject);
        }
    }
}
