using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using Framework.Player;

namespace Framework.Manager
{
    public class PlayerManager : MonoBehaviour
    {
        public static PlayerManager Instance { get; private set; }
        
        public PlayerCountEvent OnPlayerCountChanged;

        public List<Player.Player> Players { get; private set; } = new List<Player.Player>();

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
                Players.AddRange(FindObjectsOfType<Player.Player>());
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
        public List<Player.Player> GetAlivePlayers()
        {
            return Players.Where(player => player.PlayerState == PlayerState.Alive).ToList();
        }

        public void AddPlayer(Player.Player player)
        {
            Players.Add(player);
            InitializePlayer(player);
        }

        private void InitializePlayer(Player.Player player)
        {
            player.OnPlayerStateChanged.AddListener(OnPlayerStateChanged);
            player.transform.SetParent(null);
            _alivePlayerCount++;
        }

        public void InitalizePlayers()
        {
            _spawnPointsOccupancy.Clear();

            foreach (Player.Player player in Players)
            {
                InitializePlayer(player);
                DontDestroyOnLoad(player.gameObject);
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
                
                player.transform.position = spawnPoint;
                player.GetComponent<PlayerMovementController>().enabled = true;
                player.GetComponent<PlayerInteractionController>().enabled = true;

                player.PlayerAnimator.SetBool("IsInGame", true);

                // Disable Selection Cam
                player.GetComponentInChildren<Camera>().enabled = false;

            }
        }

        private Vector3 GetRandomSpawnPoint()
        {
            List<Transform> availableSpawnPoints = (from spawnPoint in _spawnPointsOccupancy
                where !spawnPoint.Value select spawnPoint.Key).ToList();

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
