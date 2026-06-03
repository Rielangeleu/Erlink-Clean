using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using DG.Tweening;
using System.Threading.Tasks;
using System.Collections;

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
    public Button adminToggle;

    [Space(5)]
    public Image studentToggleBg;
    public Image instructorToggleBg;
    public Image adminToggleBg;

    [Space(5)]
    public TextMeshProUGUI studentToggleText;
    public TextMeshProUGUI instructorToggleText;
    public TextMeshProUGUI adminToggleText;

    [Header("Buttons")]
    public Button loginButton;
    public Button registerButton;

    [Header("Feedback")]
    public GameObject errorMessage;
    public TextMeshProUGUI errorText;
    public GameObject loadingOverlay;

    [Header("Colors")]
    public Color activeToggleColor = new Color(0.145f, 0.337f, 0.922f);
    public Color inactiveToggleColor = new Color(0.953f, 0.953f, 0.961f);
    public Color activeTextColor = Color.white;
    public Color inactiveTextColor = new Color(0.216f, 0.255f, 0.318f);

    private UserRole _selectedRole = UserRole.Student;
    private bool _isLoading = false;

    void Start()
    {
        AnimateEntrance();

        if (studentToggle != null)
            studentToggle.onClick.AddListener(() => SelectRole(UserRole.Student));

        if (instructorToggle != null)
            instructorToggle.onClick.AddListener(() => SelectRole(UserRole.Instructor));

        if (adminToggle != null)
            adminToggle.onClick.AddListener(() => SelectRole(UserRole.Admin));

        if (loginButton != null)
            loginButton.onClick.AddListener(OnLoginClicked);

        if (registerButton != null)
            registerButton.onClick.AddListener(() => SceneManager.LoadScene("RegisterScene"));

        SelectRole(UserRole.Student);
        FirebaseManager.OnLoginSuccess += OnLoginSuccess;
        FirebaseManager.OnLoginFailed += OnLoginFailed;

        if (errorMessage != null) errorMessage.SetActive(false);
        if (loadingOverlay != null) loadingOverlay.SetActive(false);

        if (FirebaseManager.CurrentUser != null && FirebaseManager.CurrentProfile != null)
        {
            RouteToScene(FirebaseManager.CurrentProfile.role);
        }
    }

    void AnimateEntrance()
    {
        CanvasGroup cg = GetComponent<CanvasGroup>() ?? gameObject.AddComponent<CanvasGroup>();
        cg.alpha = 0f;
        cg.DOFade(1f, 0.5f).SetEase(Ease.OutCubic);
    }

    public void SelectRole(UserRole role)
    {
        _selectedRole = role;

        if (registerButton != null)
        {
            registerButton.gameObject.SetActive(_selectedRole == UserRole.Student);
        }

        UpdateToggleVisuals(studentToggleBg, studentToggleText, _selectedRole == UserRole.Student);
        UpdateToggleVisuals(instructorToggleBg, instructorToggleText, _selectedRole == UserRole.Instructor);
        UpdateToggleVisuals(adminToggleBg, adminToggleText, _selectedRole == UserRole.Admin);

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

        SetLoading(true);
        HideError();

        Debug.Log($"Attempting login for: {email}");

        try
        {
            bool success = await FirebaseManager.Instance.LoginAsync(email, password);

            if (!success)
            {
                Debug.Log("Login failed, waiting for error callback");
                // Don't set loading false here - OnLoginFailed will do it
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Login exception: {e.Message}");
            SetLoading(false);
            ShowError("An unexpected error occurred. Please try again.");
        }
    }

    void OnLoginSuccess(UserProfile profile)
    {
        Debug.Log($"Login success for: {profile.email}, role: {profile.role}");
        SetLoading(false);
        RouteToScene(profile.role);
    }

    void OnLoginFailed(string error)
    {
        Debug.Log($"Login failed with error: {error}");
        SetLoading(false);

        // Clear any previous error and show new one
        if (errorMessage != null)
        {
            errorMessage.SetActive(false);
        }

        ShowError(error);

        // Shake the login button
        if (loginButton != null)
        {
            loginButton.GetComponent<RectTransform>()
                .DOShakePosition(0.4f, 8f, 20);
        }

        // Shake the password field on failure
        ShakeField(passwordField);

        // Clear password field for security (optional - you can comment this out)
        // passwordField.text = "";

        // Highlight the password field in red briefly
        if (passwordField != null)
        {
            Image passwordBg = passwordField.GetComponent<Image>();
            if (passwordBg != null)
            {
                Color originalColor = passwordBg.color;
                passwordBg.DOColor(new Color(1f, 0.8f, 0.8f), 0.1f).OnComplete(() =>
                {
                    passwordBg.DOColor(originalColor, 0.5f);
                });
            }
        }
    }

    void RouteToScene(string role)
    {
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
        if (loadingOverlay != null) loadingOverlay.SetActive(loading);
        if (loginButton != null) loginButton.interactable = !loading;
        if (registerButton != null && _selectedRole == UserRole.Student) registerButton.interactable = !loading;
    }

    void ShowError(string message)
    {
        if (errorMessage == null)
        {
            Debug.LogWarning($"Error message would show: {message}");
            return;
        }

        Debug.Log($"Showing error: {message}");

        // Make sure the error panel is active
        errorMessage.SetActive(true);

        // Set the error text
        if (errorText != null)
        {
            errorText.text = message;
        }

        // Animate the error
        RectTransform errorRect = errorMessage.GetComponent<RectTransform>();
        if (errorRect != null)
        {
            errorRect.localScale = Vector3.one;
            errorRect.DOPunchScale(new Vector3(0.05f, 0.05f, 0), 0.3f, 3, 0.5f);
        }

        // Auto-hide error after 4 seconds
        if (hideErrorCoroutine != null)
            StopCoroutine(hideErrorCoroutine);
        hideErrorCoroutine = StartCoroutine(HideErrorAfterDelay(4f));
    }

    private Coroutine hideErrorCoroutine;

    IEnumerator HideErrorAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (errorMessage != null)
            errorMessage.SetActive(false);
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
            RectTransform rect = field.GetComponent<RectTransform>();
            if (rect != null)
            {
                rect.DOShakePosition(0.3f, 6f, 15);
            }
        }
    }

    void OnDestroy()
    {
        FirebaseManager.OnLoginSuccess -= OnLoginSuccess;
        FirebaseManager.OnLoginFailed -= OnLoginFailed;

        if (hideErrorCoroutine != null)
            StopCoroutine(hideErrorCoroutine);
    }
}