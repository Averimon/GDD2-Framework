using UnityEngine;

namespace Framework.Player
{
    [CreateAssetMenu(fileName = "PlayerRole", menuName = "Player/PlayerRole")]
    public class PlayerRole : ScriptableObject
    {
        [Header("Player")]
        public GameObject playerModel;
        public float moveSpeed = 3f;

        [Header("Bomb")]
        public GameObject bombPrefab;
        public int maxBombCount = 2;
        public float bombPushForce = 3f;
    }
}
