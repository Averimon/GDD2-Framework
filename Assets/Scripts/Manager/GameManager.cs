using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Framework.Player;

namespace Framework.Manager
{
    [Serializable]
    public class GameStateEvent : UnityEvent<GameState>{ }

    public enum GameState
    {
        Preperation,
        Running,
        GameOver
    }

    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        private List<PlayerInteractionController> _players;
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

        public int AlivePlayerCount => _players.FindAll(player => player.PlayerState == PlayerState.Alive).Count;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            GameState = GameState.Preperation;
            RegisterPlayers();
            GameState = GameState.Running;
        }

        private void Update()
        {
            if (AlivePlayerCount <= 1)
            {
                GameState = GameState.GameOver;
            }
        }

        public void RegisterPlayers()
        {
            _players = new List<PlayerInteractionController>(FindObjectsOfType<PlayerInteractionController>());
        }

    }
}
