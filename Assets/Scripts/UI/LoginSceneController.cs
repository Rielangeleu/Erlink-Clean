using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using DG.Tweening;
using System.Threading.Tasks;

/// <summary>
/// Controls the Login Scene.
/// Handles Firebase authentication and role-based routing.
/// </summary>
public class LoginSceneController : MonoBehaviour
{
    [Header("Input Fields")]
    public TMP_InputField emailField;
    public TMP_InputField passwordField;

    [Header("Role Toggle")]
    public Button studentToggle;
    public Button instructorToggle;
    public Image studentToggleBg;
    public Image instructorToggleBg;
    public TextMeshProUGUI studentToggleText;
    public TextMeshProUGUI instructorToggleText;

    [Header("Buttons")]
    public Button loginButton;

    [Header("Feedback")]
    public GameObject errorMessage;
    public TextMeshProUGUI errorText;
    public GameObject loadingOverlay;

    [Header("Colors")]
    public Color activeToggleColor =
        new Color(0.145f, 0.337f, 0.922f);
    public Color inactiveToggleColor =
        new Color(0.953f, 0.953f, 0.961f);
    public Color activeTextColor = Color.white;
    public Color inactiveTextColor =
        new Color(0.216f, 0.255f, 0.318f);

    private string _selectedRole = "student";
    private bool _isLoading = false;

    void Start()
    {
        // Animate entrance
        AnimateEntrance();

        // Wire toggle buttons
        studentToggle.onClick.AddListener(
            () => SelectRole("student"));
        instructorToggle.onClick.AddListener(
            () => SelectRole("instructor"));

        // Wire login button
        loginButton.onClick.AddListener(OnLoginClicked);

        // Set default role
        SelectRole("student");

        // Listen for Firebase events
        FirebaseManager.OnLoginSuccess += OnLoginSuccess;
        FirebaseManager.OnLoginFailed += OnLoginFailed;

        // Hide overlays
        if (errorMessage != null) errorMessage.SetActive(false);
        if (loadingOverlay != null) loadingOverlay.SetActive(false);

        // Check if already logged in
        if (FirebaseManager.CurrentUser != null &&
            FirebaseManager.CurrentProfile != null)
        {
            RouteToScene(FirebaseManager.CurrentProfile.role);
        }
    }

    void AnimateEntrance()
    {
        // Fade in from white
        CanvasGroup cg = GetComponent<CanvasGroup>()
            ?? gameObject.AddComponent<CanvasGroup>();
        cg.alpha = 0f;
        cg.DOFade(1f, 0.5f).SetEase(Ease.OutCubic);
    }

    void SelectRole(string role)
    {
        _selectedRole = role;

        bool isStudent = role == "student";

        // Update Student toggle
        studentToggleBg.color = isStudent
            ? activeToggleColor : inactiveToggleColor;
        studentToggleText.color = isStudent
            ? activeTextColor : inactiveTextColor;

        // Update Instructor toggle
        instructorToggleBg.color = !isStudent
            ? activeToggleColor : inactiveToggleColor;
        instructorToggleText.color = !isStudent
            ? activeTextColor : inactiveTextColor;

        // Punch scale for feedback
        Button active = isStudent
            ? studentToggle : instructorToggle;
        active.GetComponent<RectTransform>()
            .DOPunchScale(
                new Vector3(0.03f, 0.03f, 0),
                0.2f, 3, 0.5f);
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
        bool success = await FirebaseManager.Instance
            .LoginAsync(email, password);

        if (!success)
            SetLoading(false);
        // Success is handled by OnLoginSuccess event
    }

    void OnLoginSuccess(UserProfile profile)
    {
        SetLoading(false);
        Debug.Log($"Welcome {profile.displayName} " +
                  $"({profile.role})");
        RouteToScene(profile.role);
    }

    void OnLoginFailed(string error)
    {
        SetLoading(false);
        ShowError(error);

        // Shake login button
        loginButton.GetComponent<RectTransform>()
            .DOShakePosition(0.4f, 8f, 20);
    }

    void RouteToScene(string role)
    {
        switch (role)
        {
            case "student":
                SceneManager.LoadScene("DashboardScene");
                break;
            case "instructor":
                SceneManager.LoadScene("InstructorMobileScene");
                break;
            case "it_admin":
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

        // Animate in
        errorMessage.GetComponent<RectTransform>()
            .DOPunchScale(
                new Vector3(0.02f, 0.02f, 0),
                0.3f, 3, 0.5f);
    }

    void HideError()
    {
        if (errorMessage != null)
            errorMessage.SetActive(false);
    }

    void ShakeField(TMP_InputField field)
    {
        field.GetComponent<RectTransform>()
            .DOShakePosition(0.3f, 6f, 15);
    }

    void OnDestroy()
    {
        FirebaseManager.OnLoginSuccess -= OnLoginSuccess;
        FirebaseManager.OnLoginFailed -= OnLoginFailed;
    }
}