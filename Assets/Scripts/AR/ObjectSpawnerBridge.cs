using UnityEngine;
using System;
using System.Reflection;

/// <summary>
/// Bridges the AR Mobile Template's ObjectSpawner with our ScenarioLoader and RPM system.
/// Updated to keep UI panel layouts completely hidden until physical world bubbles are tapped.
/// </summary>
public class ObjectSpawnerBridge : MonoBehaviour
{
    [Header("References")]
    public MonoBehaviour objectSpawner;
    public ScenarioLoader scenarioLoader;
    public RPMSequenceController rpmController;
    public UIManager uiManager;

    [Header("State")]
    private bool _patientPlaced = false;

    void Start()
    {
        FindAndConnectSpawner();

        if (uiManager != null)
            uiManager.ShowPlacementPrompt(true);
    }

    void FindAndConnectSpawner()
    {
        if (objectSpawner == null)
        {
            foreach (MonoBehaviour mb in FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None))
            {
                if (mb != null && mb.GetType().Name == "ObjectSpawner")
                {
                    objectSpawner = mb;
                    break;
                }
            }
        }

        if (objectSpawner != null)
        {
            try
            {
                EventInfo eventInfo = objectSpawner.GetType().GetEvent("objectSpawned");
                if (eventInfo != null)
                {
                    MethodInfo targetMethod = typeof(ObjectSpawnerBridge).GetMethod(nameof(OnPatientSpawnedDynamic), BindingFlags.NonPublic | BindingFlags.Instance);
                    Delegate handler = Delegate.CreateDelegate(eventInfo.EventHandlerType, this, targetMethod);
                    eventInfo.AddMethod.Invoke(objectSpawner, new object[] { handler });
                    Debug.Log("ObjectSpawnerBridge: Dynamic link established with ObjectSpawner! ✅");
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"ObjectSpawnerBridge Binding Failed: {e.Message}");
            }
        }
    }

    void OnPatientSpawnedDynamic(object spawnedInstance)
    {
        if (_patientPlaced)
        {
            GameObject duplicateGo = null;
            if (spawnedInstance is GameObject go) duplicateGo = go;
            else
            {
                var pi = spawnedInstance.GetType().GetProperty("gameObject");
                if (pi != null) duplicateGo = pi.GetValue(spawnedInstance) as GameObject;
            }

            if (duplicateGo != null)
            {
                Debug.LogWarning("ObjectSpawnerBridge: Blocked and destroyed a secondary duplicate spawn attempt! 🛑");
                Destroy(duplicateGo);
            }
            return;
        }

        if (spawnedInstance == null) return;

        GameObject resolvedGo = null;
        if (spawnedInstance is GameObject targetGo)
        {
            resolvedGo = targetGo;
        }
        else
        {
            PropertyInfo pi = spawnedInstance.GetType().GetProperty("gameObject");
            if (pi != null)
            {
                resolvedGo = pi.GetValue(spawnedInstance) as GameObject;
            }
        }

        if (resolvedGo != null)
        {
            OnPatientSpawned(resolvedGo);
        }
    }

    void OnPatientSpawned(GameObject spawnedObject)
    {
        _patientPlaced = true;
        Debug.Log($"Patient spawned safely: {spawnedObject.name}");

        if (objectSpawner != null)
        {
            objectSpawner.enabled = false;
        }

        // Adjust positioning metrics relative to the floor coordinate space
        Vector3 currentPos = spawnedObject.transform.position;
        spawnedObject.transform.position = new Vector3(currentPos.x, currentPos.y + 0.35f, currentPos.z);
        spawnedObject.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);

        if (Camera.main != null)
        {
            Vector3 lookDir = Camera.main.transform.position - spawnedObject.transform.position;
            lookDir.y = 0;
            if (lookDir != Vector3.zero)
                spawnedObject.transform.rotation = Quaternion.LookRotation(lookDir);
        }

        if (uiManager != null)
        {
            uiManager.ShowPlacementPrompt(false);

            // ── CRITICAL RE-ROUTE FIX ──
            // Do NOT call ShowRPMPanel(true) here anymore! We keep it hidden 
            // until the player taps the first floating bubble object.
            uiManager.ShowRPMPanel(false);
        }

        Invoke(nameof(StartRPM), 0.3f);
    }

    void StartRPM()
    {
        if (scenarioLoader == null) scenarioLoader = FindFirstObjectByType<ScenarioLoader>();
        if (rpmController == null) rpmController = FindFirstObjectByType<RPMSequenceController>();

        if (scenarioLoader != null && rpmController != null)
        {
            var scenario = scenarioLoader.GetActiveScenario();
            if (scenario != null)
            {
                // This triggers 'StartRPMAssessment', which activates the Respiration bubble
                rpmController.StartRPMAssessment(scenario);
                Debug.Log("RPM Sequence triggered cleanly! 3D world interactive bubbles are active.");
            }
        }
    }

    public void ResetPlacement()
    {
        _patientPlaced = false;
        if (objectSpawner != null)
        {
            objectSpawner.enabled = true;
        }
        if (uiManager != null)
            uiManager.ShowPlacementPrompt(true);
    }
}