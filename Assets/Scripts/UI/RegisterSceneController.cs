using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using DG.Tweening; // Ensure you have DOTween imported

public class RegisterSceneController : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject verificationPanel;
    public GameObject detailsPanel;

    [Header("Verification Inputs")]
    public TMP_InputField idField;
    public TMP_InputField pinField;
    public Button verifyButton;

    [Header("Registration Inputs")]
    public TMP_InputField emailField;
    public TMP_InputField passwordField;
    public TMP_InputField confirmPasswordField;
    public TMP_InputField nameField;
    public Button registerButton;

    [Header("Feedback")]
    public GameObject errorMessage;
    public TextMeshProUGUI errorText;
    public GameObject loadingOverlay;

    private string _verifiedDocId;

    void Start()
    {
        verificationPanel.SetActive(true);
        detailsPanel.SetActive(false);
        HideError();
        SetLoading(false);

        verifyButton.onClick.AddListener(OnVerifyClicked);
        registerButton.onClick.AddListener(OnRegisterClicked);
    }

    public async void OnVerifyClicked()
    {
        if (string.IsNullOrEmpty(idField.text) || string.IsNullOrEmpty(pinField.text))
        {
            ShowError("Please fill in Student ID and PIN.");
            return;
        }

        SetLoading(true);
        string docId = await FirebaseManager.Instance.VerifyStudentPin(idField.text, pinField.text);
        SetLoading(false);

        if (!string.IsNullOrEmpty(docId))
        {
            _verifiedDocId = docId;
            HideError();
            verificationPanel.SetActive(false);
            detailsPanel.SetActive(true);
        }
        else
        {
            ShowError("Invalid ID or PIN. Please contact your instructor.");
            ShakeField(idField);
            ShakeField(pinField);
        }
    }

    public async void OnRegisterClicked()
    {
        if (passwordField.text != confirmPasswordField.text)
        {
            ShowError("Passwords do not match!");
            ShakeField(passwordField);
            ShakeField(confirmPasswordField);
            return;
        }

        if (passwordField.text.Length < 6)
        {
            ShowError("Password must be at least 6 characters.");
            ShakeField(passwordField);
            return;
        }

        SetLoading(true);
        bool success = await FirebaseManager.Instance.CompleteStudentRegistration(
            _verifiedDocId, emailField.text, passwordField.text, nameField.text);
        SetLoading(false);

        if (success)
        {
            SceneManager.LoadScene("LoginScene");
        }
        else
        {
            ShowError("Registration failed. Try again later.");
        }
    }

    // Helper methods mirroring LoginSceneController
    void SetLoading(bool loading)
    {
        if (loadingOverlay != null) loadingOverlay.SetActive(loading);
        verifyButton.interactable = !loading;
        registerButton.interactable = !loading;
    }

    void ShowError(string message)
    {
        if (errorMessage != null)
        {
            errorMessage.SetActive(true);
            if (errorText != null) errorText.text = message;
            errorMessage.transform.DOPunchScale(new Vector3(0.02f, 0.02f, 0), 0.3f);
        }
    }

    void HideError()
    {
        if (errorMessage != null) errorMessage.SetActive(false);
    }

    void ShakeField(TMP_InputField field)
    {
        field.GetComponent<RectTransform>().DOShakePosition(0.3f, 6f, 15);
    }
}