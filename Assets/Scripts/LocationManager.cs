using System.Collections;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

public class LocationManager : MonoBehaviour
{
    private const int MaxNameLength = 20;
    private const int MinNameLength = 2;
    private const float MinLightIntensity = 0.4f;
    private const float MaxLightIntensity = 1f;
    private static readonly Color WorstColor = new(1, 0.1f, 0);
    private static readonly Color BestColor = new(0.3f, 1, 0);
    private static readonly Color HoverColor = Color.gray;
    private static readonly Color DefaultColor = Color.white;
    private static readonly WaitForSeconds OneSecondTimer = new(1f);

    public bool Placed;
    public bool Named;
    
    public Transform Parent { get; private set; }
    public bool IsGuessed { get; private set; }
    public string Name => _locationName;
    
    private GlobeController _globeController;
    private SessionManager _sessionManager;
    private TextMeshProUGUI _text;
    private string _locationName;
    private bool _showTypingCursor = true;
    private float _typingCursorBlinkTimer;

    private void Awake()
    {
        _globeController = FindObjectOfType<GlobeController>();
        _sessionManager = FindObjectOfType<SessionManager>();
        Parent = transform.parent;
        _text = Parent.GetComponentInChildren<TextMeshProUGUI>();
        UpdateText();
    }

    public void InitAsNew()
    {
        Placed = false;
        Named = false;
        _locationName = "";
        _text.text = "";
    }

    public void Init(QuizData.LocationData locationData)
    {
        Placed = true;
        Named = true;
        _locationName = locationData.Name;
        UpdateText();

        Parent.SetParent(_globeController.ChildTransform);
        Parent.localScale = locationData.LocalScale;
        Parent.localPosition = locationData.LocalPosition;
        Parent.localEulerAngles = locationData.LocalEulerAngles;
    }

    private void Update()
    {
        if (_sessionManager.Paused) return;
        
        if (!Placed && Input.GetKeyDown(KeyCode.Space))
        {
            Placed = true;
            ShowText(true);
            
            _globeController.ChangeFOVToDefault();
            _globeController.FreezeInputs = true;
            return;
        }

        if (Placed && !Named)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                _locationName = _locationName.Trim();
                if (_locationName.Length < MinNameLength) return;
                
                Named = true;
                Parent.SetParent(_globeController.ChildTransform);
                ((EditorManager) _sessionManager).AddNewLocation();
                
                _globeController.RevertFOV();
                _globeController.FreezeInputs = false;
            }
            else
            {
                if (Input.GetKeyDown(KeyCode.Backspace) && _locationName.Length > 0)
                {
                    _locationName = _locationName[..^1];
                }
                else
                {
                    var inputString = Regex.Replace(Input.inputString, "[^a-zA-ZÀ-ÖÙ-öù-ÿĀ-žḀ-ỿ ']", "");
                    _locationName = Regex.Replace(_locationName + inputString, "/\\s+/g", " ");
                    
                    var lnTrimmed = _locationName.Trim();
                    if (lnTrimmed.Length > MaxNameLength)
                        _locationName = lnTrimmed[..MaxNameLength];
                }
            }
            UpdateText();
        }
    }

    private void UpdateText()
    {
        _text.text = _locationName;
        transform.parent.name = $"Location ({_locationName})";
        if (Named) return;
        
        _text.text += _showTypingCursor ? "_" : " ";
        _typingCursorBlinkTimer += Time.deltaTime;
        if (_typingCursorBlinkTimer > 0.8f)
        {
            _typingCursorBlinkTimer = 0;
            _showTypingCursor = !_showTypingCursor;
        }
    }

    private void OnMouseEnter()
    {
        if (_sessionManager.Paused || _sessionManager is QuizManager { IsOver: true }) return;
        
        if (_sessionManager is QuizManager)
            ShowText(IsGuessed);

        if (!IsGuessed)
            SetColor(HoverColor); 
    }

    private void OnMouseExit()
    {
        if (_sessionManager.Paused || _sessionManager is QuizManager { IsOver: true }) return;
        
        if (_sessionManager is QuizManager)
            ShowText(false);

        if (!IsGuessed)
            SetColor(DefaultColor);
    }

    private void OnMouseDown()
    {
        if (_sessionManager.Paused) return;
        
        _sessionManager.InteractWithLocation(this);
    }

    public void Delete() => Destroy(transform.parent.gameObject);

    private void OnDestroy()
    {
        if (!Placed || Named) return;
        _globeController.RevertFOV();
        _globeController.FreezeInputs = false;
    }

    public IEnumerator ShowIncorrectGuessCR()
    {
        SetColor(Color.red);
        yield return OneSecondTimer;
        SetColor(Color.white);
    }

    public void SetLocationAsGuessed(float guessRatio)
    {
        IsGuessed = true;
        SetColor(Color.Lerp(BestColor, WorstColor, guessRatio));
    }

    private void SetColor(Color c) => GetComponentInChildren<MeshRenderer>().material.color = c;

    public void ShowText(bool show) => _text.enabled = show;
    public void ToggleText() => _text.enabled = !_text.enabled;

    public override string ToString() => _locationName;
}