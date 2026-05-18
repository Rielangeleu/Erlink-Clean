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

    // REPLACED: patientPlacer removed to use template's working spawner
    [Header("AR Spawner System")]
    public MonoBehaviour objectSpawner;

    public PatientInfoCardUI patientInfoCardUI;
    public ScoringSystem scoringSystem;

    private ScenarioData _activeScenario;

    void Start()
    {
        // Automatically look for the template's ObjectSpawner if not assigned in Inspector
        if (objectSpawner == null)
        {
            FindObjectSpawnerDynamic();
        }

        // If ScenarioManager is in the scene, let IT call LoadScenario instead!
        if (FindAnyObjectByType<ScenarioManager>() != null)
        {
            Debug.Log("ScenarioLoader: Deferring initialization to ScenarioManager.");
            return;
        }

        // Fallback if testing the scene completely by itself
        ScenarioData toLoad = ScenarioSelector.SelectedScenario ?? scenarioToLoad;
        if (toLoad != null)
            LoadScenario(toLoad);
    }

    void FindObjectSpawnerDynamic()
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

    public void LoadScenario(ScenarioData scenario)
    {
        _activeScenario = scenario;

        Debug.Log($"Loading scenario: {scenario.scenarioTitle}");
        Debug.Log($"Patient prefab: {scenario.patientPrefab?.name ?? "NULL"}");

        // Update UI title
        uiManager.SetScenarioTitle(scenario.scenarioTitle);

        // Update patient info card dynamically
        if (patientInfoCardUI != null)
            patientInfoCardUI.PopulateFromScenario(scenario);

        // Inject the active 3D prefab into the AR template's spawner system dynamically
        if (scenario.patientPrefab != null)
        {
            AssignPrefabToSpawner(scenario.patientPrefab);
        }
        else
        {
            Debug.LogError("ScenarioLoader: Patient prefab is NULL in ScriptableObject!");
        }

        if (scoringSystem != null)
            scoringSystem.SetActiveScenario(scenario);
        if (scoringSystem != null)
            scoringSystem.ResetScoring();

        Debug.Log($"Loaded scenario: {scenario.scenarioTitle}");
    }

    void AssignPrefabToSpawner(GameObject prefab)
    {
        if (objectSpawner == null) FindObjectSpawnerDynamic();

        if (objectSpawner != null)
        {
            try
            {
                // The template's spawner holds prefabs inside a List or Array field named 'objectPrefabs' or 'm_ObjectPrefabs'
                Type spawnerType = objectSpawner.GetType();
                FieldInfo prefabsField = spawnerType.GetField("objectPrefabs", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                                      ?? spawnerType.GetField("m_ObjectPrefabs", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

                if (prefabsField != null)
                {
                    // Create a new list structure containing just our active scenario's patient model
                    var listType = typeof(System.Collections.Generic.List<>).MakeGenericType(typeof(GameObject));
                    var newList = Activator.CreateInstance(listType);
                    listType.GetMethod("Add").Invoke(newList, new object[] { prefab });

                    // Inject the new list right back into the template system component
                    prefabsField.SetValue(objectSpawner, newList);
                    Debug.Log($"ScenarioLoader: Successfully injected {prefab.name} into template ObjectSpawner! ✅");
                }
                else
                {
                    Debug.LogError("ScenarioLoader: Could not locate a prefab collection field on the template ObjectSpawner component.");
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"ScenarioLoader: Dynamic asset assignment failed: {e.Message}");
            }
        }
        else
        {
            Debug.LogError("ScenarioLoader: Cannot assign active prefab because ObjectSpawner was not found in the scene layout!");
        }
    }

    public ScenarioData GetActiveScenario() => _activeScenario;
}