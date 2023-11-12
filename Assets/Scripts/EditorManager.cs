using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EditorManager : SessionManager
{
    private const int MaxGuessesAllowed = 9;
    
    [Header("Editor Manager")]
    [SerializeField] private TextMeshProUGUI LocationCountText;
    [SerializeField] private GameObject SaveQuizButton;
    [SerializeField] private TMP_InputField SavePanelTitleField;
    [SerializeField] private Slider SavePanelGuessesSlider;

    private LocationManager _newLocation;

    protected override void Init()
    {
        base.Init();

        SavePanelGuessesSlider.wholeNumbers = true;
        SavePanelGuessesSlider.maxValue = MaxGuessesAllowed + 1;
        SavePanelGuessesSlider.minValue = 1;

        PauseText.text = "Session paused.\n\n<Space> to add & place a location\n<Enter> to set its name\n<Left Click> a location to remove it\n<Right Click> to toggle all names"; 
        
        UpdateLocationCountText();
    }

    protected override void Update()
    {
        base.Update();
        if (Paused) return;
        
        if (!_newLocation && Input.GetKeyDown(KeyCode.Space))
            GetNewLocation();
        
        if (Input.GetMouseButtonDown(1))
            foreach (var location in Locations)
                location.ToggleText();
    }

    private void GetNewLocation()
    {
        _newLocation = Instantiate(LocationPrefab).GetComponentInChildren<LocationManager>();
        _newLocation.InitAsNew();
        SaveQuizButton.SetActive(false);
    }

    public void AddNewLocation()
    {
        if (_newLocation)
            Locations.Add(_newLocation);
        
        UpdateLocationCountText();
        _newLocation = null;
        SaveQuizButton.SetActive(true);
    }

    public override void InteractWithLocation(LocationManager location)
    {
        RemoveLocation(location);
    }

    private void RemoveLocation(LocationManager location)
    {
        if (Locations.Contains(location))
        {
            Locations.Remove(location);
            UpdateLocationCountText();
        }
        
        if (location == _newLocation)
        {
            _newLocation = null;
            SaveQuizButton.SetActive(true);
        }

        location.Delete();
    }

    public override void PauseSession()
    {
        base.PauseSession();
        
        SaveQuizButton.SetActive(false);
    }

    public override void ResumeSession()
    {
        base.ResumeSession();
        
        if (_newLocation && _newLocation.Placed && !_newLocation.Named)
            GlobeController.FreezeInputs = true;
        
        SaveQuizButton.SetActive(!_newLocation);
    }

    public void ShowSavePanel()
    {
        EndPanel.SetActive(true);
        PauseSession();
        
        SavePanelTitleField.placeholder.GetComponent<TextMeshProUGUI>().text = QuizData.Name;
        SavePanelGuessesSlider.value = QuizData.GuessesAllowed == 0 ? MaxGuessesAllowed + 1 : QuizData.GuessesAllowed;
    }

    public void SaveQuiz()
    {
        if (Locations.Count < 1)
        {
            Debug.LogError("You need at least 1 location to save the quiz.");
            return;
        }
        if (SavePanelTitleField.text == "")
        {
            Debug.LogError("The quiz needs a name.");
            return;
        }

        if (SerializationManager.SaveExists(SavePanelTitleField.text))
        {
            Debug.LogError("That quiz name is taken.");
            return;
        }
        
        
        QuizData.Name = SavePanelTitleField.text;
        
        QuizData.GuessesAllowed = (int)SavePanelGuessesSlider.value;
        if (QuizData.GuessesAllowed > MaxGuessesAllowed)
            QuizData.GuessesAllowed = 0;

        QuizData.LocationsData = Locations.Select(location => new QuizData.LocationData(location)).ToArray();
        
        SerializationManager.SaveQuiz(QuizData);
    }

    public void UpdateTextWithSlider(TMP_Text text)
    {
        var sliderVal = (int)SavePanelGuessesSlider.value;
        text.text = sliderVal > MaxGuessesAllowed ? "∞" : sliderVal.ToString();
    }

    private void UpdateLocationCountText()
    {
        LocationCountText.text = Locations.Count + (Locations.Count == 1 ? " location" : " locations");
    }
}