using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class QuizLoadButton : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    [SerializeField] private TextMeshProUGUI TmpName;
    [SerializeField] private TextMeshProUGUI TmpLocations;
    
    private string _loadPath = "";
    private string _quizName;
    private string[] _quizLocations;
    
    private QuizLoadManager _quizLoadManager;

    public void Init(string path)
    {
        _quizLoadManager = FindObjectOfType<QuizLoadManager>();
        _loadPath = path;
        if (!SerializationManager.GetQuizDetails(path, out _quizName, out _quizLocations))
            Destroy(gameObject);

        TmpName.text = _quizName;
        TmpLocations.text = _quizLocations.Length.ToString();
    }

    public void OnSelect(BaseEventData eventData)
    {
        _quizLoadManager.SelectedButton = GetComponent<Button>();
        SerializationManager.SetQuizToLoadPath(_loadPath);
        _quizLoadManager.EnableQuizLoadButtons(true);
    }

    public void OnDeselect(BaseEventData eventData)
    {
        _quizLoadManager.SelectedButton = null;
        //StartCoroutine(DisableButtonsCR());
    }
    
    IEnumerator DisableButtonsCR()
    {
        yield return new WaitForSeconds(0.1f);
        if (_quizLoadManager.SelectedButton == GetComponent<Button>()) yield break;
        _quizLoadManager.EnableQuizLoadButtons(false);
    }
}
