using UnityEngine;
using TMPro;
using System.Linq;
using UnityEngine.UI;
using System.Collections.Generic;
using Framework.Player;
using Framework.Networking;
using Mirror;

namespace Framework.Manager
{
    public class SelectionHandler : MonoBehaviour
    {
        [SerializeField] private List<PlayerSelection> _playerSelections = new List<PlayerSelection>();
        // Get count of players that have confirmed their roles
        private int ConfirmedPlayerCount => PlayerManager.Instance.Players.Count(p => p.PlayerRole);
        [SerializeField] private TextMeshProUGUI _playerConfirmationText;
        [SerializeField] private Button _startButton;

        private void Awake()
        {
            NetworkManagerExtension networkManager = (NetworkManagerExtension)FindObjectOfType<NetworkManager>();

            if (networkManager)
            {
                Debug.Log("NetworkManager found");
                networkManager.OnPlayerJoin.AddListener(RpcHandlePlayerJoin);
            }
            else
            {
                Debug.LogWarning("NetworkManager not found");
            }
        }

        private void Start()
        {
            if (!NetworkServer.active)
            {
                _startButton.interactable = false;
            }

            foreach (PlayerSelection playerSelection in _playerSelections)
            {
                playerSelection.OnPlayerConfirmed.AddListener(UpdateStartButtonUI);
            }
            
            UpdateStartButtonUI();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space) && NetworkServer.active)
            {
                StartGame();
            }
        }

        // [ClientRpc]
        private void RpcHandlePlayerJoin(int connectionId)
        {
            Player.Player[] players = FindObjectsOfType(typeof(Player.Player)) as Player.Player[];
            Player.Player connPlayer = players.FirstOrDefault(p => p.name.Equals($"Player [connId={connectionId}]"));
            int playerSelectioIndex = players.Length - 1;

            if (connPlayer && connPlayer.GetComponent<NetworkIdentity>().isLocalPlayer)
            {
                _playerSelections[playerSelectioIndex].SetPlayer(connPlayer);
                PlayerManager.Instance.Players.Add(connPlayer);
                UpdateStartButtonUI();

                foreach (PlayerSelection playerSelection in _playerSelections)
                {
                    // Hide selection UI if no player is assigned
                    if (!playerSelection.GetPlayer())
                    {
                        // playerSelection.gameObject.SetActive(false);
                    }
                    // Hide buttons of selection UI not assigned to local player
                    else if (playerSelection != _playerSelections[playerSelectioIndex])
                    {
                        Button[] buttons = playerSelection.GetComponentsInChildren<Button>();
                        foreach (Button button in buttons)
                        {
                            button.gameObject.SetActive(false);
                        }
                    }
                }
            }
        }

        public void StartGame()
        {
            if (ConfirmedPlayerCount.Equals(PlayerManager.Instance.Players.Count))
            {
                PlayerManager.Instance.InitalizePlayers();
                SceneManager.Instance.LoadScene("GameScene");
            }
        }
        
        private void UpdateStartButtonUI()
        {
            int playerCount = PlayerManager.Instance.Players.Count;

            if (ConfirmedPlayerCount.Equals(playerCount))
            {
                _startButton.interactable = NetworkServer.active;
                _playerConfirmationText.text = "All players confirmed";
                _playerConfirmationText.color = Color.green;
            }
            else
            {
                _startButton.interactable = false;
                _playerConfirmationText.text = ConfirmedPlayerCount + " / " + playerCount + " players confirmed";
                _playerConfirmationText.color = Color.red;
            }
        }
    }
}
