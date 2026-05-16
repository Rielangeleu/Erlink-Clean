using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using DG.Tweening;

/// <summary>
/// Instructor landing screen in the mobile app.
/// Full management is done via web portal.
/// </summary>
public class InstructorMobileController : MonoBehaviour
{
    [Header("Profile Display")]
    public TextMeshProUGUI welcomeNameText;
    public TextMeshProUGUI initialText;
    public TextMeshProUGUI institutionText;
    public Image avatarCircle;

    [Header("Buttons")]
    public Button webPortalButton;
    public Button logoutButton;

    [Header("Footer")]
    public TextMeshProUGUI footerText;

    [Header("Animation")]
    public CanvasGroup topSection;
    public CanvasGroup profileCard;
    public CanvasGroup infoCard;
    public CanvasGroup actionButtons;

    private const string WEB_PORTAL_URL =
        "https://erlinkar-3a41f.web.app";

    void Start()
    {
        PopulateProfile();
        SetupButtons();
        AnimateEntrance();
    }

    void PopulateProfile()
    {
        var profile = FirebaseManager.CurrentProfile;

        if (profile == null)
        {
            Debug.LogWarning("No profile loaded");
            return;
        }

        // Set welcome name
        if (welcomeNameText != null)
            welcomeNameText.text =
                $"Welcome, {profile.displayName}";

        // Set avatar initial
        if (initialText != null)
            initialText.text =
                profile.displayName?[0].ToString()
                .ToUpper() ?? "I";

        // Set institution
        if (institutionText != null)
            institutionText.text =
                profile.institution.Length > 0
                ? profile.institution
                : "Mapúa University";

        // Footer with web URL
        if (footerText != null)
            footerText.text = WEB_PORTAL_URL;
    }

    void SetupButtons()
    {
        if (webPortalButton != null)
            webPortalButton.onClick.AddListener(
                OpenWebPortal);

        if (logoutButton != null)
            logoutButton.onClick.AddListener(Logout);
    }

    void OpenWebPortal()
    {
        // Animate button press
        webPortalButton.GetComponent<RectTransform>()
            .DOPunchScale(
                new Vector3(0.05f, 0.05f, 0),
                0.2f, 3, 0.5f);

        // Open browser
        Application.OpenURL(WEB_PORTAL_URL);
    }

    void Logout()
    {
        FirebaseManager.Instance?.Logout();
        SceneManager.LoadScene("LoginScene");
    }

    void AnimateEntrance()
    {
        // Staggered fade in for each section
        AnimateSection(topSection, 0.0f);
        AnimateSection(profileCard, 0.2f);
        AnimateSection(infoCard, 0.4f);
        AnimateSection(actionButtons, 0.6f);
    }

    void AnimateSection(CanvasGroup cg, float delay)
    {
        if (cg == null) return;
        cg.alpha = 0f;
        cg.transform.localPosition +=
            new Vector3(0, -20, 0);

        cg.DOFade(1f, 0.4f).SetDelay(delay);
        cg.transform.DOLocalMoveY(
            cg.transform.localPosition.y + 20,
            0.4f).SetDelay(delay)
            .SetEase(Ease.OutCubic);
    }
}