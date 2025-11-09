using UnityEngine;

using UnityEngine.SceneManagement;
public class mainmenu : MonoBehaviour
{
    public void ChangeScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
