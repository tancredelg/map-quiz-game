using TMPro;
using UnityEngine;

public class TitleMover : MonoBehaviour
{
    private Vector3 _screenCentre;
    private RectTransform _rectTransform;
    private TextMeshProUGUI _tmp;

    private void Awake()
    {
        _screenCentre = new Vector3(Screen.width * 0.5f, Screen.height * 0.5f + 60, 0);
        _rectTransform = GetComponent<RectTransform>();
        _tmp = GetComponent<TextMeshProUGUI>();
    }

    private void Update()
    {
        Vector3 delta = _screenCentre - Input.mousePosition;
        
        float xFactor = Mathf.Abs(delta.x) / _screenCentre.x;
        float yFactor = Mathf.Abs(delta.y) / _screenCentre.y;

        var offset = new Vector3(xFactor == 0 ? 0 : delta.x * Mathf.Pow(xFactor, -0.25f), yFactor == 0 ? 0 : delta.y * Mathf.Pow(yFactor, -0.25f), 0);
        
        _rectTransform.position = _screenCentre + offset;
        
        float deltaFactor = 3.25f * xFactor * xFactor + 1.1f * yFactor * yFactor;
        _tmp.fontSize = Mathf.Clamp(200 - Mathf.Pow(200 * deltaFactor, 1.125f), 0, 200);
        _tmp.color = new Color(1,1,1,Mathf.Clamp01(0.75f - deltaFactor));
    }
}