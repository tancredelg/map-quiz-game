using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class GlobeController : MonoBehaviour
{
    private const float ChangeFOVAnimLength = 0.5f;

    public bool FreezeInputs;
    public bool MenuMode;

    public Transform ChildTransform { get; private set; }
    public bool IsZooming { get; private set; }

    [SerializeField] private int MaxFieldOfView;
    [SerializeField] private int MinFieldOfView;
    [SerializeField] private int FocusedFieldOfView;
    [SerializeField] private float ScrollMultiplier;
    [SerializeField] private float RotateSpeed;
    [SerializeField] private float GlobeYOffset;

    private Camera _camera;
    private Vector3 _lastMousePos;
    private float _rotateMultiplier;
    private float _xAxisRotation;
    private float _originalXRotation;
    private float _yAxisRotation;
    private float _lastFieldOfView;

    private void Awake()
    {
        ChildTransform = transform.GetChild(0);
        _camera = FindObjectOfType<Camera>();
        UpdateRotationFactor();
        _originalXRotation = Random.Range(0, 360);
        if (MenuMode)
            ChildTransform.localEulerAngles += Vector3.up * _originalXRotation;
    }

    private void UpdateRotationFactor() => _rotateMultiplier = _camera.fieldOfView * RotateSpeed / 1000f;

    private void Update()
    {
        if (MenuMode)
        {
            _xAxisRotation = (Screen.height / 2 - (Input.mousePosition.y + GlobeYOffset)) * 0.75f * _rotateMultiplier;
            _yAxisRotation = _originalXRotation + Input.mousePosition.x * _rotateMultiplier;
            
            transform.eulerAngles = new Vector3(_xAxisRotation, 0, 0);
            ChildTransform.localEulerAngles = new Vector3(ChildTransform.localEulerAngles.x, _yAxisRotation, 0);
            return;
        }
        
        if (FreezeInputs) return;
        
        if (Input.GetMouseButtonDown(0))
            _lastMousePos = Input.mousePosition;

        if (Input.mouseScrollDelta.magnitude > 0)
        {
            _camera.fieldOfView = Mathf.Clamp(_camera.fieldOfView - Input.mouseScrollDelta.y * ScrollMultiplier, MinFieldOfView, MaxFieldOfView);
            UpdateRotationFactor();
        }

        if (Input.GetMouseButton(0))
        {
            _xAxisRotation = transform.eulerAngles.x + (Input.mousePosition.y - _lastMousePos.y) * _rotateMultiplier;
            _yAxisRotation = ChildTransform.localEulerAngles.y - (Input.mousePosition.x - _lastMousePos.x) * _rotateMultiplier;
            
            transform.eulerAngles = new Vector3(_xAxisRotation, 0, 0);
            ChildTransform.localEulerAngles = new Vector3(ChildTransform.localEulerAngles.x, _yAxisRotation, 0);
            
            _lastMousePos = Input.mousePosition;
        }
    }

    public void ChangeFOVToDefault() => ChangeFOV(FocusedFieldOfView);

    public void ChangeFOV(int fieldOfView)
    {
        _lastFieldOfView = _camera.fieldOfView;
        StartCoroutine(ChangeFOVCR(fieldOfView));
    }
    
    public void RevertFOV()
    {
        StartCoroutine(ChangeFOVCR(_lastFieldOfView));
    }
    
    private IEnumerator ChangeFOVCR(float endFieldOfView)
    {
        IsZooming = true;
        int animFrameCount = Mathf.RoundToInt(ChangeFOVAnimLength / Time.deltaTime);
        float startFieldOfView = _camera.fieldOfView;
        
        // Debug.Log("ChangeFOVAnimLength = " + ChangeFOVAnimLength + " | animFrameCount = " + animFrameCount + " | startFieldOfView = " + startFieldOfView + " | endFieldOfView = " + endFieldOfView);
        
        for (int i = 1; i < animFrameCount; i++)
        {
            if (!_camera) yield break;
            _camera.fieldOfView = Mathf.Lerp(startFieldOfView, endFieldOfView, (float)Math.Pow((double)i / animFrameCount, 0.25f));
            yield return null;
        }

        UpdateRotationFactor();
        IsZooming = false;
    }
}