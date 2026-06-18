using UnityEngine;
using UnityEngine.SceneManagement;
<<<<<<< HEAD
using System.Collections;
using System.Reflection;
using System;

/// <summary>
/// Manages multi-patient scenarios (Medium and Hard difficulties).
=======

/// <summary>
/// Manages multi-patient scenarios (Medium difficulty).
>>>>>>> 26ca292180f2e5632fdb78b15fe5f649ef097e93
/// Tracks which patient is active and transitions between them.
/// </summary>
public class ScenarioManager : MonoBehaviour
{
    [Header("Easy Scenario")]
    public ScenarioData easyScenario;

    [Header("Medium Scenarios (2 patients)")]
    public ScenarioData mediumPatientA;
    public ScenarioData mediumPatientB;

<<<<<<< HEAD
    [Header("Hard Scenarios (3 patients)")]
    public ScenarioData hardPatientA;
    public ScenarioData hardPatientB;
    public ScenarioData hardPatientC;

=======
>>>>>>> 26ca292180f2e5632fdb78b15fe5f649ef097e93
    [Header("Dependencies")]
    public ScenarioLoader scenarioLoader;
    public UIManager uiManager;
    public TimerSystem timerSystem;

    [Header("State")]
    private int _currentPatientIndex = 0;
    private DifficultyLevel _activeDifficulty;
    private ScenarioData[] _activeScenarios;
<<<<<<< HEAD
    private bool _isFirebaseReady = false;
=======
>>>>>>> 26ca292180f2e5632fdb78b15fe5f649ef097e93

    public static ScenarioManager Instance;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
<<<<<<< HEAD
        StartCoroutine(WaitForFirebaseAndStart());
    }

    IEnumerator WaitForFirebaseAndStart()
    {
        Debug.Log("ScenarioManager: Waiting for Firebase scenarios to load...");
        
        int maxWait = 30;
        int waitCount = 0;
        
        while (waitCount < maxWait)
        {
            if (ScenarioSelector.SelectedScenario != null)
            {
                Debug.Log($"ScenarioManager: Found SelectedScenario - {ScenarioSelector.SelectedScenario.scenarioTitle}");
                break;
            }
            
            if (ScenarioSelectController.AllEasyScenarios != null && ScenarioSelectController.AllEasyScenarios.Count > 0)
            {
                Debug.Log("ScenarioManager: Firebase scenarios are ready!");
                break;
            }
            
            waitCount++;
            yield return new WaitForSeconds(0.5f);
        }
        
=======
        // Load based on ScenarioSelector
>>>>>>> 26ca292180f2e5632fdb78b15fe5f649ef097e93
        if (ScenarioSelector.SelectedScenario != null)
        {
            if (ScenarioSelector.SelectedDifficulty == DifficultyLevel.Medium)
            {
                StartMediumScenario();
            }
<<<<<<< HEAD
            else if (ScenarioSelector.SelectedDifficulty == DifficultyLevel.Hard)
            {
                StartHardScenario();
            }
=======
>>>>>>> 26ca292180f2e5632fdb78b15fe5f649ef097e93
            else
            {
                StartEasyScenario();
            }
        }
        else
        {
<<<<<<< HEAD
            Debug.Log("ScenarioManager: No selected scenario, using hardcoded fallback");
            if (ScenarioSelector.SelectedDifficulty == DifficultyLevel.Medium)
            {
                StartMediumScenario();
            }
            else if (ScenarioSelector.SelectedDifficulty == DifficultyLevel.Hard)
            {
                StartHardScenario();
            }
            else
            {
                StartEasyScenario();
            }
=======
            // Default: load easy for testing
            StartEasyScenario();
>>>>>>> 26ca292180f2e5632fdb78b15fe5f649ef097e93
        }
    }

    public void StartEasyScenario()
    {
<<<<<<< HEAD
        if (ScenarioSelector.SelectedScenario != null)
        {
            easyScenario = ScenarioSelector.SelectedScenario;
            Debug.Log($"ScenarioManager: Using selected easy scenario: {easyScenario.scenarioTitle}");
        }
        
        _activeDifficulty = DifficultyLevel.Easy;
        _activeScenarios = new ScenarioData[] { easyScenario };
        _currentPatientIndex = 0;
        Debug.Log($"Started Easy Scenario: {easyScenario?.scenarioTitle}");
=======
        _activeDifficulty = DifficultyLevel.Easy;
        _activeScenarios = new ScenarioData[] { easyScenario };
        _currentPatientIndex = 0;
>>>>>>> 26ca292180f2e5632fdb78b15fe5f649ef097e93
        LoadCurrentPatient();
    }

    public void StartMediumScenario()
    {
<<<<<<< HEAD
        if (ScenarioSelector.MediumPatients != null && ScenarioSelector.MediumPatients.Length >= 2)
        {
            mediumPatientA = ScenarioSelector.MediumPatients[0];
            mediumPatientB = ScenarioSelector.MediumPatients[1];
            Debug.Log($"ScenarioManager: Using selected medium scenarios: {mediumPatientA?.scenarioTitle}, {mediumPatientB?.scenarioTitle}");
        }
        
        _activeDifficulty = DifficultyLevel.Medium;
        _activeScenarios = new ScenarioData[] { mediumPatientA, mediumPatientB };
        _currentPatientIndex = 0;
        Debug.Log($"Started Medium Scenario - Patient A: {mediumPatientA?.scenarioTitle}, Patient B: {mediumPatientB?.scenarioTitle}");
        Debug.Log($"Patient A Prefab: {(mediumPatientA?.patientPrefab != null ? mediumPatientA.patientPrefab.name : "NULL")}");
        Debug.Log($"Patient B Prefab: {(mediumPatientB?.patientPrefab != null ? mediumPatientB.patientPrefab.name : "NULL")}");
        Debug.Log($"Priority Question: {mediumPatientA?.priorityQuestion}");
        Debug.Log($"Priority Options: {(mediumPatientA?.priorityOptions != null ? string.Join(", ", mediumPatientA.priorityOptions) : "NULL")}");
        
        ForceSpawnerPrefab(mediumPatientA?.patientPrefab);
        LoadCurrentPatient();
    }

    public void StartHardScenario()
    {
        if (ScenarioSelector.HardPatients != null && ScenarioSelector.HardPatients.Length >= 3)
        {
            hardPatientA = ScenarioSelector.HardPatients[0];
            hardPatientB = ScenarioSelector.HardPatients[1];
            hardPatientC = ScenarioSelector.HardPatients[2];
            Debug.Log($"ScenarioManager: Using selected hard scenarios: {hardPatientA?.scenarioTitle}, {hardPatientB?.scenarioTitle}, {hardPatientC?.scenarioTitle}");
        }
        
        _activeDifficulty = DifficultyLevel.Hard;
        _activeScenarios = new ScenarioData[] { hardPatientA, hardPatientB, hardPatientC };
        _currentPatientIndex = 0;
        Debug.Log($"Started Hard Scenario - Patient A: {hardPatientA?.scenarioTitle}, Patient B: {hardPatientB?.scenarioTitle}, Patient C: {hardPatientC?.scenarioTitle}");
        Debug.Log($"Patient A Prefab: {(hardPatientA?.patientPrefab != null ? hardPatientA.patientPrefab.name : "NULL")}");
        Debug.Log($"Patient B Prefab: {(hardPatientB?.patientPrefab != null ? hardPatientB.patientPrefab.name : "NULL")}");
        Debug.Log($"Patient C Prefab: {(hardPatientC?.patientPrefab != null ? hardPatientC.patientPrefab.name : "NULL")}");
        Debug.Log($"Priority Question: {hardPatientA?.priorityQuestion}");
        Debug.Log($"Priority Options: {(hardPatientA?.priorityOptions != null ? string.Join(", ", hardPatientA.priorityOptions) : "NULL")}");
        Debug.Log($"Correct Priority Index: {hardPatientA?.correctPriorityIndex}");
        
        ForceSpawnerPrefab(hardPatientA?.patientPrefab);
        LoadCurrentPatient();
    }
    
    private void ForceSpawnerPrefab(GameObject prefab)
    {
        if (prefab == null) return;
        
        ObjectSpawnerBridge bridge = FindFirstObjectByType<ObjectSpawnerBridge>();
        if (bridge != null)
        {
            var objectSpawner = bridge.GetType().GetField("objectSpawner", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)?.GetValue(bridge) as MonoBehaviour;
            if (objectSpawner != null)
            {
                Type spawnerType = objectSpawner.GetType();
                FieldInfo prefabsField = spawnerType.GetField("objectPrefabs", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                                      ?? spawnerType.GetField("m_ObjectPrefabs", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                
                if (prefabsField != null)
                {
                    var list = prefabsField.GetValue(objectSpawner);
                    if (list != null)
                    {
                        var clearMethod = list.GetType().GetMethod("Clear");
                        clearMethod?.Invoke(list, null);
                        var addMethod = list.GetType().GetMethod("Add");
                        addMethod?.Invoke(list, new object[] { prefab });
                        Debug.Log($"Forced ObjectSpawner to use prefab: {prefab.name}");
                    }
                }
            }
        }
    }

    void LoadCurrentPatient()
    {
        if (_activeScenarios == null || _currentPatientIndex >= _activeScenarios.Length)
        {
            Debug.LogError($"Cannot load patient {_currentPatientIndex} - array null or index out of range");
            return;
        }
        
        ScenarioData scenario = _activeScenarios[_currentPatientIndex];
        
        if (scenario == null)
        {
            Debug.LogError($"Scenario at index {_currentPatientIndex} is null!");
            return;
        }

        if (_activeDifficulty == DifficultyLevel.Medium || _activeDifficulty == DifficultyLevel.Hard)
        {
            string patientLetter = GetPatientLetter(_currentPatientIndex);
            uiManager.SetScenarioTitle($"Patient {patientLetter} — " + scenario.scenarioTitle);
            Debug.Log($"Loading Patient {patientLetter}: {scenario.scenarioTitle} (Prefab: {scenario.patientPrefab?.name})");
            Debug.Log($"  - Priority Question: {scenario.priorityQuestion}");
            Debug.Log($"  - Priority Options: {(scenario.priorityOptions != null ? string.Join(", ", scenario.priorityOptions) : "NULL")}");
        }
        else
        {
            uiManager.SetScenarioTitle(scenario.scenarioTitle);
=======
        _activeDifficulty = DifficultyLevel.Medium;
        _activeScenarios = new ScenarioData[] { mediumPatientA, mediumPatientB };
        _currentPatientIndex = 0;
        LoadCurrentPatient();
    }

    void LoadCurrentPatient()
    {
        ScenarioData scenario = _activeScenarios[_currentPatientIndex];

        // Show patient number for medium
        if (_activeDifficulty == DifficultyLevel.Medium)
        {
            uiManager.SetScenarioTitle(
                $"Patient {_currentPatientIndex + 1} of 2 — " + scenario.scenarioTitle);
>>>>>>> 26ca292180f2e5632fdb78b15fe5f649ef097e93
        }

        scenarioLoader.LoadScenario(scenario);

<<<<<<< HEAD
=======
        // Start timer
>>>>>>> 26ca292180f2e5632fdb78b15fe5f649ef097e93
        if (timerSystem != null)
            timerSystem.StartTimer(scenario.timeLimitSeconds);
    }

<<<<<<< HEAD
    public string GetPatientLetter(int index)
    {
        switch (index)
        {
            case 0: return "A";
            case 1: return "B";
            case 2: return "C";
            default: return (index + 1).ToString();
        }
    }

    public string GetCurrentPatientName()
    {
        if (_activeDifficulty == DifficultyLevel.Medium)
        {
            return $"Patient {GetPatientLetter(_currentPatientIndex)}";
        }
        else if (_activeDifficulty == DifficultyLevel.Hard)
        {
            return $"Patient {GetPatientLetter(_currentPatientIndex)}";
        }
        return "Patient";
    }

    public void LoadNextPatient()
    {
        _currentPatientIndex++;
        Debug.Log($"Loading next patient - Index: {_currentPatientIndex}, Total: {_activeScenarios?.Length}");

        if (_currentPatientIndex < _activeScenarios.Length)
        {
=======
    /// <summary>
    /// Called after Patient A is done in Medium scenario.
    /// Loads Patient B.
    /// </summary>
    public void LoadNextPatient()
    {
        _currentPatientIndex++;

        if (_currentPatientIndex < _activeScenarios.Length)
        {
            // Reset the template spawner bridge status so it allows spawning the next patient model
>>>>>>> 26ca292180f2e5632fdb78b15fe5f649ef097e93
            ObjectSpawnerBridge bridge = FindFirstObjectByType<ObjectSpawnerBridge>();
            if (bridge != null)
            {
                bridge.ResetPlacement();
<<<<<<< HEAD
                Debug.Log("ObjectSpawnerBridge reset for next patient");
            }
            
            // Small delay to ensure spawner is reset before loading next patient
            Invoke(nameof(LoadCurrentPatientWithDelay), 0.8f);
        }
        else
        {
            Debug.Log("All patients done - loading feedback scene");
            LoadFeedbackScene();
        }
    }
    
    void LoadCurrentPatientWithDelay()
    {
        LoadCurrentPatient();
    }
=======
            }

            // Small delay before next patient
            Invoke(nameof(LoadCurrentPatient), 1.5f);
        }
        else
        {
            // All patients done — go to feedback
            LoadFeedbackScene();
        }
    }
>>>>>>> 26ca292180f2e5632fdb78b15fe5f649ef097e93

    public bool IsLastPatient() => _currentPatientIndex >= _activeScenarios.Length - 1;

    public int GetCurrentPatientIndex() => _currentPatientIndex;
<<<<<<< HEAD
    
    public DifficultyLevel GetActiveDifficulty() => _activeDifficulty;
    
    public int GetTotalPatientCount() => _activeScenarios != null ? _activeScenarios.Length : 1;
=======

    public DifficultyLevel GetActiveDifficulty() => _activeDifficulty;
>>>>>>> 26ca292180f2e5632fdb78b15fe5f649ef097e93

    void LoadFeedbackScene()
    {
        SceneManager.LoadScene("FeedbackScene");
    }
}