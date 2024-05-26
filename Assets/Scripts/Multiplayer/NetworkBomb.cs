using Mirror;
using UnityEngine;
using UnityEngine.Events;
using Framework.Bomb;

namespace Framework.Multiplayer
{
    public class NetworkBomb : NetworkBehaviour
    {
        [SyncVar]
        public int authorID;
        
        public float fuseTimer;

        [SerializeField] private GameObject _explosionPrefab;

        public UnityEvent OnBombExploded;

        public void Start()
        {
            Invoke(nameof(CmdExplode), fuseTimer);
        }

        [Command]
        private void CmdExplode()
        {
            Vector3 updatedPosition = new Vector3(transform.position.x, 0f, transform.position.z);

            GameObject explosionObj = Instantiate(_explosionPrefab, updatedPosition, Quaternion.identity);
            NetworkServer.Spawn(explosionObj, connectionToClient);

            GameObject explosionMarkObj = explosionObj.GetComponent<NetworkExplosion>().ExplosionMarkPrefab;
            if (explosionMarkObj) explosionMarkObj.GetComponent<ExplosionMark>().authorID = authorID;

            OnBombExploded?.Invoke();
            NetworkServer.Destroy(gameObject);
        }
    }
}
