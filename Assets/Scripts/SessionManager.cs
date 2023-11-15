using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class SessionManager : MonoBehaviour
{
    public bool Paused { get; protected set; }

    [FormerlySerializedAs("MessageText")]
    [Header("Session Manager")]
    [SerializeField] protected TextMeshProUGUI TimerText;
    [SerializeField] protected TextMeshProUGUI PauseText;
    [SerializeField] protected int PauseFieldOfView;
    [SerializeField] protected GameObject EndPanel;
    [SerializeField] protected GameObject LocationPrefab;
    [SerializeField] private GameObject MessagePrefab;
    [SerializeField] private Transform MessageContainer;

    protected List<LocationManager> Locations;
    protected GlobeController GlobeController;
    protected QuizData QuizData;

    private float _time;

    protected void Awake()
    {
        Init();
    }

    protected virtual void Init()
    {
        // Clear scene
        foreach (var location in FindObjectsOfType<LocationManager>())
            location.Delete();
        
        Locations = new List<LocationManager>();
        GlobeController = FindObjectOfType<GlobeController>();
        PauseText.transform.parent.gameObject.SetActive(false);
        EndPanel.SetActive(false);

        QuizData = SerializationManager.LoadQuizData();

        foreach (var locationData in QuizData.LocationsData)
        {
            var location = Instantiate(LocationPrefab).GetComponentInChildren<LocationManager>();
            location.Init(locationData);
            Locations.Add(location);
        }
    }

    protected virtual void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !GlobeController.IsZooming)
        {
            if (Paused)
                ResumeSession();
            else
                PauseSession();
        }

        if (Paused || this is QuizManager { IsOver: true })
            return;

        UpdateTimer();
    }

    private void UpdateTimer()
    {
        _time += Time.deltaTime;
        int sec = (int)_time % 60;
        int min = (int)_time / 60;
        TimerText.text = (min < 10 ? "0" + min : min) + ":" + (sec < 10 ? "0" + sec : sec);
    }

    public virtual void InteractWithLocation(LocationManager location) => location.ToggleText();

    public virtual void PauseSession()
    {
        Paused = true;
        GlobeController.FreezeInputs = true;
        GlobeController.ChangeFOV(PauseFieldOfView);
        PauseText.transform.parent.gameObject.SetActive(!EndPanel.activeInHierarchy);
    }

    public virtual void ResumeSession()
    {
        Paused = false;
        GlobeController.FreezeInputs = false;
        GlobeController.RevertFOV();
        PauseText.transform.parent.gameObject.SetActive(false);
        EndPanel.SetActive(false);
    }

    public void PrintMessage(string message)
    {
        Instantiate(MessagePrefab, MessageContainer).GetComponent<Message>().SpawnMessage(message);
    }
    
    public void PrintErrorMessage(string message)
    {
        Instantiate(MessagePrefab, MessageContainer).GetComponent<Message>().SpawnErrorMessage(message);
    }
}