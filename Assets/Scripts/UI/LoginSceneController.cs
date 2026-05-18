using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using DG.Tweening;
using System.Threading.Tasks;

public enum UserRole
{
    Student,
    Instructor,
    Admin
}

/// <summary>
/// Controls the Login Scene.
/// Handles Firebase authentication, responsive layout scaling, and 3-way role routing.
/// </summary>
public class LoginSceneController : MonoBehaviour
{
    [Header("Input Fields")]
    public TMP_InputField emailField;
    public TMP_InputField passwordField;

    [Header("3-Way Role Toggles")]
    public Button studentToggle;
    public Button instructorToggle;
    public Button adminToggle; // Drag your brand new Admin option button here in the Inspector!

    [Space(5)]
    public Image studentToggleBg;
    public Image instructorToggleBg;
    public Image adminToggleBg; // Drag your Admin button's background Image component here!

    [Space(5)]
    public TextMeshProUGUI studentToggleText;
    public TextMeshProUGUI instructorToggleText;
    public TextMeshProUGUI adminToggleText; // Drag your Admin button's Text component here!

    [Header("Buttons")]
    public Button loginButton;

    [Header("Feedback")]
    public GameObject errorMessage;
    public TextMeshProUGUI errorText;
    public GameObject loadingOverlay;

    [Header("Colors")]
    public Color activeToggleColor = new Color(0.145f, 0.337f, 0.922f); // #2563EB Active Vibrant Blue
    public Color inactiveToggleColor = new Color(0.953f, 0.953f, 0.961f); // #F1F1F3 Clean Off-White Muted Grey
    public Color activeTextColor = Color.white;
    public Color inactiveTextColor = new Color(0.216f, 0.255f, 0.318f); // Dark slate charcoal gray look

    private UserRole _selectedRole = UserRole.Student;
    private bool _isLoading = false;

    void Start()
    {
        // Animate entrance
        AnimateEntrance();

        // Wire 3-way selection buttons with explicit lambda listeners
        if (studentToggle != null)
            studentToggle.onClick.AddListener(() => SelectRole(UserRole.Student));

        if (instructorToggle != null)
            instructorToggle.onClick.AddListener(() => SelectRole(UserRole.Instructor));

        if (adminToggle != null)
            adminToggle.onClick.AddListener(() => SelectRole(UserRole.Admin));

        // Wire login button
        if (loginButton != null)
            loginButton.onClick.AddListener(OnLoginClicked);

        // Set default role state on initialization frame zero
        SelectRole(UserRole.Student);

        // Listen for Firebase events
        FirebaseManager.OnLoginSuccess += OnLoginSuccess;
        FirebaseManager.OnLoginFailed += OnLoginFailed;

        // Hide overlays
        if (errorMessage != null) errorMessage.SetActive(false);
        if (loadingOverlay != null) loadingOverlay.SetActive(false);

        // Check if already logged in
        if (FirebaseManager.CurrentUser != null && FirebaseManager.CurrentProfile != null)
        {
            RouteToScene(FirebaseManager.CurrentProfile.role);
        }
    }

    void AnimateEntrance()
    {
        // Fade in cleanly via Canvas Group alpha interpolation matrix checks
        CanvasGroup cg = GetComponent<CanvasGroup>() ?? gameObject.AddComponent<CanvasGroup>();
        cg.alpha = 0f;
        cg.DOFade(1f, 0.5f).SetEase(Ease.OutCubic);
    }

    public void SelectRole(UserRole role)
    {
        _selectedRole = role;

        // 1. Process Structural Visual Component Refreshes Across All Three Buttons
        UpdateToggleVisuals(studentToggleBg, studentToggleText, _selectedRole == UserRole.Student);
        UpdateToggleVisuals(instructorToggleBg, instructorToggleText, _selectedRole == UserRole.Instructor);
        UpdateToggleVisuals(adminToggleBg, adminToggleText, _selectedRole == UserRole.Admin);

        // 2. Punch Scale Feedback for the user click target
        Button activeTargetButton = _selectedRole switch
        {
            UserRole.Student => studentToggle,
            UserRole.Instructor => instructorToggle,
            UserRole.Admin => adminToggle,
            _ => studentToggle
        };

        if (activeTargetButton != null)
        {
            activeTargetButton.GetComponent<RectTransform>()
                .DOPunchScale(new Vector3(0.03f, 0.03f, 0), 0.2f, 3, 0.5f);
        }
    }

    void UpdateToggleVisuals(Image bgImage, TextMeshProUGUI textMesh, bool isActive)
    {
        if (bgImage != null) bgImage.color = isActive ? activeToggleColor : inactiveToggleColor;
        if (textMesh != null) textMesh.color = isActive ? activeTextColor : inactiveTextColor;
    }

    async void OnLoginClicked()
    {
        if (_isLoading) return;

        // Validate inputs
        string email = emailField.text.Trim();
        string password = passwordField.text;

        if (string.IsNullOrEmpty(email))
        {
            ShowError("Please enter your email address.");
            ShakeField(emailField);
            return;
        }

        if (string.IsNullOrEmpty(password))
        {
            ShowError("Please enter your password.");
            ShakeField(passwordField);
            return;
        }

        if (!email.Contains("@"))
        {
            ShowError("Please enter a valid email address.");
            ShakeField(emailField);
            return;
        }

        // Show loading
        SetLoading(true);
        HideError();

        // Attempt Firebase login
        bool success = await FirebaseManager.Instance.LoginAsync(email, password);

        if (!success)
            SetLoading(false);
    }

    void OnLoginSuccess(UserProfile profile)
    {
        SetLoading(false);
        Debug.Log($"Welcome {profile.displayName} ({profile.role})");
        RouteToScene(profile.role);
    }

    void OnLoginFailed(string error)
    {
        SetLoading(false);
        ShowError(error);

        // Shake login button
        if (loginButton != null)
        {
            loginButton.GetComponent<RectTransform>()
                .DOShakePosition(0.4f, 8f, 20);
        }
    }

    void RouteToScene(string role)
    {
        // Enforces string conversions back down safely from your database profiles string keys
        switch (role.ToLower().Trim())
        {
            case "student":
                SceneManager.LoadScene("DashboardScene");
                break;
            case "instructor":
                SceneManager.LoadScene("InstructorMobileScene");
                break;
            case "it_admin":
            case "admin":
                SceneManager.LoadScene("AdminMobileScene");
                break;
            default:
                SceneManager.LoadScene("DashboardScene");
                break;
        }
    }

    void SetLoading(bool loading)
    {
        _isLoading = loading;
        if (loadingOverlay != null)
            loadingOverlay.SetActive(loading);
        if (loginButton != null)
            loginButton.interactable = !loading;
    }

    void ShowError(string message)
    {
        if (errorMessage == null) return;
        errorMessage.SetActive(true);
        if (errorText != null)
            errorText.text = message;

        errorMessage.GetComponent<RectTransform>()
            .DOPunchScale(new Vector3(0.02f, 0.02f, 0), 0.3f, 3, 0.5f);
    }

    void ShowErrorTextDirectly(string text)
    {
        ShowError(text);
    }

    void HideError()
    {
        if (errorMessage != null)
            errorMessage.SetActive(false);
    }

    void ShakeField(TMP_InputField field)
    {
        if (field != null)
        {
            field.GetComponent<RectTransform>()
                .DOShakePosition(0.3f, 6f, 15);
        }
    }

    void OnDestroy()
    {
        FirebaseManager.OnLoginSuccess -= OnLoginSuccess;
        FirebaseManager.OnLoginFailed -= OnLoginFailed;
    }
}