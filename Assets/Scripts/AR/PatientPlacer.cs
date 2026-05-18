using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.InputSystem;

/// <summary>
/// Handles tapping the screen to place the AR patient model.
/// Once placed, the patient is locked in position.
/// </summary>
public class PatientPlacer : MonoBehaviour
{
    [Header("References")]
    public GameObject patientPrefab;
    public GameObject placementPromptUI;

    // Event fired when patient is successfully placed
    public System.Action OnPatientPlaced;

    private ARRaycastManager _raycastManager;
    private static List<ARRaycastHit> _hits = new List<ARRaycastHit>();
    private GameObject _spawnedPatient;
    private bool _isPlaced = false;

    void Awake()
    {
        _raycastManager = FindAnyObjectByType<ARRaycastManager>();
        if (_raycastManager == null)
            Debug.LogError("PatientPlacer: No ARRaycastManager found!");
    }

    void Update()
    {
        if (_isPlaced) return;
        if (Touchscreen.current == null) return;

        var touch = Touchscreen.current.primaryTouch;
        if (!touch.press.wasPressedThisFrame) return;

        // ADD THIS DEBUG:
        Debug.Log($"TAP detected! Prefab = {patientPrefab?.name ?? "NULL"}");

        Vector2 touchPosition = touch.position.ReadValue();

        if (_raycastManager.Raycast(
            touchPosition, _hits,
            TrackableType.PlaneWithinPolygon))
        {
            PlacePatient(_hits[0].pose);
        }
        else
        {
            Debug.Log("No AR plane hit detected");
        }
    }

    void PlacePatient(Pose pose)
    {
        if (patientPrefab == null)
        {
            Debug.LogError("PATIENT PREFAB IS NULL!");
            return;
        }

        Debug.Log($"Placing patient: {patientPrefab.name}");

        _spawnedPatient = Instantiate(
            patientPrefab, pose.position, pose.rotation);

        // Force correct scale
        _spawnedPatient.transform.localScale =
            new Vector3(0.5f, 0.5f, 0.5f);

        // Face camera
        Vector3 lookDir =
            Camera.main.transform.position -
            _spawnedPatient.transform.position;
        lookDir.y = 0;
        if (lookDir != Vector3.zero)
            _spawnedPatient.transform.rotation =
                Quaternion.LookRotation(lookDir);

        _isPlaced = true;

        if (placementPromptUI != null)
            placementPromptUI.SetActive(false);

        OnPatientPlaced?.Invoke();

        Debug.Log("Patient placed successfully!");
    }

    public void ResetPlacement()
    {
        if (_spawnedPatient != null)
            Destroy(_spawnedPatient);

        _isPlaced = false;

        if (placementPromptUI != null)
            placementPromptUI.SetActive(true);
    }
}