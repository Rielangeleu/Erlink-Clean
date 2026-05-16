using UnityEngine;

public class ScenarioLoader : MonoBehaviour
{
    [Header("Fallback — used if no ScenarioSelector set")]
    public ScenarioData scenarioToLoad;

    [Header("Dependencies")]
    public UIManager uiManager;
    public RPMSequenceController rpmController;
    public PatientPlacer patientPlacer;
    public PatientInfoCardUI patientInfoCardUI;
    public ScoringSystem scoringSystem;

    private ScenarioData _activeScenario;

    void Start()
    {
        ScenarioData toLoad =
            ScenarioSelector.SelectedScenario ?? scenarioToLoad;

        if (toLoad != null)
            LoadScenario(toLoad);
    }

    public void LoadScenario(ScenarioData scenario)
    {
        _activeScenario = scenario;

        // ADD THESE DEBUG LINES:
        Debug.Log($"Loading scenario: {scenario.scenarioTitle}");
        Debug.Log($"Patient prefab: {scenario.patientPrefab?.name ?? "NULL"}");

        // Update UI title
        uiManager.SetScenarioTitle(scenario.scenarioTitle);

        // Update patient info card dynamically
        if (patientInfoCardUI != null)
            patientInfoCardUI.PopulateFromScenario(scenario);

        // This is the key line — make sure it's here:
        if (scenario.patientPrefab != null)
            patientPlacer.patientPrefab = scenario.patientPrefab;
        else
            Debug.LogError("ScenarioLoader: Patient prefab is NULL in ScriptableObject!");

        if (scoringSystem != null)
            scoringSystem.SetActiveScenario(scenario);
        if (scoringSystem != null)
            scoringSystem.ResetScoring();

        patientPlacer.OnPatientPlaced -= StartRPMAssessment;
        patientPlacer.OnPatientPlaced += StartRPMAssessment;

        Debug.Log($"Loaded scenario: {scenario.scenarioTitle}");
    }

    void StartRPMAssessment()
    {
        // Unsubscribe to prevent duplicate calls
        patientPlacer.OnPatientPlaced -= StartRPMAssessment;
        rpmController.StartRPMAssessment(_activeScenario);
    }

    public ScenarioData GetActiveScenario() => _activeScenario;
}