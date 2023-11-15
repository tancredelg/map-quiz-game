using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class QuizLoadButton : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    [SerializeField] private TextMeshProUGUI TmpName;
    [SerializeField] private TextMeshProUGUI TmpLocations;
    
    private string _loadPath = "";
    private QuizData _quizData;
    
    private QuizLoadManager _quizLoadManager;

    public void Init(string path, QuizLoadManager quizLoadManager)
    {
        _quizLoadManager = quizLoadManager;
        _loadPath = path;
        
        SerializationManager.SetQuizLoadPath(_loadPath);
        _quizData = SerializationManager.LoadQuizData();
        
        if (_quizData == null)
            Destroy(gameObject);
        
        TmpName.text = _quizData.Name;;
        TmpLocations.text = _quizData.LocationsData.Length.ToString();
    }

    public void OnSelect(BaseEventData eventData)
    {
        _quizLoadManager.SelectedButton = GetComponent<Button>();
        SerializationManager.SetQuizLoadPath(_loadPath);
        _quizLoadManager.EnableQuizLoadButtons();
    }

    public void OnDeselect(BaseEventData eventData)
    {
        StartCoroutine(DisableButtonsCR());
    }
    
    IEnumerator DisableButtonsCR()
    {
        yield return null;
        
        if (_quizLoadManager.SelectedButton != null && _quizLoadManager.SelectedButton != GetComponent<Button>())
            yield break;
        
        if (_quizLoadManager.DisableQuizLoadButtons())
            _quizLoadManager.SelectedButton = null;
    }
}
