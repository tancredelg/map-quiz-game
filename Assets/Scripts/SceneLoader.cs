using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance;

    [SerializeField] private string MainMenu = "MainMenuScene";
    [SerializeField] private string LoadMenu = "LoadMenuScene";
    [SerializeField] private string QuizMaker = "QuizMakerScene";
    [SerializeField] private string QuizTaker = "QuizTakerScene";
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void LoadScene(string sceneName) => StartCoroutine(LoadSceneAsync(sceneName));

    private static IEnumerator LoadSceneAsync(string sceneName)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        Debug.Log("Loading " + sceneName + "...");

        while (!asyncLoad.isDone)
            yield return null;
        
        // New scene fully loaded
        Debug.Log("Scene fully loaded.");
    }
}