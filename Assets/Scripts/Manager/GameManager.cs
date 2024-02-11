using UnityEngine;
using Framework.Player;
using System.Collections.Generic;

namespace Framework.Manager
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        // This is a dictionary of spawn points and whether they are occupied or not
        [SerializeField] private List<Transform> _spawnPoints;

        private Dictionary<Transform, bool> _spawnPointsOccupancy = new Dictionary<Transform, bool>();

        private GameState _gameState;

        public GameStateEvent OnGameStateChanged;

        public GameState GameState
        {
            get => _gameState;
            set
            {
                _gameState = value;
                OnGameStateChanged?.Invoke(_gameState);
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

        private void Start()
        {
            GameState = GameState.Preperation;
            PlayerManager.Instance.OnPlayerCountChanged.AddListener(OnPlayerCountChanged);
            SetSpawnPointsOccupancy();
            SpawnPlayers(PlayerManager.Instance.Players);
            GameState = GameState.Running;
        }

        private void SetSpawnPointsOccupancy()
        {
            foreach (Transform spawnPoint in _spawnPoints)
            {
                _spawnPointsOccupancy.Add(spawnPoint, false);
            }
        }

        private void SpawnPlayers(List<Player.Player> players)
        {
            foreach (Player.Player player in players)
            {
                player.GetComponent<Rigidbody>().isKinematic = false;
                player.transform.position = GetRandomSpawnPoint();
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

        private void OnPlayerCountChanged(int playerCount)
        {
            if (playerCount == 1)
            {
                GameState = GameState.GameOver;
            }
            else if (playerCount == 0)
            {
                Debug.LogError("All players seem to be dead. This should never happen!");
            }
        }
    }
}
