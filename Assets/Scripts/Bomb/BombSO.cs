using UnityEngine;

namespace Framework.Bomb
{
    
    [CreateAssetMenu(fileName = "BombSO", menuName = "BombSO", order = 0)]
    public class BombSO : ScriptableObject
    {
        public GameObject prefab;
        public float explosionCooldown { get => prefab.GetComponent<Bomb>().explosionCooldown;}
        public float explosionRadius { get => prefab.GetComponent<Bomb>().explosionRadius;}
    }
}
