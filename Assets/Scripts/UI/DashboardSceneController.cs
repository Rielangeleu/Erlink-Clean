using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using Firebase.Firestore;
using Firebase.Extensions;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Controls the Student Dashboard Scene.
/// Loads user performance data from Firebase.
/// Routes to correct screens based on user actions.
/// </summary>
public class DashboardSceneController : MonoBehaviour
{
    [Header("Header")]
    public TextMeshProUGUI welcomeText;

    [Header("Action Cards")]
    public Button startSimulationCard;
    public Button viewResultsCard;

    [Header("Performance Stats")]
    public TextMeshProUGUI accuracyValue;
    public TextMeshProUGUI speedValue;
    public TextMeshProUGUI confidenceValue;
    public TextMeshProUGUI accuracySub;
    public TextMeshProUGUI speedSub;

    [Header("Recent Activity")]
    public Transform activityList;
    public GameObject activityItemPrefab;
    public GameObject noActivityPlaceholder;

    [Header("Navigation")]
    public Button homeNav;
    public Button simulateNav;
    public Button resultsNav;
    public Button profileNav;

    [Header("Logout")]
    public Button logoutButton;

    void Start()
    {
        SetupWelcome();
        SetupNavigation();
        StartCoroutine(WaitForFirebaseAndLoad());
    }

    System.Collections.IEnumerator WaitForFirebaseAndLoad()
    {
        // Wait for Firebase to be ready
        while (FirebaseManager.Instance == null || FirebaseManager.CurrentUser == null)
        {
            yield return new WaitForSeconds(0.5f);
        }

        Debug.Log("Firebase ready, loading dashboard data...");
        LoadPerformanceData();
    }

    void SetupWelcome()
    {
        if (welcomeText != null)
        {
            string name = FirebaseManager.CurrentProfile?.displayName ?? "Student";
            welcomeText.text = $"Welcome, {name}";
        }

        if (startSimulationCard != null)
            startSimulationCard.onClick.AddListener(() => SceneManager.LoadScene("ScenarioSelectScene"));

        if (viewResultsCard != null)
            viewResultsCard.onClick.AddListener(() => SceneManager.LoadScene("ResultsScene"));
    }

    void SetupNavigation()
    {
        if (simulateNav != null)
            simulateNav.onClick.AddListener(() => SceneManager.LoadScene("ScenarioSelectScene"));

        if (resultsNav != null)
            resultsNav.onClick.AddListener(() => SceneManager.LoadScene("ResultsScene"));

        if (profileNav != null)
            profileNav.onClick.AddListener(() => SceneManager.LoadScene("ProfileScene"));

        if (logoutButton != null)
            logoutButton.onClick.AddListener(OnLogout);
    }

    async void LoadPerformanceData()
    {
        if (FirebaseManager.Instance == null || FirebaseManager.CurrentUser == null)
        {
            Debug.LogError("FirebaseManager or CurrentUser is null");
            ShowEmptyStats();
            return;
        }

        string userId = FirebaseManager.CurrentUser.UserId;
        Debug.Log($"Loading data for userId: {userId}");

        try
        {
            // Load ALL sessions for stats
            QuerySnapshot allSessionsSnap = await FirebaseFirestore
                .DefaultInstance
                .Collection("sessions")
                .WhereEqualTo("userId", userId)
                .GetSnapshotAsync();

            Debug.Log($"Found {allSessionsSnap.Count} total sessions");

            if (allSessionsSnap.Count == 0)
            {
                ShowEmptyStats();
                return;
            }

            // Calculate averages from ALL sessions
            List<int> accuracies = new List<int>();
            List<float> speeds = new List<float>();
            List<int> confidences = new List<int>();

            foreach (DocumentSnapshot doc in allSessionsSnap.Documents)
            {
                if (doc.ContainsField("accuracyScore"))
                    accuracies.Add(doc.GetValue<int>("accuracyScore"));

                if (doc.ContainsField("timeTaken"))
                    speeds.Add(doc.GetValue<float>("timeTaken"));

                if (doc.ContainsField("confidenceScore"))
                    confidences.Add(doc.GetValue<int>("confidenceScore"));
            }

            // Update Accuracy
            if (accuracies.Count > 0 && accuracyValue != null)
            {
                float avg = (float)accuracies.Average();
                accuracyValue.text = $"{avg:F0}%";
                accuracyValue.color = GetScoreColor((int)avg);

                if (accuracySub != null)
                    accuracySub.text = $"Based on {accuracies.Count} simulations";
            }

            // Update Speed
            if (speeds.Count > 0 && speedValue != null)
            {
                float avgSpeed = (float)speeds.Average();
                speedValue.text = avgSpeed < 60
                    ? $"{avgSpeed:F0}s"
                    : $"{avgSpeed / 60:F0}m {avgSpeed % 60:F0}s";

                if (speedSub != null)
                    speedSub.text = $"Avg response time";
            }

            // Update Confidence
            if (confidences.Count > 0 && confidenceValue != null)
            {
                float avgConf = (float)confidences.Average();
                confidenceValue.text = GetConfidenceLabel((int)avgConf);
            }

            // Load LAST 5 SESSIONS for Recent Activity
            QuerySnapshot recentSessionsSnap = await FirebaseFirestore
                .DefaultInstance
                .Collection("sessions")
                .WhereEqualTo("userId", userId)
                .OrderByDescending("completedAt")
                .Limit(5)
                .GetSnapshotAsync();

            Debug.Log($"Found {recentSessionsSnap.Count} recent sessions");

            // Populate recent activity
            PopulateRecentActivity(recentSessionsSnap.Documents.ToList());
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Dashboard load failed: {e.Message}");
            ShowEmptyStats();
        }
    }

    void PopulateRecentActivity(List<DocumentSnapshot> docs)
    {
        if (activityList == null)
        {
            Debug.LogError("activityList is not assigned!");
            return;
        }

        // Clear existing items first
        foreach (Transform child in activityList)
        {
            Destroy(child.gameObject);
        }

        // Show/hide placeholder
        if (noActivityPlaceholder != null)
            noActivityPlaceholder.SetActive(docs.Count == 0);

        if (docs.Count == 0)
        {
            Debug.Log("No recent sessions to display");
            return;
        }

        Debug.Log($"Creating {docs.Count} recent activity items");

        // Create activity items
        foreach (DocumentSnapshot doc in docs)
        {
            CreateActivityItem(doc);
        }
    }

    void CreateActivityItem(DocumentSnapshot doc)
    {
        // Get scenario title
        string scenarioTitle = doc.ContainsField("scenarioTitle")
            ? doc.GetValue<string>("scenarioTitle") : "Simulation";

        // Get score
        int score = doc.ContainsField("finalScore")
            ? doc.GetValue<int>("finalScore") : 0;

        // Get correctness
        bool isCorrect = doc.ContainsField("isCorrect")
            ? doc.GetValue<bool>("isCorrect") : false;

        // Get date
        string dateStr = "Just now";
        if (doc.ContainsField("completedAt"))
        {
            Timestamp timestamp = doc.GetValue<Timestamp>("completedAt");
            System.DateTime date = timestamp.ToDateTime();
            dateStr = date.ToString("MMM dd, yyyy");
        }

        Debug.Log($"Creating activity item: {scenarioTitle} - {score}% - {dateStr}");

        if (activityItemPrefab != null)
        {
            GameObject item = Instantiate(activityItemPrefab, activityList);
            TextMeshProUGUI[] texts = item.GetComponentsInChildren<TextMeshProUGUI>();

            if (texts.Length >= 2)
            {
                texts[0].text = scenarioTitle;
                texts[1].text = $"Score: {score}%";
                texts[1].color = GetScoreColor(score);

                if (texts.Length >= 3)
                    texts[2].text = dateStr;
                if (texts.Length >= 4)
                    texts[3].text = isCorrect ? "✓ Correct" : "✗ Incorrect";
            }
        }
        else
        {
            // Create item dynamically if no prefab
            CreateActivityItemDynamic(scenarioTitle, score, dateStr, isCorrect);
        }
    }

    void CreateActivityItemDynamic(string scenarioTitle, int score, string dateStr, bool isCorrect)
    {
        GameObject item = new GameObject("ActivityItem");
        item.transform.SetParent(activityList, false);

        RectTransform rt = item.AddComponent<RectTransform>();
        rt.sizeDelta = new Vector2(0, 80);

        Image bg = item.AddComponent<Image>();
        bg.color = new Color(0.973f, 0.980f, 0.988f);

        LayoutElement le = item.AddComponent<LayoutElement>();
        le.minHeight = 80;

        // Title
        GameObject titleGO = new GameObject("Title");
        titleGO.transform.SetParent(item.transform, false);
        TextMeshProUGUI title = titleGO.AddComponent<TextMeshProUGUI>();
        title.text = $"{scenarioTitle} — {score}%";
        title.fontSize = 24;
        title.color = GetScoreColor(score);
        title.fontStyle = FontStyles.Bold;

        RectTransform titleRt = titleGO.GetComponent<RectTransform>();
        titleRt.anchorMin = new Vector2(0, 0.5f);
        titleRt.anchorMax = new Vector2(1, 0.5f);
        titleRt.offsetMin = new Vector2(16, 0);
        titleRt.offsetMax = new Vector2(-16, 0);
        titleRt.sizeDelta = new Vector2(0, 30);

        // Date
        if (!string.IsNullOrEmpty(dateStr))
        {
            GameObject dateGO = new GameObject("Date");
            dateGO.transform.SetParent(item.transform, false);
            TextMeshProUGUI dateText = dateGO.AddComponent<TextMeshProUGUI>();
            dateText.text = dateStr;
            dateText.fontSize = 14;
            dateText.color = new Color(0.6f, 0.6f, 0.6f);

            RectTransform dateRt = dateGO.GetComponent<RectTransform>();
            dateRt.anchorMin = new Vector2(0, 0);
            dateRt.anchorMax = new Vector2(1, 0);
            dateRt.offsetMin = new Vector2(16, 10);
            dateRt.offsetMax = new Vector2(-16, 30);
        }
    }

    void ShowEmptyStats()
    {
        if (accuracyValue != null) accuracyValue.text = "--";
        if (speedValue != null) speedValue.text = "--";
        if (confidenceValue != null) confidenceValue.text = "--";
        if (accuracySub != null) accuracySub.text = "No simulations yet";
        if (speedSub != null) speedSub.text = "Complete a simulation";

        if (noActivityPlaceholder != null)
            noActivityPlaceholder.SetActive(true);
    }

    void OnLogout()
    {
        FirebaseManager.Instance?.Logout();
        SceneManager.LoadScene("LoginScene");
    }

    Color GetScoreColor(int score)
    {
        if (score >= 90) return new Color(0.086f, 0.635f, 0.290f);
        if (score >= 80) return new Color(0.145f, 0.337f, 0.922f);
        if (score >= 70) return new Color(0.851f, 0.467f, 0.024f);
        return new Color(0.863f, 0.149f, 0.149f);
    }

    string GetConfidenceLabel(int score)
    {
        return score switch
        {
            5 => "Excellent",
            4 => "Good",
            3 => "Average",
            2 => "Low",
            _ => "Very Low"
        };
    }
}