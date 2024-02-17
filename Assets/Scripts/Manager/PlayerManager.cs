using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using Framework.Player;
using Framework.Bomb;

namespace Framework.Manager
{
    public class PlayerManager : MonoBehaviour
    {
        public static PlayerManager Instance { get; private set; }

        public List<Player.Player> Players { get; private set; } = new List<Player.Player>();
        public PlayerCountEvent OnPlayerCountChanged;

        // This is a dictionary of spawn points and whether they are occupied or not
        private Dictionary<Transform, bool> _spawnPointsOccupancy = new Dictionary<Transform, bool>();
        private int _alivePlayerCount;

        public int AlivePlayerCount
        {
            get => _alivePlayerCount;
            private set
            {
                _alivePlayerCount = value;
                OnPlayerCountChanged?.Invoke(_alivePlayerCount);
            }
        }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
        public void InitalizePlayers(List<Player.Player> players)
        {
            Players.Clear();

            foreach (Player.Player player in players)
            {
                player.OnPlayerStateChanged.AddListener(OnPlayerStateChanged);
                player.transform.SetParent(null);
                DontDestroyOnLoad(player.gameObject);

                Players.Add(player);
                AlivePlayerCount++;
            }
        }

        public void InitalizeSpawnPoints()
        {
            _spawnPointsOccupancy.Clear();
            
            GameObject[] spawnPoints = GameObject.FindGameObjectsWithTag("SpawnPoint");
            foreach (GameObject spawnPoint in spawnPoints)
            {
                _spawnPointsOccupancy.Add(spawnPoint.transform, false);
            }

        }

        public void SpawnPlayers()
        {
            foreach (Player.Player player in Players)
            {
                Vector3 spawnPoint = GetRandomSpawnPoint();
                _spawnPointsOccupancy[_spawnPointsOccupancy.First(x => x.Key.position == spawnPoint).Key] = true;
                
                player.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                player.transform.position = spawnPoint;

                player.GetComponent<Rigidbody>().isKinematic = false;
                player.GetComponent<PlayerMovementController>().enabled = true;
                player.GetComponent<BombController>().enabled = true;

                player.PlayerAnimator.SetBool("IsInGame", true);
            }
        }

        private Vector3 GetRandomSpawnPoint()
        {
            List<Transform> availableSpawnPoints = new List<Transform>();

            foreach (KeyValuePair<Transform, bool> spawnPoint in _spawnPointsOccupancy)
            {
                if (!spawnPoint.Value)
                {
                    availableSpawnPoints.Add(spawnPoint.Key);
                }
            }

            int randomIndex = Random.Range(0, availableSpawnPoints.Count);
            return availableSpawnPoints[randomIndex].position;
        }

        private void OnPlayerStateChanged(PlayerState playerState)
        {
            if (playerState == PlayerState.Dead)
            {
                AlivePlayerCount--;
            }
            else if (playerState == PlayerState.Alive)
            {
                AlivePlayerCount++;
            }
        }
    }
}
