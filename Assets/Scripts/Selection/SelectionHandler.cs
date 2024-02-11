using System.Collections.Generic;
using Framework.Manager;
using UnityEngine;
using TMPro;
using Framework.Player;

namespace Framework.Selection
{
    public class SelectionHandler : MonoBehaviour
    {
        [SerializeField] private List<PlayerRole> _availablePlayerRoles;
        [SerializeField] private TextMeshProUGUI _bombNameText;
        private PlayerRole selectedBomb;

        private void Start()
        {
            if (_availablePlayerRoles.Count == 0)
            {
                LoadPlayerRolesFromResources();
            }

            selectedBomb = _availablePlayerRoles[0];
            _bombNameText.text = selectedBomb.name;
        }

        private void LoadPlayerRolesFromResources()
        {
            _availablePlayerRoles.Clear();
            // Load all player roles from scriptableobjects folder
            
            foreach (PlayerRole playerRole in Resources.LoadAll<PlayerRole>("Player/PlayerRoles"))
            {
                _availablePlayerRoles.Add(playerRole);
            }
        }

        public void Select()
        {
            PlayerManager.Instance.RegisterPlayer(selectedBomb);
        }

        public void StartGame()
        {
            SceneManager.Instance.LoadScene("GameScene");
        }

        public void GoRight()
        {
            if (_availablePlayerRoles.IndexOf(selectedBomb) < _availablePlayerRoles.Count - 1)
            {
                selectedBomb = _availablePlayerRoles[_availablePlayerRoles.IndexOf(selectedBomb) + 1];
            }
            else if (_availablePlayerRoles.IndexOf(selectedBomb) == _availablePlayerRoles.Count - 1)
            {
                selectedBomb = _availablePlayerRoles[0];
            }

            _bombNameText.text = selectedBomb.name;
        }

        public void GoLeft()
        {
            if (_availablePlayerRoles.IndexOf(selectedBomb) > 0)
            {
                selectedBomb = _availablePlayerRoles[_availablePlayerRoles.IndexOf(selectedBomb) - 1];
            }
            else if (_availablePlayerRoles.IndexOf(selectedBomb) == 0)
            {
                selectedBomb = _availablePlayerRoles[_availablePlayerRoles.Count - 1];
            }
            
            _bombNameText.text = selectedBomb.name;
        }
    }
}
