using UnityEngine;
using TMPro;
using System.Linq;
using UnityEngine.UI;
using System.Collections.Generic;
using Framework.Player;

namespace Framework.Manager
{
    public class SelectionHandler : MonoBehaviour
    {
        [SerializeField] private List<PlayerSelection> _playerSelections = new List<PlayerSelection>();
        // Get count of players that have confirmed their roles
        private int ConfirmedPlayerCount => PlayerManager.Instance.Players.Count(p => p.PlayerRole);
        [SerializeField] private TextMeshProUGUI _playerConfirmationText;
        [SerializeField] private Button _startButton;

        private void Start()
        {
            foreach (PlayerSelection playerSelection in _playerSelections)
            {
                playerSelection.OnPlayerConfirmed.AddListener(UpdateStartButtonUI);
            }

            UpdateStartButtonUI();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                StartGame();
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
                _startButton.interactable = true;
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
