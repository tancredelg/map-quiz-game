using System;
using UnityEngine;
using UnityEngine.UI;

public class QuizLoadManager : MonoBehaviour
{
    [SerializeField] private GameObject QuizLoadButton;
    [SerializeField] private Button PlayButton;
    [SerializeField] private Button EditButton;

    private Button[] _quizButtons;

    public Button SelectedButton;

    private void Awake()
    {
        foreach (var path in SerializationManager.GetAllQuizPaths())
            Instantiate(QuizLoadButton, transform).GetComponent<QuizLoadButton>().Init(path, this);
    }

    private void Update()
    {
        if (SelectedButton == null && AreQuizLoadButtonsActive())
            EnableQuizLoadButtons(false);
        
        if (SelectedButton != null && !AreQuizLoadButtonsActive())
            EnableQuizLoadButtons(true);
    }

    public void NewQuiz()
    {
        SerializationManager.SetQuizToLoadPath("");
        SceneLoader.Instance.LoadScene("QuizMakerScene");
        Debug.Log("New QuizMakerScene");
    }

    public void EditQuiz()
    {
        SceneLoader.Instance.LoadScene("QuizMakerScene");
        Debug.Log("Edit QuizMakerScene");
    }
    
    public void PlayQuiz()
    {
        SceneLoader.Instance.LoadScene("QuizTakerScene");
        Debug.Log("Play QuizTakerScene");
    }

    public void EnableQuizLoadButtons(bool enable)
    {
        PlayButton.interactable = enable;
        EditButton.interactable = enable;
    }

    private bool AreQuizLoadButtonsActive() => PlayButton.interactable && EditButton.interactable;
}