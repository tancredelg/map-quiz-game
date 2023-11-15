using System;
using UnityEngine;
using UnityEngine.EventSystems;
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

    public void NewQuiz()
    {
        SerializationManager.SetQuizLoadPath("");
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

    public void EnableQuizLoadButtons()
    {
        PlayButton.interactable = true;
        EditButton.interactable = true;
    }

    public bool DisableQuizLoadButtons()
    {
        if (EventSystem.current.currentSelectedGameObject == PlayButton.gameObject
            || EventSystem.current.currentSelectedGameObject == EditButton.gameObject)
            return false;
        
        PlayButton.interactable = false;
        EditButton.interactable = false;
        return true;
    }

    public bool AreQuizLoadButtonsActive() => PlayButton.interactable && EditButton.interactable;
}