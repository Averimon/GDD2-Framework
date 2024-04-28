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
            OnGameStateChanged.AddListener(HandleGameStateChanged);
        }

        private void OnPlayerCountChanged(int playerCount)
        {
            if (playerCount <= 1)
            {
                GameState = GameState.GameOver;
            }
        }

        private void HandleGameStateChanged(GameState gameState)
        {
            if (gameState == GameState.GameOver)
            {
                foreach (Player.Player player in PlayerManager.Instance.Players)
                {
                    if (player.PlayerState == Player.PlayerState.Dead)
                    {
                        Destroy(player.gameObject);
                    }
                }
                
                SceneManager.Instance.LoadScene("GameOver");
            }
        }
    }
}
