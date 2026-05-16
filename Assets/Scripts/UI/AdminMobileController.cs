using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using DG.Tweening;

/// <summary>
/// IT Admin landing screen in mobile app.
/// Shows system status and redirects to web portal.
/// </summary>
public class AdminMobileController : MonoBehaviour
{
    [Header("Profile")]
    public TextMeshProUGUI welcomeNameText;
    public TextMeshProUGUI initialText;
    public TextMeshProUGUI institutionText;

    [Header("System Status")]
    public TextMeshProUGUI firebaseStatusText;
    public TextMeshProUGUI authStatusText;
    public TextMeshProUGUI webPortalStatusText;
    public Image firebaseStatusDot;
    public Image authStatusDot;
    public Image webPortalStatusDot;

    [Header("Buttons")]
    public Button webPortalButton;
    public Button logoutButton;

    [Header("Animation Sections")]
    public CanvasGroup topSection;
    public CanvasGroup profileCard;
    public CanvasGroup statusCard;
    public CanvasGroup actionButtons;

    private const string WEB_PORTAL_URL =
        "https://erlinkar-3a41f.web.app";

    private Color _onlineColor =
        new Color(0.086f, 0.635f, 0.290f);
    private Color _offlineColor =
        new Color(0.863f, 0.149f, 0.149f);

    void Start()
    {
        PopulateProfile();
        SetupButtons();
        CheckSystemStatus();
        AnimateEntrance();
    }

    void PopulateProfile()
    {
        var profile = FirebaseManager.CurrentProfile;
        if (profile == null) return;

        if (welcomeNameText != null)
            welcomeNameText.text =
                $"Welcome, {profile.displayName}";

        if (initialText != null)
            initialText.text =
                profile.displayName?[0]
                .ToString().ToUpper() ?? "A";

        if (institutionText != null)
            institutionText.text = "Mapúa University — DOIT";
    }

    void SetupButtons()
    {
        if (webPortalButton != null)
            webPortalButton.onClick.AddListener(() =>
            {
                webPortalButton
                    .GetComponent<RectTransform>()
                    .DOPunchScale(
                        new Vector3(0.05f, 0.05f, 0),
                        0.2f, 3, 0.5f);
                Application.OpenURL(WEB_PORTAL_URL);
            });

        if (logoutButton != null)
            logoutButton.onClick.AddListener(() =>
            {
                FirebaseManager.Instance?.Logout();
                SceneManager.LoadScene("LoginScene");
            });
    }

    async void CheckSystemStatus()
    {
        // Check Firebase connection
        bool firebaseOk = FirebaseManager.IsInitialized;
        bool authOk = FirebaseManager.CurrentUser != null
                       || true; // Auth service available

        SetStatus(firebaseStatusText,
            firebaseStatusDot,
            firebaseOk, "Firebase");

        SetStatus(authStatusText,
            authStatusDot,
            authOk, "Auth Service");

        SetStatus(webPortalStatusText,
            webPortalStatusDot,
            true, "Web Portal"); // Assume online if Firebase works
    }

    void SetStatus(
        TextMeshProUGUI text,
        Image dot,
        bool isOnline,
        string label)
    {
        if (text != null)
        {
            text.text = $"● {label}";
            text.color = isOnline ? _onlineColor : _offlineColor;
        }

        if (dot != null)
            dot.color = isOnline ? _onlineColor : _offlineColor;
    }

    void AnimateEntrance()
    {
        AnimateSection(topSection, 0.0f);
        AnimateSection(profileCard, 0.2f);
        AnimateSection(statusCard, 0.4f);
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
            0.4f)
            .SetDelay(delay)
            .SetEase(Ease.OutCubic);
    }
}