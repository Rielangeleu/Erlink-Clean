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

<<<<<<< HEAD
=======
    // REPLACED: patientPlacer removed to use template's working spawner
>>>>>>> 26ca292180f2e5632fdb78b15fe5f649ef097e93
    [Header("AR Spawner System")]
    public MonoBehaviour objectSpawner;

    public PatientInfoCardUI patientInfoCardUI;
    public ScoringSystem scoringSystem;

<<<<<<< HEAD
    private RuntimeScenarioData _activeRuntimeScenario;
    private ScenarioData _activeScriptableScenario;

    void Start()
    {
        if (objectSpawner == null)
            FindObjectSpawnerDynamic();

        if (FindFirstObjectByType<ScenarioManager>() != null)
=======
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
>>>>>>> 26ca292180f2e5632fdb78b15fe5f649ef097e93
        {
            Debug.Log("ScenarioLoader: Deferring initialization to ScenarioManager.");
            return;
        }

<<<<<<< HEAD
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
=======
        // Fallback if testing the scene completely by itself
        ScenarioData toLoad = ScenarioSelector.SelectedScenario ?? scenarioToLoad;
        if (toLoad != null)
            LoadScenario(toLoad);
>>>>>>> 26ca292180f2e5632fdb78b15fe5f649ef097e93
    }

    void FindObjectSpawnerDynamic()
    {
<<<<<<< HEAD
        var allBehaviours = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None);
        foreach (MonoBehaviour mb in allBehaviours)
=======
        foreach (MonoBehaviour mb in FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None))
>>>>>>> 26ca292180f2e5632fdb78b15fe5f649ef097e93
        {
            if (mb != null && mb.GetType().Name == "ObjectSpawner")
            {
                objectSpawner = mb;
<<<<<<< HEAD
                Debug.Log($"Found ObjectSpawner: {mb.name}");
=======
>>>>>>> 26ca292180f2e5632fdb78b15fe5f649ef097e93
                break;
            }
        }
    }

<<<<<<< HEAD
    void AssignPrefabToSpawner(GameObject prefab)
    {
        if (objectSpawner == null)
            FindObjectSpawnerDynamic();
=======
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
>>>>>>> 26ca292180f2e5632fdb78b15fe5f649ef097e93

        if (objectSpawner != null)
        {
            try
            {
<<<<<<< HEAD
=======
                // The template's spawner holds prefabs inside a List or Array field named 'objectPrefabs' or 'm_ObjectPrefabs'
>>>>>>> 26ca292180f2e5632fdb78b15fe5f649ef097e93
                Type spawnerType = objectSpawner.GetType();
                FieldInfo prefabsField = spawnerType.GetField("objectPrefabs", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                                      ?? spawnerType.GetField("m_ObjectPrefabs", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

                if (prefabsField != null)
                {
<<<<<<< HEAD
                    var listType = typeof(System.Collections.Generic.List<>).MakeGenericType(typeof(GameObject));
                    var newList = Activator.CreateInstance(listType);
                    listType.GetMethod("Add").Invoke(newList, new object[] { prefab });
                    prefabsField.SetValue(objectSpawner, newList);
                    Debug.Log($"✅ Successfully injected {prefab.name} into ObjectSpawner!");
                }
                else
                {
                    Debug.LogError("Could not locate prefab collection field on ObjectSpawner!");
=======
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
>>>>>>> 26ca292180f2e5632fdb78b15fe5f649ef097e93
                }
            }
            catch (Exception e)
            {
<<<<<<< HEAD
                Debug.LogError($"Dynamic asset assignment failed: {e.Message}");
=======
                Debug.LogError($"ScenarioLoader: Dynamic asset assignment failed: {e.Message}");
>>>>>>> 26ca292180f2e5632fdb78b15fe5f649ef097e93
            }
        }
        else
        {
<<<<<<< HEAD
            Debug.LogError("ObjectSpawner was not found in the scene!");
        }
    }

    public ScenarioData GetActiveScenario() => _activeScriptableScenario;
    public RuntimeScenarioData GetActiveRuntimeScenario() => _activeRuntimeScenario;
=======
            Debug.LogError("ScenarioLoader: Cannot assign active prefab because ObjectSpawner was not found in the scene layout!");
        }
    }

    public ScenarioData GetActiveScenario() => _activeScenario;
>>>>>>> 26ca292180f2e5632fdb78b15fe5f649ef097e93
}