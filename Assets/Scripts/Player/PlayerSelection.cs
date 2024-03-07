using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

namespace Framework.Player
{
    public class PlayerSelection : MonoBehaviour
    {
        [SerializeField] private int _playerID;
        private List<PlayerRole> _availablePlayerRoles = new List<PlayerRole>();
        private Player _player;
        private Button _selectButton;
        private Button _goLeftButton;
        private Button _goRightButton;
        private TextMeshProUGUI _currRoleText;
        private PlayerRole _selectedRole;

        public UnityEvent OnPlayerConfirmed = new UnityEvent();

        private void Awake()
        {
            _player = GetComponentInChildren<Player>();
            _selectButton = transform.Find("SelectButton").GetComponent<Button>();
            _goLeftButton = transform.Find("GoLeftButton").GetComponent<Button>();
            _goRightButton = transform.Find("GoRightButton").GetComponent<Button>();
            _currRoleText = transform.Find("CurrentRoleText").GetComponent<TextMeshProUGUI>();
        }

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
