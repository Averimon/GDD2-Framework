using UnityEngine;
using TMPro;
using Framework.Player;

namespace Framework.Manager
{
    public class GameOverManager : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _winnerText;

        private void Start()
        {
            if (PlayerManager.Instance.AlivePlayerCount == 1)
            {
                Player.Player player = FindObjectOfType<Player.Player>();
                _winnerText.text = $"Congratulations Player #{player.PlayerID}.\nYou won!";
                player.transform.position = Vector3.zero;
                player.transform.rotation = Quaternion.Euler(0, 180, 0);
                player.transform.localScale = Vector3.one;
                player.GetComponent<PlayerMovementController>().enabled = false;
            }
            else
            {
                _winnerText.text = "Now, that's awkward...\nNo one won.\n\nHere's a consolation prize:\nA free ticket to the main menu!";
            }
        }

        public void LoadMainMenu()
        {
            if (PlayerManager.Instance.AlivePlayerCount == 1)
                Destroy(FindObjectOfType<Player.Player>().gameObject);
            Destroy(PlayerManager.Instance.gameObject);
            Destroy(GameManager.Instance.gameObject);
            SceneManager.Instance.LoadScene("MainMenu");
        }
    }
}
