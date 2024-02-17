using System.Collections;
using UnityEngine;

namespace Framework.Bomb
{
    public class ExplosionMark : MonoBehaviour
    {
        [Header("Mark Properties")]
        public bool sticksToObjects;
        public bool sticksToPlayers;
        public bool isOnGround;
        public bool ignitesFire;
        public bool isSticky;
        public bool isSlippery;
        
        public float lifetime = 3f;
        public float lifetimeEpsilon = 0.5f;

        public readonly float destroyingProcess = 0.2f;

        // Move collider to trigger OnTriggerExit event
        public void ToBeDestroyed(float setLifetime)
        {
            StartCoroutine(OnDestroyHandler(setLifetime));
        }

        private IEnumerator OnDestroyHandler(float setLifetime)
        {
            yield return new WaitForSeconds(setLifetime - destroyingProcess);
            transform.position += Vector3.down * 2f;

            yield return new WaitForSeconds(destroyingProcess);
            Destroy(gameObject);
        }
    }
}
