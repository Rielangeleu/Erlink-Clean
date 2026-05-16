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

        Vector2 touchPosition = touch.position.ReadValue();

        if (_raycastManager.Raycast(
            touchPosition, _hits, TrackableType.PlaneWithinPolygon))
        {
            PlacePatient(_hits[0].pose);
        }
    }

    void PlacePatient(Pose pose)
    {
        if (patientPrefab == null)
        {
            Debug.LogError("PatientPlacer: No patient prefab!");
            return;
        }

        _spawnedPatient = Instantiate(
            patientPrefab, pose.position, pose.rotation);

        // ← ADD THIS: Force correct scale for AR
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

        Debug.Log("Patient placed at: " + pose.position);
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