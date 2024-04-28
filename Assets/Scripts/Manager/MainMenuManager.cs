
using UnityEngine;

namespace Framework.Manager
{
    public class MainMenuManager : MonoBehaviour
    {
        public void LoadSelectionScene()
        {
            SceneManager.Instance.LoadScene("SelectionScene");
        }
    }
}
