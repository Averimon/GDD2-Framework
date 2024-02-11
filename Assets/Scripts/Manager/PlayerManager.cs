using System.Collections.Generic;
using Framework.Bomb;
using Framework.Player;
using UnityEditor.SearchService;
using UnityEngine;

namespace Framework.Manager
{
    public class PlayerManager : MonoBehaviour
    {
        public static PlayerManager Instance { get; private set; }
        public List<Player.Player> Players => new List<Player.Player>(_playersConfirmation.Keys);

        private Dictionary<Player.Player, bool> _playersConfirmation = new Dictionary<Player.Player, bool>();
        private int _alivePlayerCount;

        public PlayerCountEvent OnPlayerCountChanged;

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

        public void RegisterPlayer(PlayerRole playerRole)
        {
            Debug.Log("Registering player with role: " + playerRole.name);
            
            GameObject playerObj = Instantiate(Resources.Load("Player/Player")) as GameObject;
            Player.Player player = playerObj.GetComponent<Player.Player>();

            player.PlayerRole = playerRole;

            playerObj.GetComponent<Rigidbody>().isKinematic = true;
            playerObj.GetComponent<BombController>().bombPrefab = playerRole.bombPrefab;

            _playersConfirmation.Add(player, false);
            player.OnPlayerStateChanged.AddListener(OnPlayerStateChanged);
            AlivePlayerCount++;
        }

        public void SetPlayerConfirmation(Player.Player player, bool confirmation)
        {
            _playersConfirmation[player] = confirmation;

            if (!_playersConfirmation.ContainsValue(false))
            {
                SceneManager.Instance.LoadScene("GameScene");
            }
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
