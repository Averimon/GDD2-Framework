using UnityEngine;

namespace Framework.Manager
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        public GameStateEvent OnGameStateChanged;

        private GameState _gameState;

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
            PlayerManager.Instance.InitalizeSpawnPoints();
            PlayerManager.Instance.SpawnPlayers();
            GameState = GameState.Running;
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
