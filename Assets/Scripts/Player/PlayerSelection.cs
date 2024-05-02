using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

namespace Framework.Player
{
    public class PlayerSelection : MonoBehaviour
    {
        [Header("Player Selection Settings")]
        [SerializeField] private int _playerID;
        [SerializeField] private Player _player;

        [Header("UI References")]
        [SerializeField] private Button _selectButton;
        [SerializeField] private Button _goLeftButton;
        [SerializeField] private Button _goRightButton;
        [SerializeField] private TMP_Text _currRoleText;

        private readonly List<PlayerRole> _availablePlayerRoles = new();
        private PlayerRole _selectedRole;

        public UnityEvent OnPlayerConfirmed = new();

        private void Start()
        {
            LoadPlayerRolesFromResources();

            _selectedRole = _availablePlayerRoles[0];
            _currRoleText.text = _selectedRole.name;
            _player.SwitchPlayerModel(_selectedRole.playerModel);
            _player.PlayerID = _playerID;
        }
        
        private void LoadPlayerRolesFromResources()
        {
            _availablePlayerRoles.Clear();
            
            foreach (PlayerRole playerRole in Resources.LoadAll<PlayerRole>("PlayerRoles/AvailableRoles"))
            {
                _availablePlayerRoles.Add(playerRole);
            }
        }

        public void TogglePlayerConfirmation()
        {
            if (_player.PlayerRole != null)
            {
                _player.PlayerRole = null;
                _selectButton.GetComponentInChildren<TextMeshProUGUI>().text = "Select";
                _goLeftButton.gameObject.SetActive(true);
                _goRightButton.gameObject.SetActive(true);
            }
            else
            {
                _player.PlayerRole = _selectedRole;
                _selectButton.GetComponentInChildren<TextMeshProUGUI>().text = "Unselect";
                _goLeftButton.gameObject.SetActive(false);
                _goRightButton.gameObject.SetActive(false);
            }

            OnPlayerConfirmed?.Invoke();
        }
        
        public void GoRight()
        {
            if (_player.PlayerRole != null) return;

            if (_availablePlayerRoles.IndexOf(_selectedRole) < _availablePlayerRoles.Count - 1)
            {
                _selectedRole = _availablePlayerRoles[_availablePlayerRoles.IndexOf(_selectedRole) + 1];
            }
            else if (_availablePlayerRoles.IndexOf(_selectedRole) == _availablePlayerRoles.Count - 1)
            {
                _selectedRole = _availablePlayerRoles[0];
            }

            _currRoleText.text = _selectedRole.name;
            _player.SwitchPlayerModel(_selectedRole.playerModel);
        }

        public void GoLeft()
        {
            if (_player.PlayerRole != null) return;
            
            if (_availablePlayerRoles.IndexOf(_selectedRole) > 0)
            {
                _selectedRole = _availablePlayerRoles[_availablePlayerRoles.IndexOf(_selectedRole) - 1];
            }
            else if (_availablePlayerRoles.IndexOf(_selectedRole) == 0)
            {
                _selectedRole = _availablePlayerRoles[_availablePlayerRoles.Count - 1];
            }
            
            _currRoleText.text = _selectedRole.name;
            _player.SwitchPlayerModel(_selectedRole.playerModel);
        }
    }
}
