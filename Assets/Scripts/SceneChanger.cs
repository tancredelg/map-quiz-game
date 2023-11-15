using UnityEngine;

public class SceneChanger : MonoBehaviour
{
    public void LoadScene(string sceneName) => SceneLoader.Instance.LoadScene(sceneName);
    public void ReloadScene() => SceneLoader.Instance.ReloadScene();

    public void QuitGame() => Application.Quit();
}