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
            uiManager.ShowRPMPanel(false);
        }

        Invoke(nameof(StartRPM), 0.3f);
    }

    void StartRPM()
    {
        Debug.Log("ObjectSpawnerBridge: StartRPM called");
        
        if (scenarioLoader == null) 
        {
            scenarioLoader = FindFirstObjectByType<ScenarioLoader>();
            Debug.Log($"ScenarioLoader found: {(scenarioLoader != null ? "YES" : "NO")}");
        }
        
        if (rpmController == null) 
        {
            rpmController = FindFirstObjectByType<RPMSequenceController>();
            Debug.Log($"RPMController found: {(rpmController != null ? "YES" : "NO")}");
        }

        if (scenarioLoader != null && rpmController != null)
        {
            var runtimeScenario = scenarioLoader.GetActiveRuntimeScenario();
            if (runtimeScenario != null)
            {
                Debug.Log($"✅ Loading RUNTIME scenario in RPM: {runtimeScenario.scenarioTitle}");
                rpmController.StartRPMAssessment(runtimeScenario);
                Debug.Log("RPM Sequence triggered for RUNTIME scenario!");
                return;
            }
            
            var scriptableScenario = scenarioLoader.GetActiveScenario();
            if (scriptableScenario != null)
            {
                Debug.Log($"Loading ScriptableObject scenario in RPM: {scriptableScenario.scenarioTitle}");
                rpmController.StartRPMAssessment(scriptableScenario);
                Debug.Log("RPM Sequence triggered for ScriptableObject scenario!");
                return;
            }
            
            Debug.LogError("ObjectSpawnerBridge: No active scenario found in ScenarioLoader!");
        }
        else
        {
            Debug.LogError($"ObjectSpawnerBridge: Missing references - scenarioLoader={scenarioLoader != null}, rpmController={rpmController != null}");
        }
    }

    public void ResetPlacement()
    {
        Debug.Log($"ObjectSpawnerBridge.ResetPlacement called - Current state: _patientPlaced={_patientPlaced}");
        
        _patientPlaced = false;
        
        if (objectSpawner != null)
        {
            objectSpawner.enabled = true;
            Debug.Log("ObjectSpawner re-enabled");
        }
        
        // CRITICAL: Clear and re-inject the correct prefab for the next patient
        if (scenarioLoader != null)
        {
            // Get the current active scenario (which should be Patient B now)
            var currentScenario = scenarioLoader.GetActiveScenario();
            if (currentScenario != null && currentScenario.patientPrefab != null)
            {
                Debug.Log($"Re-injecting prefab for next patient: {currentScenario.patientPrefab.name}");
                ReassignPrefabToSpawner(currentScenario.patientPrefab);
            }
            else
            {
                Debug.LogWarning("No current scenario or patient prefab found for next patient");
            }
        }
        
        if (uiManager != null)
        {
            uiManager.ShowPlacementPrompt(true);
        }
    }
    
    private void ReassignPrefabToSpawner(GameObject prefab)
    {
        if (objectSpawner == null)
        {
            Debug.LogError("ObjectSpawner is null, cannot reassign prefab");
            return;
        }
        
        try
        {
            Type spawnerType = objectSpawner.GetType();
            
            // Find the prefabs list field
            FieldInfo prefabsField = spawnerType.GetField("objectPrefabs", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                                  ?? spawnerType.GetField("m_ObjectPrefabs", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            
            if (prefabsField != null)
            {
                // Get the current list
                var list = prefabsField.GetValue(objectSpawner);
                
                if (list != null)
                {
                    // Clear existing list
                    var clearMethod = list.GetType().GetMethod("Clear");
                    if (clearMethod != null)
                    {
                        clearMethod.Invoke(list, null);
                        Debug.Log("Cleared existing prefabs from ObjectSpawner");
                    }
                    
                    // Add new prefab
                    var addMethod = list.GetType().GetMethod("Add");
                    if (addMethod != null)
                    {
                        addMethod.Invoke(list, new object[] { prefab });
                        Debug.Log($"✅ Re-injected new prefab into ObjectSpawner: {prefab.name}");
                    }
                }
                else
                {
                    // If list is null, create a new list
                    var listType = typeof(System.Collections.Generic.List<>).MakeGenericType(typeof(GameObject));
                    var newList = Activator.CreateInstance(listType);
                    var addMethod = listType.GetMethod("Add");
                    addMethod?.Invoke(newList, new object[] { prefab });
                    prefabsField.SetValue(objectSpawner, newList);
                    Debug.Log($"✅ Created new prefab list and injected: {prefab.name}");
                }
            }
            else
            {
                Debug.LogError("Could not locate prefab collection field on ObjectSpawner!");
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to reassign prefab to spawner: {e.Message}");
        }
    }
}