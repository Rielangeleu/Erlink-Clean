using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Dynamically populates the Patient Info Card based on loaded scenario data.
/// Controls step-by-step reveal parameters for thesis sequential compliance tracking.
/// </summary>
public class PatientInfoCardUI : MonoBehaviour
{
    [Header("Scenario Badge")]
    public TextMeshProUGUI scenarioBadgeText;
    public Image scenarioBadgeBackground;

    [Header("Patient Description")]
    public TextMeshProUGUI patientDescriptionText;

    [Header("Explicit Vitals Row Pillars")]
    public GameObject vitalsRow;

    // Direct references to your individual structural layout pills
    public GameObject rrPill;
    public GameObject prPill;
    public GameObject msPill;

    [Header("Code Alert")]
    public GameObject codeAlertBanner;
    public TextMeshProUGUI codeAlertText;
    public Image codeAlertBackground;

    [Header("Difficulty Colors")]
    public Color easyColor = new Color(0.086f, 0.635f, 0.290f);
    public Color mediumColor = new Color(0.851f, 0.467f, 0.024f);
    public Color hardColor = new Color(0.863f, 0.149f, 0.149f);

    private ScenarioData _cachedScenario;

    public void PopulateFromScenario(ScenarioData scenario)
    {
        if (scenario == null) return;
        _cachedScenario = scenario;

        // ── Scenario Badge ──────────────────────────
        if (scenarioBadgeText != null)
            scenarioBadgeText.text = scenario.scenarioTitle.ToUpper();

        if (scenarioBadgeBackground != null)
        {
            scenarioBadgeBackground.color = scenario.difficulty switch
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
            string status = GetStatusFromPresentation(scenario.patientPresentation);

            patientDescriptionText.text = $"Patient — {gender}, {age} · {status}";
        }

        // ── INITIAL BLANK STATE ──
        // Hide the vitals row container or individual pills when patient first spawns
        if (vitalsRow != null) vitalsRow.SetActive(true);

        if (rrPill != null) rrPill.SetActive(false);
        if (prPill != null) prPill.SetActive(false);
        if (msPill != null) msPill.SetActive(false);

        // ── Code Alert Banner ───────────────────────────
        ShowCodeAlert(scenario.emergencyCode);
    }

    
    // ── STEP EXPLICIT REVEALERS ──
    public void RevealRespirationValue(string value)
    {
        if (rrPill != null)
        {
            rrPill.SetActive(true);
            var textComp = rrPill.GetComponentInChildren<TextMeshProUGUI>();
            // FIXED: Displays only the raw text value directly
            if (textComp != null) textComp.text = value;
        }
    }

    public void RevealPerfusionValue(string value)
    {
        if (prPill != null)
        {
            prPill.SetActive(true);
            var textComp = prPill.GetComponentInChildren<TextMeshProUGUI>();
            // FIXED: Displays only the raw text value directly
            if (textComp != null) textComp.text = value;
        }
    }

    public void RevealMentalValue(string value)
    {
        if (msPill != null)
        {
            msPill.SetActive(true);
            var textComp = msPill.GetComponentInChildren<TextMeshProUGUI>();
            // FIXED: Displays only the raw text value directly
            if (textComp != null) textComp.text = value;
        }
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
            EmergencyCode.CodeBlue => "CODE BLUE — CARDIAC/RESPIRATORY ARREST",
            EmergencyCode.CodeRed => "CODE RED — FIRE EMERGENCY",
            EmergencyCode.CodeOrange => "CODE ORANGE — HAZARDOUS MATERIALS",
            EmergencyCode.CodeSilver => "CODE SILVER — ACTIVE THREAT",
            _ => ""
        };

        if (codeAlertText != null)
            codeAlertText.text = alertText;

        if (codeAlertBackground != null)
        {
            codeAlertBackground.color = code switch
            {
                EmergencyCode.CodeBlue => new Color(0.863f, 0.149f, 0.149f),
                EmergencyCode.CodeOrange => new Color(0.851f, 0.467f, 0.024f),
                _ => new Color(0.863f, 0.149f, 0.149f)
            };
        }
    }

    string GetStatusFromPresentation(string presentation)
    {
        if (string.IsNullOrEmpty(presentation)) return "Unresponsive";
        if (presentation.Contains("unconscious") || presentation.Contains("unresponsive")) return "Unresponsive";
        if (presentation.Contains("confused") || presentation.Contains("altered")) return "Confused";
        if (presentation.Contains("alert")) return "Alert & Oriented";
        return "Assess Required";
    }
}