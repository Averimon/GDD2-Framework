using UnityEngine;
using TMPro;
using Framework.Player;
using Framework.Networking;

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
                _winnerText.rectTransform.localPosition = new Vector3(0, 330, 0);
                player.transform.position = Vector3.zero;
                player.transform.rotation = Quaternion.Euler(0, 180, 0);
                player.transform.localScale = Vector3.one;
                player.GetComponent<PlayerMovementController>().enabled = false;
            }
            else
            {
                _winnerText.text = "Now, that's awkward...\nNo one won.\n\nHere's a consolation prize:\nA free ticket to the main menu!";
                _winnerText.rectTransform.localPosition = new Vector3(0, 130, 0);
            }
        }

        public void LoadMainMenu()
        {
            if (PlayerManager.Instance.AlivePlayerCount == 1)
                Destroy(FindObjectOfType<Player.Player>().gameObject);
            Destroy(PlayerManager.Instance.gameObject);
            Destroy(GameManager.Instance.gameObject);
            NetworkManagerExtension networkManager = FindObjectOfType<NetworkManagerExtension>();
            networkManager.StopHost();
            SceneManager.Instance.LoadScene("MainMenu");
        }
    }
}
