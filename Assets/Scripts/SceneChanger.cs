using UnityEngine;

public class SceneChanger : MonoBehaviour
{
    public void LoadScene(string sceneName) => SceneLoader.Instance.LoadScene(sceneName);

    public void QuitGame() => Application.Quit();
}