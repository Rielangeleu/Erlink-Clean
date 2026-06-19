using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections.Generic;
using Firebase.Firestore;
using System;

public class ProfileSceneController : MonoBehaviour
{
    [Header("Panels")]
    public GameObject viewPanel;
    public GameObject editProfilePanel;
    public GameObject changePasswordPanel;

    [Header("Profile Info (View Mode)")]
    public TextMeshProUGUI nameText, emailText, roleText, institutionText, studentIdText;

    [Header("Edit Profile Inputs")]
    public TMP_InputField editNameField;
    public Button saveEditButton, cancelEditButton;

    [Header("Change Password Inputs")]
    public TMP_InputField currentPassField;
    public TMP_InputField newPassField;
    public TMP_InputField reenterPassField;
    public Button savePassButton, cancelPassButton;

    [Header("Error Handling")]
    public GameObject errorHeader;
    public TextMeshProUGUI errorText;

    [Header("Main View Buttons")]
    public Button editProfileButton;     // Drag your "Edit" button here
    public Button changePasswordButton;  // Drag your "Change Password" button here

    [Header("Navigation Buttons")]
    public Button backButton;            // Drag your Back button here
    public Button logoutButton;          // Drag your Logout button here

    void Start()
    {
        RefreshUI();
        TogglePanels("view");

        // Wire Main View Buttons
        if (editProfileButton != null)
            editProfileButton.onClick.AddListener(() => TogglePanels("edit"));

        if (changePasswordButton != null)
            changePasswordButton.onClick.AddListener(() => TogglePanels("password"));

        // Wire Action Buttons
        if (saveEditButton != null)
            saveEditButton.onClick.AddListener(SaveProfile);

        if (savePassButton != null)
            savePassButton.onClick.AddListener(SavePassword);

        // Navigation / Cancel logic
        if (cancelEditButton != null)
            cancelEditButton.onClick.AddListener(() => TogglePanels("view"));

        if (cancelPassButton != null)
            cancelPassButton.onClick.AddListener(() => TogglePanels("view"));

        // ✅ ADD BACK BUTTON HANDLER
        if (backButton != null)
            backButton.onClick.AddListener(GoBack);

        // ✅ ADD LOGOUT BUTTON HANDLER
        if (logoutButton != null)
            logoutButton.onClick.AddListener(Logout);
    }

    void RefreshUI()
    {
        var p = FirebaseManager.CurrentProfile;
        if (p == null) return;

        if (nameText != null) nameText.text = p.displayName;
        if (emailText != null) emailText.text = p.email;
        if (roleText != null) roleText.text = p.role.ToLower().Contains("student") ? "Student" : p.role;
        
        if (studentIdText != null) studentIdText.text = p.studentId;

        // Pre-fill edit field with current name
        if (editNameField != null) editNameField.text = p.displayName;
    }

    public void TogglePanels(string mode)
    {
        if (viewPanel != null) viewPanel.SetActive(mode == "view");
        if (editProfilePanel != null) editProfilePanel.SetActive(mode == "edit");
        if (changePasswordPanel != null) changePasswordPanel.SetActive(mode == "password");
        if (errorHeader != null) errorHeader.SetActive(false);

        // Clear password fields when switching to password panel
        if (mode == "password")
        {
            if (currentPassField != null) currentPassField.text = "";
            if (newPassField != null) newPassField.text = "";
            if (reenterPassField != null) reenterPassField.text = "";
        }
    }

    public async void SaveProfile()
    {
        if (string.IsNullOrEmpty(editNameField.text))
        {
            ShowError("Name cannot be empty.");
            return;
        }

        var profile = FirebaseManager.CurrentProfile;

        if (editNameField.text != profile.displayName)
        {
            var updates = new Dictionary<string, object> { { "displayName", editNameField.text } };
            bool success = await FirebaseManager.Instance.UpdateUserProfileAsync(profile.userId, updates);

            if (success)
            {
                profile.displayName = editNameField.text;
                RefreshUI();
                TogglePanels("view");
            }
            else
            {
                ShowError("Failed to update profile.");
            }
        }
        else
        {
            TogglePanels("view"); // No changes
        }
    }

    public async void SavePassword()
    {
        if (string.IsNullOrEmpty(currentPassField.text) || string.IsNullOrEmpty(newPassField.text))
        {
            ShowError("Please fill all password fields.");
            return;
        }

        if (newPassField.text != reenterPassField.text)
        {
            ShowError("New passwords do not match.");
            return;
        }

        if (newPassField.text.Length < 6)
        {
            ShowError("Password must be at least 6 characters.");
            return;
        }

        bool success = await FirebaseManager.Instance.ChangePasswordAsync(newPassField.text);

        if (success)
        {
            TogglePanels("view");
            // Clear fields for security
            if (currentPassField != null) currentPassField.text = "";
            if (newPassField != null) newPassField.text = "";
            if (reenterPassField != null) reenterPassField.text = "";

            // Show success message (optional)
            ShowSuccess("Password changed successfully!");
        }
        else
        {
            ShowError("Password change failed. Please check your current password or login again.");
        }
    }

    // ✅ BACK BUTTON FUNCTIONALITY
    public void GoBack()
    {
        Debug.Log("Go Back button pressed");

        // Determine which scene to go back to based on user role
        if (FirebaseManager.CurrentProfile != null)
        {
            string role = FirebaseManager.CurrentProfile.role.ToLower();

            switch (role)
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
        else
        {
            // Fallback to dashboard
            SceneManager.LoadScene("DashboardScene");
        }
    }

    // ✅ LOGOUT BUTTON FUNCTIONALITY
    public void Logout()
    {
        Debug.Log("Logout button pressed");

        // Show confirmation dialog (optional)
        // You can implement a simple popup here if needed

        // Call FirebaseManager logout
        FirebaseManager.Instance.Logout();

        // Load login scene
        SceneManager.LoadScene("LoginScene");
    }

    void ShowError(string msg)
    {
        if (errorHeader != null) errorHeader.SetActive(true);
        if (errorText != null) errorText.text = msg;

        // Auto-hide error after 3 seconds
        Invoke(nameof(HideError), 3f);
    }

    void ShowSuccess(string msg)
    {
        if (errorHeader != null)
        {
            errorHeader.SetActive(true);
            // Change color to green for success (optional)
            var bgImage = errorHeader.GetComponent<Image>();
            if (bgImage != null)
            {
                bgImage.color = Color.green;
            }
        }
        if (errorText != null) errorText.text = msg;

        // Auto-hide success after 2 seconds
        Invoke(nameof(HideError), 2f);
    }

    void HideError()
    {
        if (errorHeader != null) errorHeader.SetActive(false);

        // Reset color back to red (if you changed it)
        var bgImage = errorHeader?.GetComponent<Image>();
        if (bgImage != null)
        {
            bgImage.color = new Color(0.9f, 0.2f, 0.2f, 0.9f); // Red color
        }
    }
}