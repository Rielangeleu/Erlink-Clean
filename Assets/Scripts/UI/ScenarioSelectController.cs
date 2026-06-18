using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
<<<<<<< HEAD
using System.Collections;
using System.Collections.Generic;
using System.Linq;

=======

/// <summary>
/// Scenario selection screen.
/// Sets ScenarioSelector before loading MainAR.
/// </summary>
>>>>>>> 26ca292180f2e5632fdb78b15fe5f649ef097e93
public class ScenarioSelectController : MonoBehaviour
{
    [Header("Scenario References")]
    public ScenarioData easyScenario;
    public ScenarioData mediumPatientA;
    public ScenarioData mediumPatientB;
<<<<<<< HEAD
    public ScenarioData hardPatientA;
    public ScenarioData hardPatientB;
    public ScenarioData hardPatientC;
=======
>>>>>>> 26ca292180f2e5632fdb78b15fe5f649ef097e93

    [Header("Buttons")]
    public Button startEasyButton;
    public Button startMediumButton;
<<<<<<< HEAD
    public Button startHardButton;
=======
>>>>>>> 26ca292180f2e5632fdb78b15fe5f649ef097e93
    public Button backButton;

    [Header("Hard Card")]
    public GameObject hardCardLockOverlay;
<<<<<<< HEAD
    public GameObject hardCardTagsRow;  // Reference to the tags row (circles)
    public GameObject hardCardStartButton; // Reference to the Hard card start button GameObject
    public TextMeshProUGUI hardCardDescriptionText; // Reference to the Hard card description text

    [Header("Card Description Texts")]
    public string defaultHardDescription = "Coming soon — Future scope";
    public string unlockedHardDescription = "3 patients · 8 minutes";

    public static List<ScenarioData> AllEasyScenarios = new List<ScenarioData>();
    public static List<ScenarioData> AllMediumScenarios = new List<ScenarioData>();
    public static List<ScenarioData> AllHardScenarios = new List<ScenarioData>();
    public static int CurrentEasyIndex = 0;
    public static int CurrentMediumIndex = 0;
    public static int CurrentHardIndex = 0;

    void Start()
    {
        Debug.Log("ScenarioSelectController: Start called");
        
        StartCoroutine(LoadFirebaseScenarios());
        
=======

    void Start()
    {
>>>>>>> 26ca292180f2e5632fdb78b15fe5f649ef097e93
        if (startEasyButton != null)
            startEasyButton.onClick.AddListener(StartEasy);

        if (startMediumButton != null)
            startMediumButton.onClick.AddListener(StartMedium);

<<<<<<< HEAD
        if (startHardButton != null)
            startHardButton.onClick.AddListener(StartHard);

        if (backButton != null)
            backButton.onClick.AddListener(() => SceneManager.LoadScene("DashboardScene"));

        // Initially hide Hard card content (show lock) and set default description
        SetHardCardActive(false);
        UpdateHardCardDescription(false);
    }
    
    private void SetHardCardActive(bool hasHardScenarios)
    {
        if (hasHardScenarios)
        {
            // Hard scenarios available - unlock the card
            if (hardCardLockOverlay != null)
                hardCardLockOverlay.SetActive(false);
            
            if (hardCardTagsRow != null)
                hardCardTagsRow.SetActive(true);
            
            // Note: We don't touch hardCardStartButton because it's the same as startHardButton's GameObject
            // The startHardButton is already active, we just enable its interactivity
            
            // Also enable the startHardButton component interactivity
            if (startHardButton != null)
                startHardButton.interactable = true;
            
            Debug.Log("Hard card unlocked - showing tags and start button");
        }
        else
        {
            // No hard scenarios - show lock and hide content
            if (hardCardLockOverlay != null)
                hardCardLockOverlay.SetActive(true);
            
            if (hardCardTagsRow != null)
                hardCardTagsRow.SetActive(false);
            
            // Also disable the startHardButton component interactivity
            if (startHardButton != null)
                startHardButton.interactable = false;
            
            Debug.Log("Hard card locked - no hard scenarios available");
        }
    }
    
    private void UpdateHardCardDescription(bool hasHardScenarios)
    {
        if (hardCardDescriptionText != null)
        {
            if (hasHardScenarios)
            {
                hardCardDescriptionText.text = unlockedHardDescription;
                Debug.Log($"Hard card description updated to: {unlockedHardDescription}");
            }
            else
            {
                hardCardDescriptionText.text = defaultHardDescription;
                Debug.Log($"Hard card description set to: {defaultHardDescription}");
            }
        }
        else
        {
            Debug.LogWarning("Hard card description TextMeshProUGUI not assigned!");
        }
    }
    
    IEnumerator LoadFirebaseScenarios()
    {
        Debug.Log("=== LOADING FIREBASE SCENARIOS ===");
        
        int waitCount = 0;
        while (FirestoreScenarioLoader.Instance == null)
        {
            waitCount++;
            Debug.Log($"Waiting for FirestoreScenarioLoader... ({waitCount})");
            yield return new WaitForSeconds(0.5f);
            
            if (waitCount > 20)
            {
                Debug.LogError("Timeout waiting for FirestoreScenarioLoader! Using fallback.");
                UseFallbackScenarios();
                yield break;
            }
        }
        
        Debug.Log("FirestoreScenarioLoader found!");
        
        waitCount = 0;
        while (!FirebaseManager.IsInitialized)
        {
            waitCount++;
            Debug.Log($"Waiting for Firebase... ({waitCount})");
            yield return new WaitForSeconds(0.5f);
            
            if (waitCount > 20)
            {
                Debug.LogError("Timeout waiting for Firebase! Using fallback.");
                UseFallbackScenarios();
                yield break;
            }
        }
        
        Debug.Log("Firebase is ready!");
        
        var easyTask = FirestoreScenarioLoader.Instance.GetScenarios(DifficultyLevel.Easy);
        var mediumTask = FirestoreScenarioLoader.Instance.GetScenarios(DifficultyLevel.Medium);
        var hardTask = FirestoreScenarioLoader.Instance.GetScenarios(DifficultyLevel.Hard);
        
        while (!easyTask.IsCompleted || !mediumTask.IsCompleted || !hardTask.IsCompleted)
        {
            yield return null;
        }
        
        var easyScenarios = FirestoreScenarioLoader.Instance.GetCachedScenarios(DifficultyLevel.Easy);
        var mediumScenarios = FirestoreScenarioLoader.Instance.GetCachedScenarios(DifficultyLevel.Medium);
        var hardScenarios = FirestoreScenarioLoader.Instance.GetCachedScenarios(DifficultyLevel.Hard);
        
        Debug.Log($"Easy scenarios count: {easyScenarios?.Count ?? 0}");
        Debug.Log($"Medium scenarios count: {mediumScenarios?.Count ?? 0}");
        Debug.Log($"Hard scenarios count: {hardScenarios?.Count ?? 0}");
        
        // Track if we have any hard scenarios
        bool hasHardScenarios = (hardScenarios != null && hardScenarios.Count > 0);
        
        // Process Easy Scenarios
        if (easyScenarios != null && easyScenarios.Count > 0)
        {
            var sortedEasy = easyScenarios.OrderBy(s => s.orderIndex).ToList();
            
            Debug.Log("Easy scenarios sorted by orderIndex:");
            foreach (var runtime in sortedEasy)
            {
                Debug.Log($"  - {runtime.scenarioTitle} (orderIndex: {runtime.orderIndex})");
            }
            
            AllEasyScenarios = new List<ScenarioData>();
            foreach (var runtime in sortedEasy)
            {
                var temp = ScriptableObject.CreateInstance<ScenarioData>();
                temp.scenarioTitle = runtime.scenarioTitle;
                temp.scenarioID = runtime.scenarioID;
                temp.difficulty = runtime.difficulty;
                temp.timeLimitSeconds = runtime.timeLimitSeconds;
                temp.patientAge = runtime.patientAge;
                temp.patientGender = runtime.patientGender;
                temp.patientPresentation = runtime.patientPresentation;
                temp.vitalSigns = runtime.vitalSigns;
                temp.rpmAssessment = runtime.rpmAssessment;
                temp.correctTriageCategory = runtime.correctTriageCategory;
                temp.ehrActions = runtime.ehrActions;
                temp.clinicalExplanation = runtime.clinicalExplanation;
                temp.patientPrefab = runtime.patientPrefab;
                temp.priorityQuestion = runtime.priorityQuestion;
                temp.priorityOptions = runtime.priorityOptions;
                temp.correctPriorityIndex = runtime.correctPriorityIndex;
                temp.isMultiPatient = runtime.isMultiPatient;
                temp.orderIndex = runtime.orderIndex;
                
                AllEasyScenarios.Add(temp);
            }
            
            if (AllEasyScenarios.Count > 0)
            {
                easyScenario = AllEasyScenarios[0];
                Debug.Log($"✅ Selected FIRST easy scenario: {easyScenario.scenarioTitle}");
            }
        }
        else
        {
            UseFallbackScenarios();
        }
        
        // Process Medium Scenarios - Handle single document with multiple patients
        if (mediumScenarios != null && mediumScenarios.Count > 0)
        {
            Debug.Log($"Processing {mediumScenarios.Count} medium scenario(s) from Firestore");
            
            // Store all medium scenarios
            foreach (var runtime in mediumScenarios)
            {
                var temp = ScriptableObject.CreateInstance<ScenarioData>();
                temp.scenarioTitle = runtime.scenarioTitle;
                temp.scenarioID = runtime.scenarioID;
                temp.difficulty = runtime.difficulty;
                temp.timeLimitSeconds = runtime.timeLimitSeconds;
                temp.patientAge = runtime.patientAge;
                temp.patientGender = runtime.patientGender;
                temp.patientPresentation = runtime.patientPresentation;
                temp.vitalSigns = runtime.vitalSigns;
                temp.rpmAssessment = runtime.rpmAssessment;
                temp.correctTriageCategory = runtime.correctTriageCategory;
                temp.ehrActions = runtime.ehrActions;
                temp.clinicalExplanation = runtime.clinicalExplanation;
                temp.patientPrefab = runtime.patientPrefab;
                temp.priorityQuestion = runtime.priorityQuestion;
                temp.priorityOptions = runtime.priorityOptions;
                temp.correctPriorityIndex = runtime.correctPriorityIndex;
                temp.isMultiPatient = runtime.isMultiPatient;
                temp.orderIndex = runtime.orderIndex;
                
                AllMediumScenarios.Add(temp);
            }
            
            // Find a multi-patient scenario (with isMultiPatient=true and patients array length >= 2)
            var multiPatientScenario = mediumScenarios.FirstOrDefault(s => s.isMultiPatient && s.patients != null && s.patients.Length >= 2);
            
            if (multiPatientScenario != null)
            {
                Debug.Log($"Found multi-patient medium scenario: {multiPatientScenario.scenarioTitle} with {multiPatientScenario.patients.Length} patients");
                Debug.Log($"  - Priority Question: {multiPatientScenario.priorityQuestion}");
                Debug.Log($"  - Priority Options: {(multiPatientScenario.priorityOptions != null ? string.Join(", ", multiPatientScenario.priorityOptions) : "NULL")}");
                Debug.Log($"  - Correct Priority Index: {multiPatientScenario.correctPriorityIndex}");
                
                // Create Patient A from first patient
                var patientAData = multiPatientScenario.patients[0];
                var patientA = ScriptableObject.CreateInstance<ScenarioData>();
                patientA.scenarioTitle = multiPatientScenario.scenarioTitle + " - Patient A";
                patientA.scenarioID = multiPatientScenario.scenarioID + "_A";
                patientA.difficulty = multiPatientScenario.difficulty;
                patientA.timeLimitSeconds = multiPatientScenario.timeLimitSeconds;
                patientA.isMultiPatient = true;
                patientA.priorityQuestion = multiPatientScenario.priorityQuestion;
                patientA.priorityOptions = multiPatientScenario.priorityOptions;
                patientA.correctPriorityIndex = multiPatientScenario.correctPriorityIndex;
                patientA.orderIndex = multiPatientScenario.orderIndex;
                patientA.patientAge = patientAData.patientAge;
                patientA.patientGender = patientAData.patientGender;
                patientA.patientPresentation = patientAData.patientPresentation;
                patientA.vitalSigns = patientAData.vitalSigns;
                patientA.rpmAssessment = patientAData.rpmAssessment;
                patientA.correctTriageCategory = patientAData.correctTriageCategory;
                patientA.ehrActions = patientAData.ehrActions;
                patientA.clinicalExplanation = patientAData.clinicalExplanation;
                patientA.patientPrefab = patientAData.patientPrefab ?? multiPatientScenario.patientPrefab;
                
                // Create Patient B from second patient
                var patientBData = multiPatientScenario.patients[1];
                var patientB = ScriptableObject.CreateInstance<ScenarioData>();
                patientB.scenarioTitle = multiPatientScenario.scenarioTitle + " - Patient B";
                patientB.scenarioID = multiPatientScenario.scenarioID + "_B";
                patientB.difficulty = multiPatientScenario.difficulty;
                patientB.timeLimitSeconds = multiPatientScenario.timeLimitSeconds;
                patientB.isMultiPatient = true;
                patientB.priorityQuestion = multiPatientScenario.priorityQuestion;
                patientB.priorityOptions = multiPatientScenario.priorityOptions;
                patientB.correctPriorityIndex = multiPatientScenario.correctPriorityIndex;
                patientB.orderIndex = multiPatientScenario.orderIndex;
                patientB.patientAge = patientBData.patientAge;
                patientB.patientGender = patientBData.patientGender;
                patientB.patientPresentation = patientBData.patientPresentation;
                patientB.vitalSigns = patientBData.vitalSigns;
                patientB.rpmAssessment = patientBData.rpmAssessment;
                patientB.correctTriageCategory = patientBData.correctTriageCategory;
                patientB.ehrActions = patientBData.ehrActions;
                patientB.clinicalExplanation = patientBData.clinicalExplanation;
                patientB.patientPrefab = patientBData.patientPrefab ?? multiPatientScenario.patientPrefab;
                
                mediumPatientA = patientA;
                mediumPatientB = patientB;
                
                Debug.Log($"✅ Successfully created medium scenario pair from single Firestore document");
            }
            else
            {
                // Fallback: try to use two separate medium scenarios
                var sortedMedium = mediumScenarios.OrderBy(s => s.orderIndex).ToList();
                if (sortedMedium.Count >= 2)
                {
                    Debug.Log("Using two separate medium scenarios for patient A and B");
                    var runtimeA = sortedMedium[0];
                    var runtimeB = sortedMedium[1];
                    
                    var tempA = ScriptableObject.CreateInstance<ScenarioData>();
                    tempA.scenarioTitle = runtimeA.scenarioTitle;
                    tempA.scenarioID = runtimeA.scenarioID;
                    tempA.difficulty = runtimeA.difficulty;
                    tempA.timeLimitSeconds = runtimeA.timeLimitSeconds;
                    tempA.patientAge = runtimeA.patientAge;
                    tempA.patientGender = runtimeA.patientGender;
                    tempA.patientPresentation = runtimeA.patientPresentation;
                    tempA.vitalSigns = runtimeA.vitalSigns;
                    tempA.rpmAssessment = runtimeA.rpmAssessment;
                    tempA.correctTriageCategory = runtimeA.correctTriageCategory;
                    tempA.ehrActions = runtimeA.ehrActions;
                    tempA.clinicalExplanation = runtimeA.clinicalExplanation;
                    tempA.patientPrefab = runtimeA.patientPrefab;
                    tempA.isMultiPatient = true;
                    tempA.priorityQuestion = runtimeA.priorityQuestion;
                    tempA.priorityOptions = runtimeA.priorityOptions;
                    tempA.correctPriorityIndex = runtimeA.correctPriorityIndex;
                    tempA.orderIndex = runtimeA.orderIndex;
                    
                    var tempB = ScriptableObject.CreateInstance<ScenarioData>();
                    tempB.scenarioTitle = runtimeB.scenarioTitle;
                    tempB.scenarioID = runtimeB.scenarioID;
                    tempB.difficulty = runtimeB.difficulty;
                    tempB.timeLimitSeconds = runtimeB.timeLimitSeconds;
                    tempB.patientAge = runtimeB.patientAge;
                    tempB.patientGender = runtimeB.patientGender;
                    tempB.patientPresentation = runtimeB.patientPresentation;
                    tempB.vitalSigns = runtimeB.vitalSigns;
                    tempB.rpmAssessment = runtimeB.rpmAssessment;
                    tempB.correctTriageCategory = runtimeB.correctTriageCategory;
                    tempB.ehrActions = runtimeB.ehrActions;
                    tempB.clinicalExplanation = runtimeB.clinicalExplanation;
                    tempB.patientPrefab = runtimeB.patientPrefab;
                    tempB.isMultiPatient = true;
                    tempB.priorityQuestion = runtimeB.priorityQuestion;
                    tempB.priorityOptions = runtimeB.priorityOptions;
                    tempB.correctPriorityIndex = runtimeB.correctPriorityIndex;
                    tempB.orderIndex = runtimeB.orderIndex;
                    
                    mediumPatientA = tempA;
                    mediumPatientB = tempB;
                    
                    Debug.Log($"✅ Using two separate medium scenarios for patient A and B");
                }
                else
                {
                    Debug.LogWarning("No valid medium scenario configuration found. Using fallback.");
                    UseFallbackScenarios();
                }
            }
        }
        
        // Process Hard Scenarios - Handle single document with 3 patients
        if (hardScenarios != null && hardScenarios.Count > 0)
        {
            Debug.Log($"Processing {hardScenarios.Count} hard scenario(s) from Firestore");
            
            foreach (var runtime in hardScenarios)
            {
                var temp = ScriptableObject.CreateInstance<ScenarioData>();
                temp.scenarioTitle = runtime.scenarioTitle;
                temp.scenarioID = runtime.scenarioID;
                temp.difficulty = runtime.difficulty;
                temp.timeLimitSeconds = runtime.timeLimitSeconds;
                temp.patientAge = runtime.patientAge;
                temp.patientGender = runtime.patientGender;
                temp.patientPresentation = runtime.patientPresentation;
                temp.vitalSigns = runtime.vitalSigns;
                temp.rpmAssessment = runtime.rpmAssessment;
                temp.correctTriageCategory = runtime.correctTriageCategory;
                temp.ehrActions = runtime.ehrActions;
                temp.clinicalExplanation = runtime.clinicalExplanation;
                temp.patientPrefab = runtime.patientPrefab;
                temp.priorityQuestion = runtime.priorityQuestion;
                temp.priorityOptions = runtime.priorityOptions;
                temp.correctPriorityIndex = runtime.correctPriorityIndex;
                temp.isMultiPatient = runtime.isMultiPatient;
                temp.orderIndex = runtime.orderIndex;
                
                AllHardScenarios.Add(temp);
            }
            
            // Find a multi-patient hard scenario (with 3 patients)
            var hardMultiPatientScenario = hardScenarios.FirstOrDefault(s => s.isMultiPatient && s.patients != null && s.patients.Length >= 3);
            
            if (hardMultiPatientScenario != null)
            {
                Debug.Log($"Found multi-patient hard scenario: {hardMultiPatientScenario.scenarioTitle} with {hardMultiPatientScenario.patients.Length} patients");
                Debug.Log($"  - Priority Question: {hardMultiPatientScenario.priorityQuestion}");
                Debug.Log($"  - Priority Options: {(hardMultiPatientScenario.priorityOptions != null ? string.Join(", ", hardMultiPatientScenario.priorityOptions) : "NULL")}");
                Debug.Log($"  - Correct Priority Index: {hardMultiPatientScenario.correctPriorityIndex}");
                
                // Create Patient A
                var patientAData = hardMultiPatientScenario.patients[0];
                var patientA = ScriptableObject.CreateInstance<ScenarioData>();
                patientA.scenarioTitle = hardMultiPatientScenario.scenarioTitle + " - Patient A";
                patientA.scenarioID = hardMultiPatientScenario.scenarioID + "_A";
                patientA.difficulty = hardMultiPatientScenario.difficulty;
                patientA.timeLimitSeconds = hardMultiPatientScenario.timeLimitSeconds;
                patientA.isMultiPatient = true;
                patientA.priorityQuestion = hardMultiPatientScenario.priorityQuestion;
                patientA.priorityOptions = hardMultiPatientScenario.priorityOptions;
                patientA.correctPriorityIndex = hardMultiPatientScenario.correctPriorityIndex;
                patientA.orderIndex = hardMultiPatientScenario.orderIndex;
                patientA.patientAge = patientAData.patientAge;
                patientA.patientGender = patientAData.patientGender;
                patientA.patientPresentation = patientAData.patientPresentation;
                patientA.vitalSigns = patientAData.vitalSigns;
                patientA.rpmAssessment = patientAData.rpmAssessment;
                patientA.correctTriageCategory = patientAData.correctTriageCategory;
                patientA.ehrActions = patientAData.ehrActions;
                patientA.clinicalExplanation = patientAData.clinicalExplanation;
                patientA.patientPrefab = patientAData.patientPrefab ?? hardMultiPatientScenario.patientPrefab;
                
                // Create Patient B
                var patientBData = hardMultiPatientScenario.patients[1];
                var patientB = ScriptableObject.CreateInstance<ScenarioData>();
                patientB.scenarioTitle = hardMultiPatientScenario.scenarioTitle + " - Patient B";
                patientB.scenarioID = hardMultiPatientScenario.scenarioID + "_B";
                patientB.difficulty = hardMultiPatientScenario.difficulty;
                patientB.timeLimitSeconds = hardMultiPatientScenario.timeLimitSeconds;
                patientB.isMultiPatient = true;
                patientB.priorityQuestion = hardMultiPatientScenario.priorityQuestion;
                patientB.priorityOptions = hardMultiPatientScenario.priorityOptions;
                patientB.correctPriorityIndex = hardMultiPatientScenario.correctPriorityIndex;
                patientB.orderIndex = hardMultiPatientScenario.orderIndex;
                patientB.patientAge = patientBData.patientAge;
                patientB.patientGender = patientBData.patientGender;
                patientB.patientPresentation = patientBData.patientPresentation;
                patientB.vitalSigns = patientBData.vitalSigns;
                patientB.rpmAssessment = patientBData.rpmAssessment;
                patientB.correctTriageCategory = patientBData.correctTriageCategory;
                patientB.ehrActions = patientBData.ehrActions;
                patientB.clinicalExplanation = patientBData.clinicalExplanation;
                patientB.patientPrefab = patientBData.patientPrefab ?? hardMultiPatientScenario.patientPrefab;
                
                // Create Patient C
                var patientCData = hardMultiPatientScenario.patients[2];
                var patientC = ScriptableObject.CreateInstance<ScenarioData>();
                patientC.scenarioTitle = hardMultiPatientScenario.scenarioTitle + " - Patient C";
                patientC.scenarioID = hardMultiPatientScenario.scenarioID + "_C";
                patientC.difficulty = hardMultiPatientScenario.difficulty;
                patientC.timeLimitSeconds = hardMultiPatientScenario.timeLimitSeconds;
                patientC.isMultiPatient = true;
                patientC.priorityQuestion = hardMultiPatientScenario.priorityQuestion;
                patientC.priorityOptions = hardMultiPatientScenario.priorityOptions;
                patientC.correctPriorityIndex = hardMultiPatientScenario.correctPriorityIndex;
                patientC.orderIndex = hardMultiPatientScenario.orderIndex;
                patientC.patientAge = patientCData.patientAge;
                patientC.patientGender = patientCData.patientGender;
                patientC.patientPresentation = patientCData.patientPresentation;
                patientC.vitalSigns = patientCData.vitalSigns;
                patientC.rpmAssessment = patientCData.rpmAssessment;
                patientC.correctTriageCategory = patientCData.correctTriageCategory;
                patientC.ehrActions = patientCData.ehrActions;
                patientC.clinicalExplanation = patientCData.clinicalExplanation;
                patientC.patientPrefab = patientCData.patientPrefab ?? hardMultiPatientScenario.patientPrefab;
                
                hardPatientA = patientA;
                hardPatientB = patientB;
                hardPatientC = patientC;
                
                Debug.Log($"✅ Successfully created hard scenario triple from single Firestore document");
            }
            else if (hardScenarios.Count >= 3)
            {
                // Use three separate hard scenarios
                var sortedHard = hardScenarios.OrderBy(s => s.orderIndex).ToList();
                var runtimeA = sortedHard[0];
                var runtimeB = sortedHard[1];
                var runtimeC = sortedHard[2];
                
                hardPatientA = ScriptableObject.CreateInstance<ScenarioData>();
                hardPatientA.scenarioTitle = runtimeA.scenarioTitle;
                hardPatientA.scenarioID = runtimeA.scenarioID;
                hardPatientA.difficulty = runtimeA.difficulty;
                hardPatientA.timeLimitSeconds = runtimeA.timeLimitSeconds;
                hardPatientA.patientPrefab = runtimeA.patientPrefab;
                hardPatientA.isMultiPatient = true;
                hardPatientA.priorityQuestion = runtimeA.priorityQuestion;
                hardPatientA.priorityOptions = runtimeA.priorityOptions;
                hardPatientA.correctPriorityIndex = runtimeA.correctPriorityIndex;
                
                hardPatientB = ScriptableObject.CreateInstance<ScenarioData>();
                hardPatientB.scenarioTitle = runtimeB.scenarioTitle;
                hardPatientB.scenarioID = runtimeB.scenarioID;
                hardPatientB.difficulty = runtimeB.difficulty;
                hardPatientB.timeLimitSeconds = runtimeB.timeLimitSeconds;
                hardPatientB.patientPrefab = runtimeB.patientPrefab;
                hardPatientB.isMultiPatient = true;
                hardPatientB.priorityQuestion = runtimeB.priorityQuestion;
                hardPatientB.priorityOptions = runtimeB.priorityOptions;
                hardPatientB.correctPriorityIndex = runtimeB.correctPriorityIndex;
                
                hardPatientC = ScriptableObject.CreateInstance<ScenarioData>();
                hardPatientC.scenarioTitle = runtimeC.scenarioTitle;
                hardPatientC.scenarioID = runtimeC.scenarioID;
                hardPatientC.difficulty = runtimeC.difficulty;
                hardPatientC.timeLimitSeconds = runtimeC.timeLimitSeconds;
                hardPatientC.patientPrefab = runtimeC.patientPrefab;
                hardPatientC.isMultiPatient = true;
                hardPatientC.priorityQuestion = runtimeC.priorityQuestion;
                hardPatientC.priorityOptions = runtimeC.priorityOptions;
                hardPatientC.correctPriorityIndex = runtimeC.correctPriorityIndex;
                
                Debug.Log($"✅ Using three separate hard scenarios for patients A, B, C");
            }
            
            // Unlock the Hard card since we have hard scenarios
            hasHardScenarios = true;
        }
        
        // Update Hard card UI based on whether we have hard scenarios
        SetHardCardActive(hasHardScenarios);
        UpdateHardCardDescription(hasHardScenarios);
        
        Debug.Log($"Loaded - Easy: {AllEasyScenarios.Count}, Medium: {(mediumPatientA != null ? 2 : 0)}, Hard: {(hardPatientA != null ? 3 : 0)}");
    }

    void UseFallbackScenarios()
    {
        Debug.Log("Using fallback ScriptableObject scenarios");
        if (easyScenario != null)
        {
            AllEasyScenarios = new List<ScenarioData> { easyScenario };
        }
=======
        if (backButton != null)
            backButton.onClick.AddListener(() =>
                SceneManager.LoadScene("DashboardScene"));

        // Hard scenario locked
        if (hardCardLockOverlay != null)
            hardCardLockOverlay.SetActive(true);
>>>>>>> 26ca292180f2e5632fdb78b15fe5f649ef097e93
    }

    void StartEasy()
    {
<<<<<<< HEAD
        if (easyScenario != null)
        {
            Debug.Log($"=== Starting Easy Scenario ===");
            Debug.Log($"Scenario: {easyScenario.scenarioTitle}");
            Debug.Log($"Order Index: {easyScenario.orderIndex}");
            Debug.Log($"Patient Prefab: {(easyScenario.patientPrefab != null ? easyScenario.patientPrefab.name : "NULL")}");
            Debug.Log($"Difficulty: {easyScenario.difficulty}");
            
            ScenarioSelector.SelectEasy(easyScenario);
            CurrentEasyIndex = 0;
            SceneManager.LoadScene("MainAR");
        }
        else
        {
            Debug.LogError("No easy scenario available!");
        }
=======
        ScenarioSelector.SelectEasy(easyScenario);
        SceneManager.LoadScene("MainAR");
>>>>>>> 26ca292180f2e5632fdb78b15fe5f649ef097e93
    }

    void StartMedium()
    {
<<<<<<< HEAD
        if (mediumPatientA != null && mediumPatientB != null)
        {
            Debug.Log($"=== Starting Medium Scenario ===");
            Debug.Log($"Patient A: {mediumPatientA.scenarioTitle}");
            Debug.Log($"Patient B: {mediumPatientB.scenarioTitle}");
            Debug.Log($"Patient A Prefab: {(mediumPatientA.patientPrefab != null ? mediumPatientA.patientPrefab.name : "NULL")}");
            Debug.Log($"Patient B Prefab: {(mediumPatientB.patientPrefab != null ? mediumPatientB.patientPrefab.name : "NULL")}");
            Debug.Log($"Priority Question: {mediumPatientA.priorityQuestion}");
            Debug.Log($"Priority Options: {(mediumPatientA.priorityOptions != null ? string.Join(", ", mediumPatientA.priorityOptions) : "NULL")}");
            Debug.Log($"Correct Priority Index: {mediumPatientA.correctPriorityIndex}");
            
            ScenarioSelector.SelectMedium(mediumPatientA, mediumPatientB);
            SceneManager.LoadScene("MainAR");
        }
        else
        {
            Debug.LogError("No medium scenarios available!");
        }
    }
    
    void StartHard()
    {
        if (hardPatientA != null && hardPatientB != null && hardPatientC != null)
        {
            Debug.Log($"=== Starting Hard Scenario ===");
            Debug.Log($"Patient A: {hardPatientA.scenarioTitle}");
            Debug.Log($"Patient B: {hardPatientB.scenarioTitle}");
            Debug.Log($"Patient C: {hardPatientC.scenarioTitle}");
            Debug.Log($"Patient A Prefab: {(hardPatientA.patientPrefab != null ? hardPatientA.patientPrefab.name : "NULL")}");
            Debug.Log($"Patient B Prefab: {(hardPatientB.patientPrefab != null ? hardPatientB.patientPrefab.name : "NULL")}");
            Debug.Log($"Patient C Prefab: {(hardPatientC.patientPrefab != null ? hardPatientC.patientPrefab.name : "NULL")}");
            Debug.Log($"Priority Question: {hardPatientA.priorityQuestion}");
            Debug.Log($"Priority Options: {(hardPatientA.priorityOptions != null ? string.Join(", ", hardPatientA.priorityOptions) : "NULL")}");
            Debug.Log($"Correct Priority Index: {hardPatientA.correctPriorityIndex}");
            
            ScenarioSelector.SelectHard(hardPatientA, hardPatientB, hardPatientC);
            SceneManager.LoadScene("MainAR");
        }
        else
        {
            Debug.LogError("No hard scenarios available!");
        }
=======
        ScenarioSelector.SelectMedium(
            mediumPatientA, mediumPatientB);
        SceneManager.LoadScene("MainAR");
>>>>>>> 26ca292180f2e5632fdb78b15fe5f649ef097e93
    }
}