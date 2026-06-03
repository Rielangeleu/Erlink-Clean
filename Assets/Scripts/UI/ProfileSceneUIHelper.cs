using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Manages the enhanced data presentation layers for the Profile scene display.
/// Formats text data to guarantee clean layout hierarchy scales.
/// </summary>
public class ProfileSceneUIHelper : MonoBehaviour
{
    [Header("Avatar Components")]
    public Image avatarBackground;
    public TextMeshProUGUI avatarInitialsText;

    [Header("Personal Details")]
    public TextMeshProUGUI fullNameText;
    public TextMeshProUGUI emailAddressText;
    public TextMeshProUGUI studentIdText;

    [Header("Institutional Matrix")]
    public TextMeshProUGUI universityNameText;
    public TextMeshProUGUI roleBadgeText;

    [Header("Visual Styling Options")]
    public Color avatarBrandColor = new Color(0.145f, 0.337f, 0.922f); // #2563EB Vibrant Blue
    public Color darkSlateTextColor = new Color(0.059f, 0.090f, 0.165f); // #0F172A

    void Start()
    {
        // Fallback placeholder data load (Replace this block with your true database/Firebase payload values!)
        LoadUserProfileDetails(
            "Juan Dela Cruz",
            "juandelacruz@mymail.mapua.edu.ph",
            "2022104859",
            "Mapúa University",
            "Student"
        );
    }

    public void LoadUserProfileDetails(string fullName, string email, string studentId, string university, string role)
    {
        // ── 1. GENERATE DYNAMIC AVATAR INITIALS ──
        if (avatarInitialsText != null && !string.IsNullOrEmpty(fullName))
        {
            string[] nameParts = fullName.Split(' ');
            string initials = nameParts[0][0].ToString().ToUpper();
            if (nameParts.Length > 1) initials += nameParts[nameParts.Length - 1][0].ToString().ToUpper();

            avatarInitialsText.text = initials;
        }
        if (avatarBackground != null) avatarBackground.color = avatarBrandColor;

        // ── 2. APPLY CONTRAST TEXT SCALARS ──
        if (fullNameText != null)
        {
            fullNameText.text = fullName;
            fullNameText.color = darkSlateTextColor;
        }

        if (emailAddressText != null) emailAddressText.text = email;

        if (studentIdText != null)
        {
            // Format ID with a clean tag header layout look
            studentIdText.text = $"<color=#64748B>ID:</color> {studentId}";
        }

        // ── 3. ACADEMIC DETAILS MATRIX ──
        if (universityNameText != null)
        {
            universityNameText.text = university;
            universityNameText.color = darkSlateTextColor;
        }

        if (roleBadgeText != null)
        {
            roleBadgeText.text = role.ToUpper();
        }
    }
}