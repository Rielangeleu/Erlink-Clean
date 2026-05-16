using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Dynamically populates the Patient Info Card
/// based on the loaded scenario data.
/// </summary>
public class PatientInfoCardUI : MonoBehaviour
{
    [Header("Scenario Badge")]
    public TextMeshProUGUI scenarioBadgeText;
    public Image scenarioBadgeBackground;

    [Header("Patient Description")]
    public TextMeshProUGUI patientDescriptionText;

    [Header("Vitals Row")]
    public GameObject vitalsRow;
    public GameObject vitalPillPrefab;

    [Header("Code Alert")]
    public GameObject codeAlertBanner;
    public TextMeshProUGUI codeAlertText;
    public Image codeAlertBackground;

    [Header("Difficulty Colors")]
    public Color easyColor = new Color(0.086f, 0.635f, 0.290f); // green
    public Color mediumColor = new Color(0.851f, 0.467f, 0.024f); // orange
    public Color hardColor = new Color(0.863f, 0.149f, 0.149f); // red

    public void PopulateFromScenario(ScenarioData scenario)
    {
        if (scenario == null) return;

        // ── Scenario Badge ──────────────────────────
        if (scenarioBadgeText != null)
            scenarioBadgeText.text = scenario.scenarioTitle
                .ToUpper();

        if (scenarioBadgeBackground != null)
        {
            scenarioBadgeBackground.color =
                scenario.difficulty switch
                {
                    DifficultyLevel.Easy => easyColor,
                    DifficultyLevel.Medium => mediumColor,
                    DifficultyLevel.Hard => hardColor,
                    _ => easyColor
                };
        }

        // ── Patient Description ──────────────────────
        if (patientDescriptionText != null)
        {
            string gender = scenario.patientGender;
            string age = scenario.patientAge;
            string status = GetStatusFromPresentation(
                scenario.patientPresentation);

            patientDescriptionText.text =
                $"Patient — {gender}, {age} · {status}";
        }

        // ── Vitals Row ───────────────────────────────
        PopulateVitals(scenario.vitalSigns);

        // ── Code Alert ───────────────────────────────
        ShowCodeAlert(scenario.emergencyCode);
    }

    void PopulateVitals(string[] vitals)
    {
        if (vitalsRow == null || vitals == null) return;

        // Clear existing pills
        foreach (Transform child in vitalsRow.transform)
            Destroy(child.gameObject);

        // Create one pill per vital sign
        foreach (string vital in vitals)
        {
            if (vitalPillPrefab == null)
            {
                // Create pill dynamically if no prefab
                CreateVitalPill(vital);
            }
            else
            {
                GameObject pill = Instantiate(
                    vitalPillPrefab, vitalsRow.transform);
                pill.GetComponentInChildren<TextMeshProUGUI>()
                    .text = vital;
            }
        }
    }

    void CreateVitalPill(string vitalText)
    {
        // Create pill GameObject dynamically
        GameObject pill = new GameObject("VitalPill");
        pill.transform.SetParent(vitalsRow.transform, false);

        // Add Image background
        Image bg = pill.AddComponent<Image>();
        bg.color = new Color(0.059f, 0.090f, 0.165f); // #0F172A

        // Add RectTransform sizing
        RectTransform rt = pill.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(180, 36);

        // Add Layout Element
        UnityEngine.UI.LayoutElement le =
            pill.AddComponent<UnityEngine.UI.LayoutElement>();
        le.preferredWidth = 180;
        le.preferredHeight = 36;

        // Add TextMeshPro
        GameObject textGO = new GameObject("Text");
        textGO.transform.SetParent(pill.transform, false);
        TextMeshProUGUI tmp = textGO.AddComponent<TextMeshProUGUI>();
        tmp.text = vitalText;
        tmp.fontSize = 18;
        tmp.color = new Color(0.973f, 0.529f, 0.443f); // #F87171 red
        tmp.alignment = TextAlignmentOptions.Center;

        RectTransform textRt = textGO.GetComponent<RectTransform>();
        textRt.anchorMin = Vector2.zero;
        textRt.anchorMax = Vector2.one;
        textRt.offsetMin = Vector2.zero;
        textRt.offsetMax = Vector2.zero;
    }

    void ShowCodeAlert(EmergencyCode code)
    {
        if (codeAlertBanner == null) return;

        if (code == EmergencyCode.None)
        {
            codeAlertBanner.SetActive(false);
            return;
        }

        codeAlertBanner.SetActive(true);

        string alertText = code switch
        {
            EmergencyCode.CodeBlue =>
                "CODE BLUE — CARDIAC/RESPIRATORY ARREST",
            EmergencyCode.CodeRed =>
                "CODE RED — FIRE EMERGENCY",
            EmergencyCode.CodeOrange =>
                "CODE ORANGE — HAZARDOUS MATERIALS",
            EmergencyCode.CodeSilver =>
                "CODE SILVER — ACTIVE THREAT",
            _ => ""
        };

        if (codeAlertText != null)
            codeAlertText.text = alertText;

        // Color based on code
        if (codeAlertBackground != null)
        {
            codeAlertBackground.color = code switch
            {
                EmergencyCode.CodeBlue =>
                    new Color(0.863f, 0.149f, 0.149f), // red
                EmergencyCode.CodeOrange =>
                    new Color(0.851f, 0.467f, 0.024f), // orange
                _ => new Color(0.863f, 0.149f, 0.149f)
            };
        }
    }

    string GetStatusFromPresentation(string presentation)
    {
        // Extract a short status from the long presentation text
        if (string.IsNullOrEmpty(presentation))
            return "Unresponsive";

        // Return first meaningful word group
        if (presentation.Contains("unconscious") ||
            presentation.Contains("unresponsive"))
            return "Unresponsive";
        if (presentation.Contains("confused") ||
            presentation.Contains("altered"))
            return "Confused";
        if (presentation.Contains("alert"))
            return "Alert & Oriented";

        return "Assess Required";
    }
}