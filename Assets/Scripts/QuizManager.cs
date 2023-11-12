using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class QuizManager : SessionManager
{
    public bool IsOver { get; private set; }
    
    [Header("Quiz Manager")]
    [SerializeField] private TextMeshProUGUI LocationText;
    [SerializeField] private TextMeshProUGUI ScoreText;
    [SerializeField] private TextMeshProUGUI EndText;

    private List<LocationManager> _locationsLeft;
    private LocationManager _currentLocation;
    private int _guessesPerLocation;
    private int _totalGuessesMade;
    private int _correctGuesses;
    private int _guessesOnCurrent;
    private float _score;

    protected override void Init()
    {
        base.Init();

        _locationsLeft = new List<LocationManager>(Locations);
        _totalGuessesMade = _correctGuesses = 0;
        _guessesPerLocation = QuizData.GuessesAllowed;
        
        PauseText.text = $"{QuizData.Name}\n \nSession paused.";

        foreach (var location in Locations)
            location.ShowText(false);
        
        UpdateScore();
        ChangeLocation();
    }

    public override void InteractWithLocation(LocationManager location)
    {
        if (IsOver || location.IsGuessed)
            base.InteractWithLocation(location);
        else
            GuessCurrentLocation(location);
    }

    private void GuessCurrentLocation(LocationManager guess)
    {
        _totalGuessesMade++;
        
        if (guess.Name == _currentLocation.Name)
        {
            guess.SetLocationAsGuessed(_guessesPerLocation < 1 ? 0 : (float)_guessesOnCurrent / _guessesPerLocation);
            _correctGuesses++;
            UpdateScore();
            ChangeLocation();
        }
        else
        {
            _guessesOnCurrent++;
            StartCoroutine(guess.ShowIncorrectGuessCR());
            UpdateScore();
            
            if (_guessesPerLocation > 0 && _totalGuessesMade >= Locations.Count * _guessesPerLocation)
                EndSession();
            else if (_guessesOnCurrent == _guessesPerLocation)
            {
                _currentLocation.SetLocationAsGuessed(1);
                ChangeLocation();
            }
            else
                UpdateLocationText();
        }
    }

    private void UpdateLocationText()
    {
        int guessesLeft = _guessesPerLocation - _guessesOnCurrent;
        string guessesLeftText = $"{(_guessesPerLocation == 0 ? "∞" : guessesLeft)} guess{(guessesLeft == 1 ? "" : "es")}";
        LocationText.text = $"Click on {_currentLocation} ({guessesLeftText} left)";
    }

    private void ChangeLocation()
    {
        _locationsLeft.Remove(_currentLocation);
        if (_locationsLeft.Count == 0)
        {
            EndSession();
            return;
        }
        
        _currentLocation = _locationsLeft[new System.Random().Next(_locationsLeft.Count)];
        _guessesOnCurrent = 0;
        UpdateLocationText();
    }

    private void UpdateScore()
    {
        if (_guessesPerLocation < 1)
            _score = 100f;
        else if (_totalGuessesMade < 1)
            _score = 0f;
        else
            _score = 100f * _correctGuesses / _totalGuessesMade;
        
        ScoreText.text = Mathf.RoundToInt(_score) + "%";
    }
    
    public override void PauseSession()
    {
        base.PauseSession();
        
        PauseText.transform.parent.gameObject.SetActive(!IsOver);
        EndPanel.SetActive(IsOver);
    }

    private void EndSession()
    {
        IsOver = true;
        PauseSession();
        EndText.text = $"{QuizData.Name}\nScore: {ScoreText.text}\nTime: {TimerText.text}";
        
        foreach (var location in Locations)
            location.ShowText(true);
    }
}