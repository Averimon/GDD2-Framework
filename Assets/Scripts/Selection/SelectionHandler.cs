using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

using Framework.Player;
using Framework.Manager;

namespace Framework.Selection
{
    public class SelectionHandler : MonoBehaviour
    {

        [SerializeField] private List<PlayerRole> _availablePlayerRoles;
        [SerializeField] private GameObject _registerPlayerUI;
        [SerializeField] private GameObject _selectPlayerUI;
        [SerializeField] private Player.Player _player;
        [SerializeField] private Button _startButton;
        [SerializeField] private Button _selectButton;
        [SerializeField] private Button _goLeftButton;
        [SerializeField] private Button _goRightButton;
        [SerializeField] private TextMeshProUGUI _playerConfirmationText;
        [SerializeField] private TextMeshProUGUI _bombNameText;

        // This is a list of all players and if they have confirmed their role or not
        private List<Player.Player> _players = new List<Player.Player>();
    
        // Player counts as confirmed if its role is not null
        private bool AllPlayersConfirmed => _players.All(p => p.PlayerRole);

        private PlayerRole _selectedRole;
        private bool _registered;

        private void Start()
        {
            // Load all player roles if none specified
            if (_availablePlayerRoles.Count == 0) LoadPlayerRolesFromResources();

            _selectedRole = _availablePlayerRoles[0];
            _bombNameText.text = _selectedRole.name;
            _registered = false;

            _registerPlayerUI.SetActive(true);
            _selectPlayerUI.SetActive(false);

            _startButton.onClick.AddListener(StartGame);
            UpdateStartButtonUI();
        }

        private void Update() {
            if (!_registered && Input.anyKeyDown)
            {
                Register();
            }
        }

        private void LoadPlayerRolesFromResources()
        {
            _availablePlayerRoles.Clear();
            // Load all player roles from scriptableobjects folder
            
            foreach (PlayerRole playerRole in Resources.LoadAll<PlayerRole>("PlayerRoles"))
            {
                _availablePlayerRoles.Add(playerRole);
            }
        }

        public void Register()
        {
            if (_registered) return;

            _players.Add(_player);
            _player.PlayerNumber = _players.Count - 1;
            _player.SwitchPlayerModel(_selectedRole.playerModel);

            _registerPlayerUI.SetActive(false);
            _selectPlayerUI.SetActive(true);
            _registered = true;

            UpdateStartButtonUI();
        }

        public void TogglePlayerConfirmation(Player.Player player)
        {
            int playerNumber = player.PlayerNumber;

            if (IsPlayerConfirmed(playerNumber))
            {
                _players[playerNumber].PlayerRole = null;
                _selectButton.GetComponentInChildren<TextMeshProUGUI>().text = "Select";
                _goLeftButton.gameObject.SetActive(true);
                _goRightButton.gameObject.SetActive(true);
            }
            else
            {
                _players[playerNumber].PlayerRole = _selectedRole;
                _selectButton.GetComponentInChildren<TextMeshProUGUI>().text = "Unselect";
                _goLeftButton.gameObject.SetActive(false);
                _goRightButton.gameObject.SetActive(false);
            }

            UpdateStartButtonUI();
        }

        private bool IsPlayerConfirmed(int playerId)
        {
            if (playerId < 0)
            {
                Debug.LogError("Player ID cannot be negative. Seems like it was not initialized properly.");
            }

            return _players[playerId].PlayerRole;
        }

        private void UpdateStartButtonUI()
        {
            if (_players.Count == 0)
            {
                _startButton.interactable = false;
                _playerConfirmationText.text = "No players registered";
                _playerConfirmationText.color = Color.red;
            }
            else if (AllPlayersConfirmed)
            {
                _startButton.interactable = true;
                _playerConfirmationText.text = "All players confirmed";
                _playerConfirmationText.color = Color.green;
            }
            else
            {
                _startButton.interactable = false;
                _playerConfirmationText.text = _players.Count(p => p.PlayerRole) + " / " + _players.Count + " players confirmed";
                _playerConfirmationText.color = Color.red;
            }
        }

        public void StartGame()
        {
            PlayerManager.Instance.InitalizePlayers(_players);
            SceneManager.Instance.LoadScene("GameScene");
        }

        // TODO: Unify / Simplify GoRight() and GoLeft()
        public void GoRight()
        {
            if (_availablePlayerRoles.IndexOf(_selectedRole) < _availablePlayerRoles.Count - 1)
            {
                _selectedRole = _availablePlayerRoles[_availablePlayerRoles.IndexOf(_selectedRole) + 1];
            }
            else if (_availablePlayerRoles.IndexOf(_selectedRole) == _availablePlayerRoles.Count - 1)
            {
                _selectedRole = _availablePlayerRoles[0];
            }

            _bombNameText.text = _selectedRole.name;
            _player.SwitchPlayerModel(_selectedRole.playerModel);
        }

        public void GoLeft()
        {
            if (_availablePlayerRoles.IndexOf(_selectedRole) > 0)
            {
                _selectedRole = _availablePlayerRoles[_availablePlayerRoles.IndexOf(_selectedRole) - 1];
            }
            else if (_availablePlayerRoles.IndexOf(_selectedRole) == 0)
            {
                _selectedRole = _availablePlayerRoles[_availablePlayerRoles.Count - 1];
            }
            
            _bombNameText.text = _selectedRole.name;
            _player.SwitchPlayerModel(_selectedRole.playerModel);
        }
    }
}
