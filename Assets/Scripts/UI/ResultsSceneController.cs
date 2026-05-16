using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using Firebase.Firestore;
using Firebase.Extensions;
using System.Collections.Generic;

public class ResultsSceneController : MonoBehaviour
{
    [Header("UI")]
    public Transform resultsList;
    public TextMeshProUGUI noResultsText;
    public Button backButton;

    void Start()
    {
        if (backButton != null)
            backButton.onClick.AddListener(() =>
                SceneManager.LoadScene("DashboardScene"));

        LoadResults();
    }

    async void LoadResults()
    {
        if (FirebaseManager.CurrentUser == null) return;

        try
        {
            var snap = await FirebaseFirestore.DefaultInstance
                .Collection("sessions")
                .WhereEqualTo("userId",
                    FirebaseManager.CurrentUser.UserId)
                .OrderByDescending("completedAt")
                .Limit(20)
                .GetSnapshotAsync();

            if (snap.Count == 0)
            {
                if (noResultsText != null)
                    noResultsText.gameObject.SetActive(true);
                return;
            }

            foreach (var doc in snap.Documents)
            {
                CreateResultItem(doc);
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Results load failed: {e.Message}");
        }
    }

    void CreateResultItem(DocumentSnapshot doc)
    {
        if (resultsList == null) return;

        GameObject item = new GameObject("ResultItem");
        item.transform.SetParent(resultsList, false);

        var le = item.AddComponent<UnityEngine.UI.LayoutElement>();
        le.minHeight = 80;

        var bg = item.AddComponent<UnityEngine.UI.Image>();
        bg.color = new Color(0.97f, 0.98f, 0.99f);

        var rt = item.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(0, 80);

        // Title text
        var titleGO = new GameObject("Title");
        titleGO.transform.SetParent(item.transform, false);
        var title = titleGO.AddComponent<TMPro.TextMeshProUGUI>();

        string scenario = doc.ContainsField("scenarioTitle")
            ? doc.GetValue<string>("scenarioTitle") : "Simulation";
        int score = doc.ContainsField("finalScore")
            ? doc.GetValue<int>("finalScore") : 0;
        bool correct = doc.ContainsField("isCorrect") &&
            doc.GetValue<bool>("isCorrect");

        title.text = $"{scenario} — {score}% {(correct ? "✓" : "✗")}";
        title.fontSize = 24;
        title.color = score >= 80
            ? new Color(0.09f, 0.64f, 0.29f)
            : new Color(0.86f, 0.15f, 0.15f);

        var titleRt = titleGO.GetComponent<RectTransform>();
        titleRt.anchorMin = Vector2.zero;
        titleRt.anchorMax = Vector2.one;
        titleRt.offsetMin = new Vector2(16, 0);
        titleRt.offsetMax = new Vector2(-16, 0);
    }
}