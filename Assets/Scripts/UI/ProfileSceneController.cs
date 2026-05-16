using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class ProfileSceneController : MonoBehaviour
{
    [Header("Profile Info")]
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI emailText;
    public TextMeshProUGUI roleText;
    public TextMeshProUGUI institutionText;
    public TextMeshProUGUI studentIdText;

    [Header("Buttons")]
    public Button backButton;
    public Button logoutButton;

    void Start()
    {
        // Populate from Firebase profile
        var profile = FirebaseManager.CurrentProfile;
        if (profile != null)
        {
            if (nameText) nameText.text = profile.displayName;
            if (emailText) emailText.text = profile.email;
            if (roleText) roleText.text = profile.role;
            if (institutionText) institutionText.text = profile.institution;
            if (studentIdText) studentIdText.text = profile.studentId;
        }

        if (backButton != null)
            backButton.onClick.AddListener(() =>
                SceneManager.LoadScene("DashboardScene"));

        if (logoutButton != null)
            logoutButton.onClick.AddListener(() =>
            {
                FirebaseManager.Instance?.Logout();
                SceneManager.LoadScene("LoginScene");
            });
    }
}