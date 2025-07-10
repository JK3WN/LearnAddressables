using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyManager : MonoBehaviour
{
    public void Button_Start()
    {
        SceneManager.LoadScene("DownloadScene");
    }
}
