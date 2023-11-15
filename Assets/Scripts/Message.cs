using TMPro;
using UnityEngine;

public class Message : MonoBehaviour
{
    [SerializeField] private int TimeAlive;
    [SerializeField] private Color ErrorMessageColor;
    
    private TextMeshProUGUI _messageText;

    private void Awake()
    {
        _messageText = GetComponentInChildren<TextMeshProUGUI>();
    }

    public void SpawnMessage(string message)
    {
        _messageText.text = message;
        Destroy(gameObject, TimeAlive);
    }

    public void SpawnErrorMessage(string message)
    {
        _messageText.color = ErrorMessageColor;
        SpawnMessage(message);
    }
}