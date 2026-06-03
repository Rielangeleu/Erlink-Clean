using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Passed between scenes to tell ScenarioLoader which scenario to load.
/// </summary>
public static class ScenarioSelector
{
    // Original properties - KEEP THESE (used by existing code)
    public static ScenarioData SelectedScenario;
    public static DifficultyLevel SelectedDifficulty;

    // For medium — tracks which patient we're on
    public static int CurrentPatientIndex = 0;
    public static ScenarioData[] MediumPatients;
    
    // For hard — tracks which patient we're on (3 patients)
    public static ScenarioData[] HardPatients;

    // For Firestore runtime scenarios
    public static RuntimeScenarioData SelectedRuntimeScenario;

    public static void SelectEasy(ScenarioData scenario)
    {
        SelectedScenario = scenario;
        SelectedRuntimeScenario = null;
        SelectedDifficulty = DifficultyLevel.Easy;
        CurrentPatientIndex = 0;
        MediumPatients = null;
        HardPatients = null;
        Debug.Log($"ScenarioSelector: Easy scenario selected - {scenario?.scenarioTitle}");
    }
    
    public static void SelectEasy(RuntimeScenarioData scenario)
    {
        SelectedScenario = null;
        SelectedRuntimeScenario = scenario;
        SelectedDifficulty = DifficultyLevel.Easy;
        CurrentPatientIndex = 0;
        MediumPatients = null;
        HardPatients = null;
        Debug.Log($"ScenarioSelector: Easy runtime scenario selected - {scenario?.scenarioTitle}");
    }

    public static void SelectMedium(ScenarioData patientA, ScenarioData patientB)
    {
        SelectedScenario = patientA;
        SelectedRuntimeScenario = null;
        SelectedDifficulty = DifficultyLevel.Medium;
        CurrentPatientIndex = 0;
        MediumPatients = new ScenarioData[] { patientA, patientB };
        HardPatients = null;
        Debug.Log($"ScenarioSelector: Medium scenario selected - Patient A: {patientA?.scenarioTitle}, Patient B: {patientB?.scenarioTitle}");
        Debug.Log($"  - Patient A Priority Question: {patientA?.priorityQuestion}");
        Debug.Log($"  - Patient A Priority Options: {(patientA?.priorityOptions != null ? string.Join(", ", patientA.priorityOptions) : "NULL")}");
        Debug.Log($"  - Patient B Priority Options: {(patientB?.priorityOptions != null ? string.Join(", ", patientB.priorityOptions) : "NULL")}");
    }

    public static void SelectMedium(RuntimeScenarioData patientA, RuntimeScenarioData patientB)
    {
        SelectedScenario = null;
        SelectedRuntimeScenario = patientA;
        SelectedDifficulty = DifficultyLevel.Medium;
        CurrentPatientIndex = 0;
        MediumPatients = null;
        HardPatients = null;
        Debug.Log($"ScenarioSelector: Medium runtime scenario selected - Patient A: {patientA?.scenarioTitle}, Patient B: {patientB?.scenarioTitle}");
    }
    
    public static void SelectHard(ScenarioData patientA, ScenarioData patientB, ScenarioData patientC)
    {
        SelectedScenario = patientA;
        SelectedRuntimeScenario = null;
        SelectedDifficulty = DifficultyLevel.Hard;
        CurrentPatientIndex = 0;
        MediumPatients = null;
        HardPatients = new ScenarioData[] { patientA, patientB, patientC };
        Debug.Log($"ScenarioSelector: Hard scenario selected - Patient A: {patientA?.scenarioTitle}, Patient B: {patientB?.scenarioTitle}, Patient C: {patientC?.scenarioTitle}");
        Debug.Log($"  - Priority Question: {patientA?.priorityQuestion}");
        Debug.Log($"  - Priority Options: {(patientA?.priorityOptions != null ? string.Join(", ", patientA.priorityOptions) : "NULL")}");
        Debug.Log($"  - Correct Priority Index: {patientA?.correctPriorityIndex}");
    }
    
    public static void SelectHard(RuntimeScenarioData patientA, RuntimeScenarioData patientB, RuntimeScenarioData patientC)
    {
        SelectedScenario = null;
        SelectedRuntimeScenario = patientA;
        SelectedDifficulty = DifficultyLevel.Hard;
        CurrentPatientIndex = 0;
        MediumPatients = null;
        HardPatients = null;
        Debug.Log($"ScenarioSelector: Hard runtime scenario selected - Patient A: {patientA?.scenarioTitle}, Patient B: {patientB?.scenarioTitle}, Patient C: {patientC?.scenarioTitle}");
    }

    public static ScenarioData GetNextMediumPatient()
    {
        if (MediumPatients == null) return null;
        CurrentPatientIndex++;
        if (CurrentPatientIndex < MediumPatients.Length)
            return MediumPatients[CurrentPatientIndex];
        return null;
    }
    
    public static ScenarioData GetNextHardPatient()
    {
        if (HardPatients == null) return null;
        CurrentPatientIndex++;
        if (CurrentPatientIndex < HardPatients.Length)
            return HardPatients[CurrentPatientIndex];
        return null;
    }

    public static bool HasNextPatient()
    {
        if (MediumPatients != null)
            return CurrentPatientIndex + 1 < MediumPatients.Length;
        if (HardPatients != null)
            return CurrentPatientIndex + 1 < HardPatients.Length;
        return false;
    }
    
    public static bool HasNextMediumPatient()
    {
        if (MediumPatients == null) return false;
        return CurrentPatientIndex + 1 < MediumPatients.Length;
    }
    
    public static bool HasNextHardPatient()
    {
        if (HardPatients == null) return false;
        return CurrentPatientIndex + 1 < HardPatients.Length;
    }

    public static void Reset()
    {
        SelectedScenario = null;
        SelectedRuntimeScenario = null;
        CurrentPatientIndex = 0;
        MediumPatients = null;
        HardPatients = null;
        Debug.Log("ScenarioSelector: Reset called");
    }
}