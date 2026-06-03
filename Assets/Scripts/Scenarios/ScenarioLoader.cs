using UnityEngine;
using System;
using System.Reflection;

public class ScenarioLoader : MonoBehaviour
{
    [Header("Fallback — used if no ScenarioSelector set")]
    public ScenarioData scenarioToLoad;

    [Header("Dependencies")]
    public UIManager uiManager;
    public RPMSequenceController rpmController;

    [Header("AR Spawner System")]
    public MonoBehaviour objectSpawner;

    public PatientInfoCardUI patientInfoCardUI;
    public ScoringSystem scoringSystem;

    private RuntimeScenarioData _activeRuntimeScenario;
    private ScenarioData _activeScriptableScenario;

    void Start()
    {
        if (objectSpawner == null)
            FindObjectSpawnerDynamic();

        if (FindFirstObjectByType<ScenarioManager>() != null)
        {
            Debug.Log("ScenarioLoader: Deferring initialization to ScenarioManager.");
            return;
        }

        // Use SelectedRuntimeScenario OR SelectedScenario (original)
        if (ScenarioSelector.SelectedRuntimeScenario != null)
        {
            LoadRuntimeScenario(ScenarioSelector.SelectedRuntimeScenario);
        }
        else if (ScenarioSelector.SelectedScenario != null)
        {
            LoadScenario(ScenarioSelector.SelectedScenario);
        }
        else if (scenarioToLoad != null)
        {
            LoadScenario(scenarioToLoad);
        }
        else
        {
            Debug.LogError("No scenario selected!");
        }
    }

    public void LoadScenario(ScenarioData scenario)
    {
        _activeScriptableScenario = scenario;
        _activeRuntimeScenario = null;

        Debug.Log($"Loading ScriptableObject scenario: {scenario.scenarioTitle}");
        Debug.Log($"Patient prefab: {scenario.patientPrefab?.name ?? "NULL"}");
        Debug.Log($"Priority fields - isMultiPatient: {scenario.isMultiPatient}, options: {(scenario.priorityOptions != null ? scenario.priorityOptions.Length : 0)}");

        if (uiManager != null)
            uiManager.SetScenarioTitle(scenario.scenarioTitle);

        if (patientInfoCardUI != null)
            patientInfoCardUI.PopulateFromScenario(scenario);

        if (scenario.patientPrefab != null)
        {
            AssignPrefabToSpawner(scenario.patientPrefab);
        }
        else
        {
            Debug.LogError("No patientPrefab assigned in ScriptableObject!");
        }

        if (scoringSystem != null)
        {
            scoringSystem.SetActiveScenario(scenario);
            scoringSystem.ResetScoring();
        }
    }

    public void LoadRuntimeScenario(RuntimeScenarioData scenario)
    {
        _activeRuntimeScenario = scenario;
        _activeScriptableScenario = null;

        Debug.Log($"Loading Runtime scenario from Firestore: {scenario.scenarioTitle}");
        Debug.Log($"Patient model name: {scenario.patientModelName}");
        Debug.Log($"Patient prefab: {(scenario.patientPrefab != null ? scenario.patientPrefab.name : "NULL")}");
        Debug.Log($"Priority fields - isMultiPatient: {scenario.isMultiPatient}, options: {(scenario.priorityOptions != null ? scenario.priorityOptions.Length : 0)}");
        
        if (scenario.priorityOptions != null && scenario.priorityOptions.Length > 0)
        {
            Debug.Log($"Priority options: {string.Join(", ", scenario.priorityOptions)}");
            Debug.Log($"Correct priority index: {scenario.correctPriorityIndex}");
        }
        else
        {
            Debug.LogWarning($"Runtime scenario '{scenario.scenarioTitle}' has NO priority options! Priority panel will not show.");
        }

        if (uiManager != null)
            uiManager.SetScenarioTitle(scenario.scenarioTitle);

        if (patientInfoCardUI != null)
            patientInfoCardUI.PopulateFromRuntimeScenario(scenario);

        if (scenario.patientPrefab != null)
        {
            Debug.Log($"✅ Using pre-loaded prefab: {scenario.patientPrefab.name}");
            AssignPrefabToSpawner(scenario.patientPrefab);
        }
        else
        {
            Debug.LogError("Patient prefab is null! Loading fallback...");
            
            GameObject fallback = Resources.Load<GameObject>("Patients/Patient_Standard");
            if (fallback != null)
            {
                Debug.Log($"Using fallback prefab: Patient_Standard");
                AssignPrefabToSpawner(fallback);
            }
            else
            {
                GameObject capsule = GameObject.CreatePrimitive(PrimitiveType.Capsule);
                capsule.name = "Fallback_Patient";
                Debug.LogWarning("Created fallback capsule patient");
                AssignPrefabToSpawner(capsule);
            }
        }

        if (scoringSystem != null)
        {
            scoringSystem.SetActiveRuntimeScenario(scenario);
            scoringSystem.ResetScoring();
        }
    }

    void FindObjectSpawnerDynamic()
    {
        var allBehaviours = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None);
        foreach (MonoBehaviour mb in allBehaviours)
        {
            if (mb != null && mb.GetType().Name == "ObjectSpawner")
            {
                objectSpawner = mb;
                Debug.Log($"Found ObjectSpawner: {mb.name}");
                break;
            }
        }
    }

    void AssignPrefabToSpawner(GameObject prefab)
    {
        if (objectSpawner == null)
            FindObjectSpawnerDynamic();

        if (objectSpawner != null)
        {
            try
            {
                Type spawnerType = objectSpawner.GetType();
                FieldInfo prefabsField = spawnerType.GetField("objectPrefabs", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                                      ?? spawnerType.GetField("m_ObjectPrefabs", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

                if (prefabsField != null)
                {
                    var listType = typeof(System.Collections.Generic.List<>).MakeGenericType(typeof(GameObject));
                    var newList = Activator.CreateInstance(listType);
                    listType.GetMethod("Add").Invoke(newList, new object[] { prefab });
                    prefabsField.SetValue(objectSpawner, newList);
                    Debug.Log($"✅ Successfully injected {prefab.name} into ObjectSpawner!");
                }
                else
                {
                    Debug.LogError("Could not locate prefab collection field on ObjectSpawner!");
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Dynamic asset assignment failed: {e.Message}");
            }
        }
        else
        {
            Debug.LogError("ObjectSpawner was not found in the scene!");
        }
    }

    public ScenarioData GetActiveScenario() => _activeScriptableScenario;
    public RuntimeScenarioData GetActiveRuntimeScenario() => _activeRuntimeScenario;
}