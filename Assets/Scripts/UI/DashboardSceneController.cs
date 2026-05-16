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
        LoadPerformanceData();
    }

    void SetupWelcome()
    {
        if (welcomeText != null)
        {
            string name = FirebaseManager.CurrentProfile?
                .displayName ?? "Student";
            welcomeText.text = $"Welcome, {name}";
        }

        // Start simulation button
        if (startSimulationCard != null)
            startSimulationCard.onClick.AddListener(() =>
                SceneManager.LoadScene("ScenarioSelectScene"));

        // View results button
        if (viewResultsCard != null)
            viewResultsCard.onClick.AddListener(() =>
                SceneManager.LoadScene("ResultsScene"));
    }

    void SetupNavigation()
    {
        if (simulateNav != null)
            simulateNav.onClick.AddListener(() =>
                SceneManager.LoadScene("ScenarioSelectScene"));

        if (resultsNav != null)
            resultsNav.onClick.AddListener(() =>
                SceneManager.LoadScene("ResultsScene"));

        if (profileNav != null)
            profileNav.onClick.AddListener(() =>
                SceneManager.LoadScene("ProfileScene"));

        if (logoutButton != null)
            logoutButton.onClick.AddListener(OnLogout);
    }

    async void LoadPerformanceData()
    {
        if (FirebaseManager.Instance == null ||
            FirebaseManager.CurrentUser == null)
        {
            ShowEmptyStats();
            return;
        }

        try
        {
            // Query last 10 sessions for this user
            QuerySnapshot snap = await FirebaseFirestore
                .DefaultInstance
                .Collection("sessions")
                .WhereEqualTo("userId",
                    FirebaseManager.CurrentUser.UserId)
                .OrderByDescending("completedAt")
                .Limit(10)
                .GetSnapshotAsync();

            if (snap.Count == 0)
            {
                ShowEmptyStats();
                return;
            }

            // Calculate averages
            List<int> accuracies = new List<int>();
            List<float> speeds = new List<float>();
            List<int> confidences = new List<int>();

            foreach (DocumentSnapshot doc in snap.Documents)
            {
                if (doc.ContainsField("accuracyScore"))
                    accuracies.Add(
                        doc.GetValue<int>("accuracyScore"));
                if (doc.ContainsField("timeTaken"))
                    speeds.Add(
                        doc.GetValue<float>("timeTaken"));
                if (doc.ContainsField("confidenceScore"))
                    confidences.Add(
                        doc.GetValue<int>("confidenceScore"));
            }

            // Update UI
            if (accuracies.Count > 0 && accuracyValue != null)
            {
                float avg = (float)accuracies.Average();
                accuracyValue.text = $"{avg:F0}%";
                accuracyValue.color = GetScoreColor((int)avg);
            }

            if (speeds.Count > 0 && speedValue != null)
            {
                float avgSpeed = (float)speeds.Average();
                speedValue.text = avgSpeed < 60
                    ? $"{avgSpeed:F0}s"
                    : $"{avgSpeed / 60:F0}m {avgSpeed % 60:F0}s";
            }

            if (confidences.Count > 0 &&
                confidenceValue != null)
            {
                float avgConf = (float)confidences.Average();
                confidenceValue.text =
                    GetConfidenceLabel((int)avgConf);
            }

            if (accuracySub != null)
                accuracySub.text =
                    $"Last {snap.Count} simulations";

            // Populate recent activity
            PopulateRecentActivity(snap.Documents.Take(5)
                .ToList());
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Dashboard load failed: {e.Message}");
            ShowEmptyStats();
        }
    }

    void PopulateRecentActivity(
        List<DocumentSnapshot> docs)
    {
        if (activityList == null) return;

        // Hide placeholder
        if (noActivityPlaceholder != null)
            noActivityPlaceholder.SetActive(false);

        foreach (DocumentSnapshot doc in docs)
        {
            if (activityItemPrefab == null)
            {
                CreateActivityItemDynamic(doc);
                continue;
            }

            GameObject item = Instantiate(
                activityItemPrefab, activityList);

            // Populate item fields
            TextMeshProUGUI[] texts =
                item.GetComponentsInChildren
                <TextMeshProUGUI>();

            if (texts.Length >= 2)
            {
                string title = doc.ContainsField("scenarioTitle")
                    ? doc.GetValue<string>("scenarioTitle")
                    : "Simulation";
                int score = doc.ContainsField("finalScore")
                    ? doc.GetValue<int>("finalScore") : 0;

                texts[0].text = title;
                texts[1].text = $"Score: {score}%";
                texts[1].color = GetScoreColor(score);
            }
        }
    }

    void CreateActivityItemDynamic(DocumentSnapshot doc)
    {
        // Create item without prefab
        GameObject item = new GameObject("ActivityItem");
        item.transform.SetParent(activityList, false);

        RectTransform rt = item.AddComponent<RectTransform>();
        rt.sizeDelta = new Vector2(0, 80);

        Image bg = item.AddComponent<Image>();
        bg.color = new Color(0.973f, 0.980f, 0.988f);

        UnityEngine.UI.LayoutElement le =
            item.AddComponent<UnityEngine.UI.LayoutElement>();
        le.minHeight = 80;

        // Title
        GameObject titleGO = new GameObject("Title");
        titleGO.transform.SetParent(item.transform, false);
        TextMeshProUGUI title =
            titleGO.AddComponent<TextMeshProUGUI>();

        string scenarioTitle = doc.ContainsField("scenarioTitle")
            ? doc.GetValue<string>("scenarioTitle")
            : "Simulation";
        int score = doc.ContainsField("finalScore")
            ? doc.GetValue<int>("finalScore") : 0;

        title.text = $"{scenarioTitle} — {score}%";
        title.fontSize = 24;
        title.color = GetScoreColor(score);

        RectTransform titleRt =
            titleGO.GetComponent<RectTransform>();
        titleRt.anchorMin = Vector2.zero;
        titleRt.anchorMax = Vector2.one;
        titleRt.offsetMin = new Vector2(16, 0);
        titleRt.offsetMax = new Vector2(-16, 0);
    }

    void ShowEmptyStats()
    {
        if (accuracyValue != null) accuracyValue.text = "--";
        if (speedValue != null) speedValue.text = "--";
        if (confidenceValue != null) confidenceValue.text = "--";

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
        if (score >= 90)
            return new Color(0.086f, 0.635f, 0.290f);
        if (score >= 80)
            return new Color(0.145f, 0.337f, 0.922f);
        if (score >= 70)
            return new Color(0.851f, 0.467f, 0.024f);
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