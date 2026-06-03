using UnityEngine;
using Firebase;
using Firebase.Firestore;
using Firebase.Extensions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

/// <summary>
/// Dynamically loads scenarios from Firestore and converts to RuntimeScenarioData
/// Uses pre-loaded prefabs from Resources folder - NO DOWNLOADING!
/// </summary>
public class FirestoreScenarioLoader : MonoBehaviour
{
    public static FirestoreScenarioLoader Instance;

    [Header("Fallback ScriptableObjects")]
    public ScenarioData[] fallbackEasyScenarios;
    public ScenarioData[] fallbackMediumScenarios;
    public ScenarioData[] fallbackHardScenarios;

    [Header("Settings")]
    public bool preferFirestore = true;
    public bool cacheScenariosLocally = true;

    private Dictionary<DifficultyLevel, List<RuntimeScenarioData>> _cachedScenarios = new Dictionary<DifficultyLevel, List<RuntimeScenarioData>>();
    private bool _hasLoadedFromFirestore = false;
    private bool _isFirebaseReady = false;

    public static event Action<DifficultyLevel, List<RuntimeScenarioData>> OnScenariosLoaded;
    public static event Action<string> OnLoadError;

    void Awake()
    {
        if (!Application.isPlaying)
        {
            Debug.Log("FirestoreScenarioLoader: Skipping initialization in Editor edit mode");
            return;
        }
        
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        foreach (DifficultyLevel level in Enum.GetValues(typeof(DifficultyLevel)))
        {
            _cachedScenarios[level] = new List<RuntimeScenarioData>();
        }

        FirebaseManager.OnFirebaseReady += OnFirebaseReady;
    }

    void OnFirebaseReady()
    {
        _isFirebaseReady = true;
        Debug.Log("FirestoreScenarioLoader: Firebase is ready!");
    }

    void Start()
    {
        if (!Application.isPlaying) return;
        
        if (FirebaseManager.IsInitialized)
        {
            _isFirebaseReady = true;
            Debug.Log("FirestoreScenarioLoader: Firebase already initialized");
        }
    }

    public void DebugPrintCache()
    {
        Debug.Log("=== FIRESTORE CACHE DEBUG ===");
        foreach (var kvp in _cachedScenarios)
        {
            Debug.Log($"{kvp.Key}: {kvp.Value.Count} scenarios");
            foreach (var s in kvp.Value)
            {
                Debug.Log($"  - {s.scenarioTitle} (Model: {s.patientModelName}, Prefab: {(s.patientPrefab != null ? s.patientPrefab.name : "NULL")})");
                if (s.patients != null)
                {
                    Debug.Log($"      Patients: {s.patients.Length}");
                    for (int i = 0; i < s.patients.Length; i++)
                    {
                        Debug.Log($"        Patient {i + 1}: {s.patients[i].patientModelName}");
                    }
                }
            }
        }
    }

    public async Task<List<RuntimeScenarioData>> GetScenarios(DifficultyLevel difficulty, bool forceRefresh = false)
    {
        Debug.Log($"GetScenarios called for {difficulty}, forceRefresh={forceRefresh}");
        
        if (!_cachedScenarios.ContainsKey(difficulty))
        {
            _cachedScenarios[difficulty] = new List<RuntimeScenarioData>();
        }

        if (!forceRefresh && _cachedScenarios[difficulty].Count > 0 && _hasLoadedFromFirestore)
        {
            Debug.Log($"Returning {_cachedScenarios[difficulty].Count} cached scenarios for {difficulty}");
            return _cachedScenarios[difficulty];
        }

        if (preferFirestore && _isFirebaseReady && FirebaseManager.IsInitialized)
        {
            Debug.Log($"Loading {difficulty} scenarios from Firestore...");
            var firestoreScenarios = await LoadScenariosFromFirestore(difficulty);
            if (firestoreScenarios != null && firestoreScenarios.Count > 0)
            {
                _cachedScenarios[difficulty] = firestoreScenarios;
                _hasLoadedFromFirestore = true;
                OnScenariosLoaded?.Invoke(difficulty, firestoreScenarios);
                Debug.Log($"Loaded {firestoreScenarios.Count} scenarios from Firestore for {difficulty}");
                return firestoreScenarios;
            }
            else
            {
                Debug.Log($"No scenarios found in Firestore for {difficulty}, using fallback");
            }
        }

        Debug.Log($"Using fallback ScriptableObjects for {difficulty}");
        var fallbackScenarios = GetFallbackScenarios(difficulty);
        _cachedScenarios[difficulty] = fallbackScenarios;
        return fallbackScenarios;
    }

    public List<RuntimeScenarioData> GetCachedScenarios(DifficultyLevel difficulty)
    {
        Debug.Log($"GetCachedScenarios called for {difficulty}");
        
        if (_cachedScenarios == null)
        {
            Debug.LogError("_cachedScenarios is null!");
            return new List<RuntimeScenarioData>();
        }
        
        if (!_cachedScenarios.ContainsKey(difficulty))
        {
            Debug.LogWarning($"No cache for difficulty {difficulty}");
            return new List<RuntimeScenarioData>();
        }
        
        Debug.Log($"Returning {_cachedScenarios[difficulty].Count} cached scenarios for {difficulty}");
        return _cachedScenarios[difficulty];
    }

    private async Task<List<RuntimeScenarioData>> LoadScenariosFromFirestore(DifficultyLevel difficulty)
    {
        try
        {
            Debug.Log($"=== FIRESTORE LOAD STARTED for {difficulty} ===");
            
            var db = FirebaseFirestore.DefaultInstance;
            string difficultyString = difficulty.ToString();

            Debug.Log($"Query: scenarios where difficulty='{difficultyString}' and isActive=true");

            Query query = db.Collection("scenarios")
                .WhereEqualTo("difficulty", difficultyString)
                .WhereEqualTo("isActive", true);

            QuerySnapshot snapshot = await query.GetSnapshotAsync();

            Debug.Log($"Firestore returned {snapshot.Count} documents for {difficulty}");

            if (snapshot.Count == 0)
            {
                Debug.LogWarning($"⚠️ No active scenarios found in Firestore for {difficulty}");
                return null;
            }

            List<RuntimeScenarioData> scenarios = new List<RuntimeScenarioData>();

            foreach (DocumentSnapshot doc in snapshot.Documents)
            {
                Debug.Log($"--- Processing document: {doc.Id} ---");
                
                var scenario = ConvertFirestoreDocumentToScenario(doc, difficulty);
                if (scenario != null)
                {
                    scenarios.Add(scenario);
                    Debug.Log($"✅ Added scenario: '{scenario.scenarioTitle}'");
                }
                else
                {
                    Debug.LogError($"❌ Failed to convert document {doc.Id} to scenario");
                }
            }

            scenarios = scenarios.OrderBy(s => s.orderIndex).ToList();
            
            Debug.Log($"=== FIRESTORE LOAD COMPLETE: {scenarios.Count} scenarios loaded for {difficulty} ===");
            
            return scenarios;
        }
        catch (Exception e)
        {
            Debug.LogError($"❌ FIRESTORE LOAD FAILED: {e.Message}");
            Debug.LogError($"Stack trace: {e.StackTrace}");
            OnLoadError?.Invoke($"Failed to load scenarios: {e.Message}");
            return null;
        }
    }

    private RuntimeScenarioData ConvertFirestoreDocumentToScenario(DocumentSnapshot doc, DifficultyLevel difficulty)
    {
        try
        {
            // Initialize scenario with basic fields
            var scenario = new RuntimeScenarioData
            {
                scenarioID = doc.ContainsField("scenarioID") ? doc.GetValue<string>("scenarioID") : "",
                scenarioTitle = doc.ContainsField("scenarioTitle") ? doc.GetValue<string>("scenarioTitle") : "Untitled",
                scenarioDescription = doc.ContainsField("scenarioDescription") ? doc.GetValue<string>("scenarioDescription") : "",
                difficulty = difficulty,
                emergencyCode = ParseEmergencyCode(doc.ContainsField("emergencyCode") ? doc.GetValue<string>("emergencyCode") : "None"),
                timeLimitSeconds = doc.ContainsField("timeLimitSeconds") ? doc.GetValue<float>("timeLimitSeconds") : 180f,
                isMultiPatient = doc.ContainsField("isMultiPatient") && doc.GetValue<bool>("isMultiPatient"),
                correctPriorityOrder = doc.ContainsField("correctPriorityOrder") ? doc.GetValue<int>("correctPriorityOrder") : 0,
                orderIndex = doc.ContainsField("orderIndex") ? doc.GetValue<int>("orderIndex") : 999,
                clinicalExplanation = "",
                priorityQuestion = doc.ContainsField("priorityQuestion") ? doc.GetValue<string>("priorityQuestion") : "Which patient requires immediate attention first?",
                correctPriorityIndex = doc.ContainsField("correctPriorityIndex") ? doc.GetValue<int>("correctPriorityIndex") : 0
            };

            // Read priority options array from scenario level
            if (doc.ContainsField("priorityOptions") && doc.GetValue<List<object>>("priorityOptions") is List<object> priorityOptionsList)
            {
                scenario.priorityOptions = priorityOptionsList.Select(p => p.ToString()).ToArray();
                Debug.Log($"Loaded {scenario.priorityOptions.Length} priority options for {scenario.scenarioTitle}");
            }
            else if (scenario.isMultiPatient)
            {
                if (doc.TryGetValue<List<object>>("patients", out var defaultPatientsList) && defaultPatientsList != null)
                {
                    int patientCount = defaultPatientsList.Count;
                    if (patientCount >= 3)
                    {
                        scenario.priorityOptions = new string[] 
                        { 
                            "Patient A should be treated FIRST",
                            "Patient B should be treated FIRST",
                            "Patient C should be treated FIRST"
                        };
                    }
                    else if (patientCount == 2)
                    {
                        scenario.priorityOptions = new string[] 
                        { 
                            "Patient A should be treated FIRST",
                            "Patient B should be treated FIRST"
                        };
                    }
                }
                else
                {
                    scenario.priorityOptions = new string[] 
                    { 
                        "Patient A should be treated FIRST",
                        "Patient B should be treated FIRST"
                    };
                }
                Debug.Log($"Using default priority options for {scenario.scenarioTitle}");
            }

            // ─── PARSE PATIENTS ARRAY - STORE ALL PATIENTS ───
            List<RuntimePatientData> allPatients = new List<RuntimePatientData>();
            
            if (doc.TryGetValue<List<object>>("patients", out var patientsList) && patientsList != null && patientsList.Count > 0)
            {
                Debug.Log($"Found {patientsList.Count} patients in scenario");
                
                foreach (var patientObj in patientsList)
                {
                    var patientData = patientObj as Dictionary<string, object>;
                    if (patientData != null)
                    {
                        var runtimePatient = new RuntimePatientData();
                        
                        // Basic patient info
                        runtimePatient.patientAge = patientData.ContainsKey("patientAge") ? patientData["patientAge"].ToString() : "~40s";
                        runtimePatient.patientGender = patientData.ContainsKey("patientGender") ? patientData["patientGender"].ToString() : "Male";
                        runtimePatient.patientPresentation = patientData.ContainsKey("patientPresentation") ? patientData["patientPresentation"].ToString() : "";
                        runtimePatient.correctTriageCategory = ParseTriageCategory(patientData.ContainsKey("correctTriageCategory") ? patientData["correctTriageCategory"].ToString() : "Immediate");
                        
                        // Get model name
                        string modelName = patientData.ContainsKey("patientModelName") ? patientData["patientModelName"].ToString() : "";
                        if (string.IsNullOrEmpty(modelName))
                        {
                            modelName = GetDefaultModelName(scenario.scenarioTitle);
                        }
                        runtimePatient.patientModelName = modelName;
                        
                        // Load prefab
                        runtimePatient.patientPrefab = LoadPatientPrefab(modelName);
                        
                        // Vital signs
                        if (patientData.ContainsKey("vitalSigns") && patientData["vitalSigns"] is List<object> vitalList)
                        {
                            runtimePatient.vitalSigns = vitalList.Select(v => v.ToString()).ToArray();
                        }
                        else
                        {
                            runtimePatient.vitalSigns = new string[] { "Assess", "Assess", "Assess" };
                        }
                        
                        // RPM Assessment
                        runtimePatient.rpmAssessment = ParseRPMAssessment(patientData);
                        
                        // EHR Actions
                        runtimePatient.ehrActions = ParseEHRActions(patientData);
                        
                        // Clinical explanation
                        runtimePatient.clinicalExplanation = patientData.ContainsKey("clinicalExplanation") ? patientData["clinicalExplanation"].ToString() : "";
                        
                        allPatients.Add(runtimePatient);
                        Debug.Log($"  - Added patient: {runtimePatient.patientModelName}");
                    }
                }
            }
            
            // Store all patients in scenario
            scenario.patients = allPatients.ToArray();
            
            // Also set first patient's data for backward compatibility (single patient scenarios)
            if (allPatients.Count > 0)
            {
                var firstPatient = allPatients[0];
                scenario.patientAge = firstPatient.patientAge;
                scenario.patientGender = firstPatient.patientGender;
                scenario.patientPresentation = firstPatient.patientPresentation;
                scenario.vitalSigns = firstPatient.vitalSigns;
                scenario.rpmAssessment = firstPatient.rpmAssessment;
                scenario.correctTriageCategory = firstPatient.correctTriageCategory;
                scenario.ehrActions = firstPatient.ehrActions;
                scenario.clinicalExplanation = firstPatient.clinicalExplanation;
                scenario.patientModelName = firstPatient.patientModelName;
                scenario.patientPrefab = firstPatient.patientPrefab;
            }
            else
            {
                // Fallback: create a single patient from scenario data
                Debug.LogWarning("No patients array found, creating single patient from scenario data");
                var singlePatient = new RuntimePatientData
                {
                    patientAge = scenario.patientAge,
                    patientGender = scenario.patientGender,
                    patientPresentation = scenario.patientPresentation,
                    vitalSigns = scenario.vitalSigns,
                    patientModelName = scenario.patientModelName,
                    correctTriageCategory = scenario.correctTriageCategory,
                    rpmAssessment = scenario.rpmAssessment,
                    ehrActions = scenario.ehrActions,
                    clinicalExplanation = scenario.clinicalExplanation,
                    patientPrefab = scenario.patientPrefab
                };
                scenario.patients = new RuntimePatientData[] { singlePatient };
            }

            return scenario;
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to convert Firestore document: {e.Message}");
            return null;
        }
    }

    private GameObject LoadPatientPrefab(string modelName)
    {
        GameObject prefab = null;
        
        string[] paths = {
            $"Patients/{modelName}",
            $"Patients/Patient_{modelName}",
            $"{modelName}",
            $"Patient_{modelName}"
        };
        
        foreach (string path in paths)
        {
            prefab = Resources.Load<GameObject>(path);
            if (prefab != null)
            {
                Debug.Log($"Found prefab at path: {path}");
                break;
            }
        }
        
        if (prefab == null)
        {
            var allPatients = Resources.LoadAll<GameObject>("Patients");
            if (allPatients.Length > 0)
            {
                prefab = allPatients[0];
                Debug.Log($"Using fallback prefab: {prefab.name}");
            }
        }
        
        return prefab;
    }

    private GameObject CreateFallbackPatientPrefab()
    {
        GameObject fallback = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        fallback.name = "Fallback_Patient";
        fallback.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        
        var renderer = fallback.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material.color = new Color(0.8f, 0.6f, 0.4f);
        }
        
        return fallback;
    }

    private string GetDefaultModelName(string scenarioTitle)
    {
        if (string.IsNullOrEmpty(scenarioTitle)) return "Patient_Standard";
        
        string lowerTitle = scenarioTitle.ToLower();
        
        if (lowerTitle.Contains("hemorrhage") || lowerTitle.Contains("bleeding"))
            return "Patient_Hemorrhage";
        if (lowerTitle.Contains("fracture") || lowerTitle.Contains("broken"))
            return "Patient_Fracture";
        if (lowerTitle.Contains("pregnant") || lowerTitle.Contains("maternal"))
            return "Patient_Pregnant";
        if (lowerTitle.Contains("cardiac") || lowerTitle.Contains("heart") || lowerTitle.Contains("code blue"))
            return "Patient_Cardiac";
        if (lowerTitle.Contains("burn"))
            return "Patient_Burn";
        if (lowerTitle.Contains("pediatric") || lowerTitle.Contains("child"))
            return "Patient_Pediatric";
        
        return "Patient_Standard";
    }

    private RPMAssessment ParseRPMAssessment(Dictionary<string, object> patientData)
    {
        var rpm = new RPMAssessment();

        if (patientData.ContainsKey("rpmAssessment") && patientData["rpmAssessment"] is Dictionary<string, object> rpmData)
        {
            rpm.respirationQuestion = rpmData.ContainsKey("respirationQuestion") ? rpmData["respirationQuestion"].ToString() : "Assess Respiration:";
            rpm.respirationOptions = ParseStringArray(rpmData, "respirationOptions");
            rpm.correctRespirationIndex = rpmData.ContainsKey("correctRespirationIndex") ? Convert.ToInt32(rpmData["correctRespirationIndex"]) : 0;
            rpm.respirationFeedback = rpmData.ContainsKey("respirationFeedback") ? rpmData["respirationFeedback"].ToString() : "";

            rpm.perfusionQuestion = rpmData.ContainsKey("perfusionQuestion") ? rpmData["perfusionQuestion"].ToString() : "Assess Perfusion:";
            rpm.perfusionOptions = ParseStringArray(rpmData, "perfusionOptions");
            rpm.correctPerfusionIndex = rpmData.ContainsKey("correctPerfusionIndex") ? Convert.ToInt32(rpmData["correctPerfusionIndex"]) : 0;
            rpm.perfusionFeedback = rpmData.ContainsKey("perfusionFeedback") ? rpmData["perfusionFeedback"].ToString() : "";

            rpm.mentalStatusQuestion = rpmData.ContainsKey("mentalStatusQuestion") ? rpmData["mentalStatusQuestion"].ToString() : "Assess Mental Status:";
            rpm.mentalStatusOptions = ParseStringArray(rpmData, "mentalStatusOptions");
            rpm.correctMentalStatusIndex = rpmData.ContainsKey("correctMentalStatusIndex") ? Convert.ToInt32(rpmData["correctMentalStatusIndex"]) : 0;
            rpm.mentalStatusFeedback = rpmData.ContainsKey("mentalStatusFeedback") ? rpmData["mentalStatusFeedback"].ToString() : "";
        }
        else
        {
            rpm.respirationQuestion = "What is the patient's breathing rate?";
            rpm.respirationOptions = new string[] { "Normal (<30/min)", "Rapid (>30/min)", "Absent/Agonal" };
            rpm.perfusionQuestion = "What is the patient's perfusion status?";
            rpm.perfusionOptions = new string[] { "Normal capillary refill (<2s)", "Delayed capillary refill (>2s)", "No palpable pulse" };
            rpm.mentalStatusQuestion = "What is the patient's mental status?";
            rpm.mentalStatusOptions = new string[] { "Alert and oriented", "Confused/disoriented", "Unresponsive" };
        }

        return rpm;
    }

    private EHRAction[] ParseEHRActions(Dictionary<string, object> patientData)
    {
        var actions = new EHRAction[3];

        if (patientData.ContainsKey("ehrActions") && patientData["ehrActions"] is List<object> actionsList)
        {
            for (int i = 0; i < Math.Min(actionsList.Count, 3); i++)
            {
                var actionData = actionsList[i] as Dictionary<string, object>;
                if (actionData != null)
                {
                    actions[i] = new EHRAction
                    {
                        actionName = actionData.ContainsKey("actionName") ? actionData["actionName"].ToString() : "",
                        actionDescription = actionData.ContainsKey("actionDescription") ? actionData["actionDescription"].ToString() : "",
                        isCorrectAction = actionData.ContainsKey("isCorrectAction") && Convert.ToBoolean(actionData["isCorrectAction"])
                    };
                }
                else
                {
                    actions[i] = new EHRAction { actionName = "", actionDescription = "", isCorrectAction = false };
                }
            }
        }
        else
        {
            actions[0] = new EHRAction { actionName = "Administer oxygen", actionDescription = "Apply oxygen via non-rebreather mask", isCorrectAction = true };
            actions[1] = new EHRAction { actionName = "Apply pressure", actionDescription = "Apply direct pressure to wound", isCorrectAction = false };
            actions[2] = new EHRAction { actionName = "Position patient", actionDescription = "Lay patient flat with legs elevated", isCorrectAction = false };
        }

        return actions;
    }

    private string[] ParseStringArray(Dictionary<string, object> data, string key)
    {
        if (data.ContainsKey(key) && data[key] is List<object> list)
        {
            return list.Select(x => x.ToString()).ToArray();
        }
        return new string[] { "Option 1", "Option 2", "Option 3" };
    }

    private List<RuntimeScenarioData> GetFallbackScenarios(DifficultyLevel difficulty)
    {
        List<RuntimeScenarioData> fallbacks = new List<RuntimeScenarioData>();
        ScenarioData[] source = null;

        switch (difficulty)
        {
            case DifficultyLevel.Easy:
                source = fallbackEasyScenarios;
                break;
            case DifficultyLevel.Medium:
                source = fallbackMediumScenarios;
                break;
            case DifficultyLevel.Hard:
                source = fallbackHardScenarios;
                break;
        }

        if (source != null)
        {
            foreach (var scriptable in source)
            {
                if (scriptable != null)
                {
                    var runtime = scriptable.ToRuntime();
                    if (runtime != null)
                    {
                        fallbacks.Add(runtime);
                    }
                }
            }
        }

        return fallbacks;
    }

    private TriageCategory ParseTriageCategory(string value)
    {
        switch (value?.ToLower())
        {
            case "immediate": return TriageCategory.Immediate;
            case "delayed": return TriageCategory.Delayed;
            case "minor": return TriageCategory.Minor;
            case "expectant": return TriageCategory.Expectant;
            default: return TriageCategory.Immediate;
        }
    }

    private EmergencyCode ParseEmergencyCode(string value)
    {
        switch (value?.ToLower())
        {
            case "codeblue": return EmergencyCode.CodeBlue;
            case "codered": return EmergencyCode.CodeRed;
            case "codeorange": return EmergencyCode.CodeOrange;
            case "codesilver": return EmergencyCode.CodeSilver;
            default: return EmergencyCode.None;
        }
    }

    void OnDestroy()
    {
        FirebaseManager.OnFirebaseReady -= OnFirebaseReady;
    }
}