using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Manages multi-patient scenarios (Medium difficulty).
/// Tracks which patient is active and transitions between them.
/// </summary>
public class ScenarioManager : MonoBehaviour
{
    [Header("Easy Scenario")]
    public ScenarioData easyScenario;

    [Header("Medium Scenarios (2 patients)")]
    public ScenarioData mediumPatientA;
    public ScenarioData mediumPatientB;

    [Header("Dependencies")]
    public ScenarioLoader scenarioLoader;
    public UIManager uiManager;
    public TimerSystem timerSystem;

    [Header("State")]
    private int _currentPatientIndex = 0;
    private DifficultyLevel _activeDifficulty;
    private ScenarioData[] _activeScenarios;

    public static ScenarioManager Instance;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        // Load based on ScenarioSelector
        if (ScenarioSelector.SelectedScenario != null)
        {
            if (ScenarioSelector.SelectedDifficulty == DifficultyLevel.Medium)
            {
                StartMediumScenario();
            }
            else
            {
                StartEasyScenario();
            }
        }
        else
        {
            // Default: load easy for testing
            StartEasyScenario();
        }
    }

    public void StartEasyScenario()
    {
        _activeDifficulty = DifficultyLevel.Easy;
        _activeScenarios = new ScenarioData[] { easyScenario };
        _currentPatientIndex = 0;
        LoadCurrentPatient();
    }

    public void StartMediumScenario()
    {
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
        }

        scenarioLoader.LoadScenario(scenario);

        // Start timer
        if (timerSystem != null)
            timerSystem.StartTimer(scenario.timeLimitSeconds);
    }

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
            ObjectSpawnerBridge bridge = FindFirstObjectByType<ObjectSpawnerBridge>();
            if (bridge != null)
            {
                bridge.ResetPlacement();
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

    public bool IsLastPatient() => _currentPatientIndex >= _activeScenarios.Length - 1;

    public int GetCurrentPatientIndex() => _currentPatientIndex;

    public DifficultyLevel GetActiveDifficulty() => _activeDifficulty;

    void LoadFeedbackScene()
    {
        SceneManager.LoadScene("FeedbackScene");
    }
}