using UnityEngine;

/// <summary>
/// Passed between scenes to tell ScenarioLoader
/// which scenario to load.
/// Uses static variables so data persists between scenes.
/// </summary>
public static class ScenarioSelector
{
    public static ScenarioData SelectedScenario;
    public static DifficultyLevel SelectedDifficulty;

    // For medium — tracks which patient we're on
    public static int CurrentPatientIndex = 0;
    public static ScenarioData[] MediumPatients;

    public static void SelectEasy(ScenarioData scenario)
    {
        SelectedScenario = scenario;
        SelectedDifficulty = DifficultyLevel.Easy;
        CurrentPatientIndex = 0;
        MediumPatients = null;
    }

    public static void SelectMedium(
        ScenarioData patientA,
        ScenarioData patientB)
    {
        MediumPatients = new ScenarioData[]
            { patientA, patientB };
        SelectedScenario = patientA; // Start with Patient A
        SelectedDifficulty = DifficultyLevel.Medium;
        CurrentPatientIndex = 0;
    }

    public static ScenarioData GetNextMediumPatient()
    {
        if (MediumPatients == null) return null;
        CurrentPatientIndex++;
        if (CurrentPatientIndex < MediumPatients.Length)
            return MediumPatients[CurrentPatientIndex];
        return null; // All patients done
    }

    public static bool HasNextPatient()
    {
        if (MediumPatients == null) return false;
        return CurrentPatientIndex + 1 < MediumPatients.Length;
    }

    public static void Reset()
    {
        SelectedScenario = null;
        CurrentPatientIndex = 0;
        MediumPatients = null;
    }
}