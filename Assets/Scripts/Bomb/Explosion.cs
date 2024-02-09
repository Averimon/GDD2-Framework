using UnityEngine;

namespace Framework.Bomb
{
    public class Explosion : MonoBehaviour
    {
        [SerializeField] private float _explosionDuration;

        public float ExplosionDuration { get => _explosionDuration; }
    }
}
