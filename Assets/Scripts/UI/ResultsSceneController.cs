using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Firebase.Firestore;
using Firebase.Extensions;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.SceneManagement;

public class ResultsSceneController : MonoBehaviour
{
    [Header("Summary Stats")]
    public TextMeshProUGUI totalSessionsText;
    public TextMeshProUGUI averageScoreText;
    public TextMeshProUGUI bestScoreText;

    [Header("Session List")]
    public Transform sessionsContainer;
    public GameObject sessionCardPrefab;

    [Header("Loading & Empty States")]
    public GameObject loadingPanel;
    public GameObject emptyStatePanel;

    [Header("Navigation Buttons")]
    public Button backButton;
    public Button newSimulationButton;

    private FirebaseFirestore db;
    private string currentUserId;
    private List<SessionData> allSessions = new List<SessionData>();
    private List<SessionData> latestPerScenario = new List<SessionData>();

    void Start()
    {
        db = FirebaseFirestore.DefaultInstance;

        if (backButton != null)
            backButton.onClick.AddListener(() => SceneManager.LoadScene("DashboardScene"));

        if (newSimulationButton != null)
            newSimulationButton.onClick.AddListener(() => SceneManager.LoadScene("ScenarioSelectScene"));

        StartCoroutine(WaitForFirebaseManager());
    }

    System.Collections.IEnumerator WaitForFirebaseManager()
    {
        while (FirebaseManager.Instance == null || FirebaseManager.CurrentUser == null)
        {
            yield return new WaitForSeconds(0.5f);
        }

        currentUserId = FirebaseManager.CurrentUser.UserId;
        Debug.Log($"Loading sessions for userId: {currentUserId}");
        LoadSessions();
    }

    void LoadSessions()
    {
        if (loadingPanel != null) loadingPanel.SetActive(true);
        if (emptyStatePanel != null) emptyStatePanel.SetActive(false);

        db.Collection("sessions")
            .WhereEqualTo("userId", currentUserId)
            .OrderByDescending("completedAt")
            .GetSnapshotAsync()
            .ContinueWithOnMainThread(task =>
            {
                if (loadingPanel != null) loadingPanel.SetActive(false);

                if (task.IsCompleted && !task.IsFaulted && task.Result != null)
                {
                    QuerySnapshot snapshot = task.Result;
                    Debug.Log($"Found {snapshot.Count} total sessions for userId: {currentUserId}");

                    if (snapshot.Count == 0)
                    {
                        if (emptyStatePanel != null) emptyStatePanel.SetActive(true);
                        return;
                    }

                    allSessions.Clear();

                    foreach (DocumentSnapshot doc in snapshot.Documents)
                    {
                        SessionData session = new SessionData
                        {
                            id = doc.Id,
                            scenarioTitle = doc.ContainsField("scenarioTitle") ? doc.GetValue<string>("scenarioTitle") : "Unknown",
                            difficulty = doc.ContainsField("difficulty") ? doc.GetValue<string>("difficulty") : "Unknown",
                            finalScore = doc.ContainsField("finalScore") ? doc.GetValue<int>("finalScore") : 0,
                            accuracyScore = doc.ContainsField("accuracyScore") ? doc.GetValue<int>("accuracyScore") : 0,
                            timeTaken = doc.ContainsField("timeTaken") ? doc.GetValue<float>("timeTaken") : 0,
                            isCorrect = doc.ContainsField("isCorrect") ? doc.GetValue<bool>("isCorrect") : false,
                            completedAt = doc.ContainsField("completedAt") ? doc.GetValue<Timestamp>("completedAt").ToDateTime() : System.DateTime.Now
                        };
                        allSessions.Add(session);
                    }

                    // Calculate summary stats from ALL sessions
                    CalculateSummaryStats();

                    // Get only the LATEST session for each scenario
                    GetLatestPerScenario();

                    // Display latest sessions
                    DisplaySessions();
                }
                else if (task.IsFaulted)
                {
                    Debug.LogError($"Failed to load sessions: {task.Exception}");
                    if (emptyStatePanel != null)
                    {
                        TextMeshProUGUI errorText = emptyStatePanel.GetComponentInChildren<TextMeshProUGUI>();
                        if (errorText != null) errorText.text = "Failed to load sessions.\nCheck your connection.";
                        emptyStatePanel.SetActive(true);
                    }
                }
            });
    }

    void CalculateSummaryStats()
    {
        int total = allSessions.Count;
        float sum = 0;
        int best = 0;

        foreach (var session in allSessions)
        {
            sum += session.finalScore;
            if (session.finalScore > best)
                best = session.finalScore;
        }

        int average = total > 0 ? Mathf.RoundToInt(sum / total) : 0;

        // Update Total Sessions (no color needed)
        if (totalSessionsText != null)
            totalSessionsText.text = total.ToString();

        // Update Average Score with COLOR CODING
        if (averageScoreText != null)
        {
            averageScoreText.text = $"{average}%";
            averageScoreText.color = GetScoreColor(average);
        }

        // Update Best Score with COLOR CODING
        if (bestScoreText != null)
        {
            bestScoreText.text = $"{best}%";
            bestScoreText.color = GetScoreColor(best);
        }

        Debug.Log($"Summary Stats - Total Sessions: {total}, Average: {average}%, Best: {best}%");
    }

    void GetLatestPerScenario()
    {
        latestPerScenario.Clear();

        // Group by scenario title and take ONLY the most recent (first because ordered by date desc)
        var grouped = allSessions
            .GroupBy(s => s.scenarioTitle)
            .Select(g => g.OrderByDescending(s => s.completedAt).First());

        latestPerScenario = grouped.ToList();

        Debug.Log($"Showing {latestPerScenario.Count} unique scenarios (latest attempt only)");

        // Log each latest session for debugging
        foreach (var session in latestPerScenario)
        {
            Debug.Log($"Latest {session.scenarioTitle}: {session.finalScore}% on {session.completedAt:MMM dd, yyyy}");
        }
    }

    void DisplaySessions()
    {
        if (sessionCardPrefab == null || sessionsContainer == null)
        {
            Debug.LogError("Session Card Prefab or Sessions Container not assigned!");
            return;
        }

        // Clear existing cards (keep StatsCard, LoadingPanel, EmptyStatePanel)
        foreach (Transform child in sessionsContainer)
        {
            if (child.gameObject.name != "StatsCard" &&
                child.gameObject.name != "LoadingPanel" &&
                child.gameObject.name != "EmptyStatePanel")
            {
                Destroy(child.gameObject);
            }
        }

        // Sort by most recent date
        var sortedSessions = latestPerScenario.OrderByDescending(s => s.completedAt).ToList();

        foreach (var session in sortedSessions)
        {
            GameObject card = Instantiate(sessionCardPrefab, sessionsContainer);

            TextMeshProUGUI[] texts = card.GetComponentsInChildren<TextMeshProUGUI>();

            foreach (var text in texts)
            {
                switch (text.gameObject.name)
                {
                    case "ScenarioTitle":
                        text.text = $"{GetScenarioIcon(session.scenarioTitle)} {session.scenarioTitle}";
                        break;
                    case "DateText":
                        text.text = session.completedAt.ToString("MMM dd, yyyy");
                        break;
                    case "DifficultyBadge":
                        text.text = session.difficulty;
                        text.color = GetDifficultyColor(session.difficulty);
                        break;
                    case "ScoreValue":
                        text.text = $"{session.finalScore}%";
                        text.color = GetScoreColor(session.finalScore);
                        break;
                    case "AccuracyValue":
                        text.text = $"{session.accuracyScore}%";
                        text.color = GetScoreColor(session.accuracyScore);
                        break;
                    case "TimeValue":
                        int mins = Mathf.FloorToInt(session.timeTaken / 60f);
                        int secs = Mathf.FloorToInt(session.timeTaken % 60f);
                        text.text = mins > 0 ? $"{mins}m {secs}s" : $"{secs}s";
                        text.color = GetTimeColor(session.timeTaken);
                        break;
                    case "ResultValue":
                        text.text = session.isCorrect ? "✅ Correct" : "❌ Incorrect";
                        text.color = session.isCorrect ? new Color32(46, 204, 113, 255) : new Color32(231, 76, 60, 255);
                        break;
                }
            }
        }
    }

    // Color coding for scores (0-100)
    Color GetScoreColor(int score)
    {
        if (score >= 85) return new Color32(46, 204, 113, 255);     // Green - High/Excellent
        if (score >= 70) return new Color32(241, 196, 15, 255);     // Yellow - Average/Satisfactory
        return new Color32(231, 76, 60, 255);                       // Red - Low/Needs Improvement
    }

    // Color coding for time taken
    Color GetTimeColor(float timeInSeconds)
    {
        if (timeInSeconds <= 60) return new Color32(46, 204, 113, 255);     // Green - Fast (≤60s)
        if (timeInSeconds <= 120) return new Color32(241, 196, 15, 255);    // Yellow - Medium (61-120s)
        return new Color32(231, 76, 60, 255);                                // Red - Slow (>120s)
    }

    string GetScenarioIcon(string title)
    {
        if (string.IsNullOrEmpty(title)) return "🏥";
        if (title.ToLower().Contains("blue") || title.ToLower().Contains("code")) return "🫀";
        if (title.ToLower().Contains("hemorrhage")) return "🩸";
        if (title.ToLower().Contains("fracture")) return "🦴";
        if (title.ToLower().Contains("chemical") || title.ToLower().Contains("hazmat")) return "☣️";
        if (title.ToLower().Contains("multi")) return "👥";
        return "🏥";
    }

    Color32 GetDifficultyColor(string difficulty)
    {
        if (string.IsNullOrEmpty(difficulty)) return new Color32(149, 165, 166, 255);
        switch (difficulty.ToLower())
        {
            case "easy": return new Color32(46, 204, 113, 255);
            case "medium": return new Color32(241, 196, 15, 255);
            case "hard": return new Color32(231, 76, 60, 255);
            default: return new Color32(149, 165, 166, 255);
        }
    }

    public void LoadNewSimulation()
    {
        SceneManager.LoadScene("ScenarioSelectScene");
    }

    [System.Serializable]
    public class SessionData
    {
        public string id;
        public string scenarioTitle;
        public string difficulty;
        public int finalScore;
        public int accuracyScore;
        public float timeTaken;
        public bool isCorrect;
        public System.DateTime completedAt;
    }
}