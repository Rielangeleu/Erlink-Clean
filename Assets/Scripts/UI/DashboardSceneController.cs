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
                if (avgSpeed < 60)
                    speedValue.text = $"{avgSpeed:F0}s";
                else
                    speedValue.text = $"{(int)(avgSpeed / 60)}m {(int)(avgSpeed % 60)}s";

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
            Debug.LogError("activityList is not assigned in Inspector!");
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

        // Check if prefab is assigned
        if (activityItemPrefab != null)
        {
            Debug.Log("Using prefab to create activity items");
            foreach (DocumentSnapshot doc in docs)
            {
                CreateActivityItemWithPrefab(doc);
            }
        }
        else
        {
            Debug.LogWarning("No activityItemPrefab assigned, using dynamic creation");
            foreach (DocumentSnapshot doc in docs)
            {
                CreateActivityItemDynamic(doc);
            }
        }
    }

    void CreateActivityItemWithPrefab(DocumentSnapshot doc)
    {
        if (activityItemPrefab == null || activityList == null) return;

        // Get data from document
        string scenarioTitle = doc.ContainsField("scenarioTitle")
            ? doc.GetValue<string>("scenarioTitle") : "Simulation";

        int score = doc.ContainsField("finalScore")
            ? doc.GetValue<int>("finalScore") : 0;

        bool isCorrect = doc.ContainsField("isCorrect")
            ? doc.GetValue<bool>("isCorrect") : false;

        string dateStr = GetDateString(doc);

        Debug.Log($"Creating prefab item: {scenarioTitle} - {score}%");

        // Instantiate the prefab
        GameObject item = Instantiate(activityItemPrefab, activityList);

        // Try to find TextMeshProUGUI components in the prefab
        TextMeshProUGUI[] texts = item.GetComponentsInChildren<TextMeshProUGUI>();

        if (texts.Length >= 2)
        {
            // Assuming order: Title, Score, Date, Status
            texts[0].text = scenarioTitle;
            texts[1].text = $"{score}%";
            texts[1].color = GetScoreColor(score);

            if (texts.Length >= 3)
                texts[2].text = dateStr;
            if (texts.Length >= 4)
                texts[3].text = isCorrect ? "✓ Correct" : "✗ Incorrect";
        }
        else
        {
            Debug.LogWarning($"Prefab has only {texts.Length} TextMeshPro components, expected at least 2");
            // Try to find by name instead
            TryAssignTextsByName(item, scenarioTitle, score, dateStr, isCorrect);
        }
    }

    void TryAssignTextsByName(GameObject item, string title, int score, string dateStr, bool isCorrect)
    {
        // Try to find specific named Text components
        TextMeshProUGUI titleText = FindTextInChildren(item, "Title");
        TextMeshProUGUI scoreText = FindTextInChildren(item, "Score");
        TextMeshProUGUI dateText = FindTextInChildren(item, "Date");
        TextMeshProUGUI statusText = FindTextInChildren(item, "Status");

        if (titleText != null) titleText.text = title;
        if (scoreText != null)
        {
            scoreText.text = $"{score}%";
            scoreText.color = GetScoreColor(score);
        }
        if (dateText != null) dateText.text = dateStr;
        if (statusText != null) statusText.text = isCorrect ? "✓ Correct" : "✗ Incorrect";
    }

    TextMeshProUGUI FindTextInChildren(GameObject parent, string name)
    {
        foreach (Transform child in parent.transform.GetComponentsInChildren<Transform>(true))
        {
            if (child.name == name)
            {
                return child.GetComponent<TextMeshProUGUI>();
            }
        }
        return null;
    }

    void CreateActivityItemDynamic(DocumentSnapshot doc)
    {
        // Get data from document
        string scenarioTitle = doc.ContainsField("scenarioTitle")
            ? doc.GetValue<string>("scenarioTitle") : "Simulation";

        int score = doc.ContainsField("finalScore")
            ? doc.GetValue<int>("finalScore") : 0;

        bool isCorrect = doc.ContainsField("isCorrect")
            ? doc.GetValue<bool>("isCorrect") : false;

        string dateStr = GetDateString(doc);

        Debug.Log($"Creating dynamic item: {scenarioTitle} - {score}%");

        // Create main container
        GameObject item = new GameObject("ActivityItem");
        item.transform.SetParent(activityList, false);

        // Add Layout Element for proper sizing
        LayoutElement layoutElement = item.AddComponent<LayoutElement>();
        layoutElement.minHeight = 80;
        layoutElement.flexibleWidth = 1;

        // Add background image
        Image bg = item.AddComponent<Image>();
        bg.color = new Color(0.95f, 0.97f, 0.98f); // Light blue-gray

        // Add outline
        Outline outline = item.AddComponent<Outline>();
        outline.effectColor = new Color(0.8f, 0.8f, 0.9f);
        outline.effectDistance = new Vector2(1, 1);

        // Create title text
        GameObject titleGO = new GameObject("Title");
        titleGO.transform.SetParent(item.transform, false);
        TextMeshProUGUI title = titleGO.AddComponent<TextMeshProUGUI>();
        title.text = scenarioTitle;
        title.fontSize = 18;
        title.fontStyle = FontStyles.Bold;
        title.color = new Color(0.2f, 0.2f, 0.3f);

        RectTransform titleRt = titleGO.GetComponent<RectTransform>();
        titleRt.anchorMin = new Vector2(0, 0.5f);
        titleRt.anchorMax = new Vector2(0.6f, 0.5f);
        titleRt.offsetMin = new Vector2(16, -15);
        titleRt.offsetMax = new Vector2(-10, 15);

        // Create score text
        GameObject scoreGO = new GameObject("Score");
        scoreGO.transform.SetParent(item.transform, false);
        TextMeshProUGUI scoreText = scoreGO.AddComponent<TextMeshProUGUI>();
        scoreText.text = $"{score}%";
        scoreText.fontSize = 20;
        scoreText.fontStyle = FontStyles.Bold;
        scoreText.alignment = TextAlignmentOptions.MidlineRight;
        scoreText.color = GetScoreColor(score);

        RectTransform scoreRt = scoreGO.GetComponent<RectTransform>();
        scoreRt.anchorMin = new Vector2(0.6f, 0.5f);
        scoreRt.anchorMax = new Vector2(0.85f, 0.5f);
        scoreRt.offsetMin = new Vector2(0, -15);
        scoreRt.offsetMax = new Vector2(-10, 15);

        // Create date text
        GameObject dateGO = new GameObject("Date");
        dateGO.transform.SetParent(item.transform, false);
        TextMeshProUGUI dateText = dateGO.AddComponent<TextMeshProUGUI>();
        dateText.text = dateStr;
        dateText.fontSize = 12;
        dateText.color = new Color(0.6f, 0.6f, 0.7f);
        dateText.alignment = TextAlignmentOptions.BottomLeft;

        RectTransform dateRt = dateGO.GetComponent<RectTransform>();
        dateRt.anchorMin = new Vector2(0, 0);
        dateRt.anchorMax = new Vector2(0.5f, 0);
        dateRt.offsetMin = new Vector2(16, 8);
        dateRt.offsetMax = new Vector2(-10, 28);

        // Create status text
        GameObject statusGO = new GameObject("Status");
        statusGO.transform.SetParent(item.transform, false);
        TextMeshProUGUI statusText = statusGO.AddComponent<TextMeshProUGUI>();
        statusText.text = isCorrect ? "✓ Correct" : "✗ Incorrect";
        statusText.fontSize = 12;
        statusText.alignment = TextAlignmentOptions.BottomRight;
        statusText.color = isCorrect ? new Color(0.086f, 0.635f, 0.290f) : new Color(0.863f, 0.149f, 0.149f);

        RectTransform statusRt = statusGO.GetComponent<RectTransform>();
        statusRt.anchorMin = new Vector2(0.85f, 0);
        statusRt.anchorMax = new Vector2(1, 0);
        statusRt.offsetMin = new Vector2(0, 8);
        statusRt.offsetMax = new Vector2(-16, 28);
    }

    string GetDateString(DocumentSnapshot doc)
    {
        if (doc.ContainsField("completedAt"))
        {
            try
            {
                Timestamp timestamp = doc.GetValue<Timestamp>("completedAt");
                System.DateTime date = timestamp.ToDateTime();

                // Show relative time for recent dates
                System.TimeSpan diff = System.DateTime.Now - date;
                if (diff.TotalHours < 1)
                    return "Just now";
                if (diff.TotalDays < 1)
                    return $"{diff.Hours}h ago";
                if (diff.TotalDays < 7)
                    return $"{diff.Days}d ago";

                return date.ToString("MMM dd, yyyy");
            }
            catch
            {
                return "Recently";
            }
        }
        return "Recently";
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
        // Convert 0-100 score to 1-5 scale
        int level = Mathf.RoundToInt(score / 20f);
        return level switch
        {
            5 => "Very High",
            4 => "High",
            3 => "Medium",
            2 => "Low",
            _ => "Very Low"
        };
    }
}