using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Firebase.Firestore;
using Firebase.Extensions;
using System;
using System.Collections;
using System.Threading.Tasks;

/// <summary>
/// Manages app theme from Firebase configuration
/// </summary>
public class AppThemeManager : MonoBehaviour
{
    public static AppThemeManager Instance;

    [Header("UI References")]
    public Image[] headerBackgrounds;
    public TextMeshProUGUI[] headerTexts;
    public Image[] buttonBackgrounds;
    public TextMeshProUGUI[] buttonTexts;
    public Image[] panelBackgrounds;
    public Image[] cardBackgrounds;

    [Header("Specific Elements")]
    public Image topBarBackground;
    public TextMeshProUGUI scenarioTitleText;
    public Button[] triageButtons;
    public TextMeshProUGUI[] triageButtonTexts;
    public Image dropZoneBackground;
    public TextMeshProUGUI dropZoneText;

    [Header("Logo")]
    public Image appLogo;
    public RawImage appLogoRaw;

    [Header("Loading")]
    public GameObject loadingOverlay;
    public TextMeshProUGUI loadingText;

    private AppThemeConfig _currentConfig;
    private bool _isLoading = false;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        // Subscribe to Firebase ready event
        FirebaseManager.OnFirebaseReady += OnFirebaseReady;

        // Check if Firebase is already ready
        if (FirebaseManager.IsInitialized)
        {
            LoadAndApplyTheme();
        }
    }

    void OnFirebaseReady()
    {
        LoadAndApplyTheme();
    }

    public async void LoadAndApplyTheme()
    {
        if (_isLoading) return;
        _isLoading = true;

        if (loadingOverlay != null)
            loadingOverlay.SetActive(true);
        if (loadingText != null)
            loadingText.text = "Loading theme...";

        try
        {
            _currentConfig = await LoadSystemConfig();
            if (_currentConfig != null)
            {
                ApplyTheme();
                Debug.Log($"Theme applied successfully! Primary: {_currentConfig.primaryColor}");
            }
            else
            {
                Debug.Log("No theme config found, using defaults");
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to load theme: {e.Message}");
        }
        finally
        {
            _isLoading = false;
            if (loadingOverlay != null)
                loadingOverlay.SetActive(false);
        }
    }

    private async Task<AppThemeConfig> LoadSystemConfig()
    {
        try
        {
            var db = FirebaseFirestore.DefaultInstance;
            DocumentSnapshot doc = await db.Collection("system_config").Document("app_settings").GetSnapshotAsync();

            if (doc.Exists)
            {
                var config = new AppThemeConfig();

                if (doc.ContainsField("primaryColor"))
                    config.primaryColor = doc.GetValue<string>("primaryColor");
                if (doc.ContainsField("sidebarColor"))
                    config.sidebarColor = doc.GetValue<string>("sidebarColor");
                if (doc.ContainsField("appName"))
                    config.appName = doc.GetValue<string>("appName");
                if (doc.ContainsField("logoUrl"))
                    config.logoUrl = doc.GetValue<string>("logoUrl");
                if (doc.ContainsField("theme"))
                    config.theme = doc.GetValue<string>("theme");

                Debug.Log($"Loaded theme: Primary={config.primaryColor}, Sidebar={config.sidebarColor}");
                return config;
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Error loading system config: {e.Message}");
        }

        return null;
    }

    private void ApplyTheme()
    {
        if (_currentConfig == null) return;

        // Parse colors (use defaults if null)
        Color primaryColor = HexToColor(_currentConfig.primaryColor ?? "#2563EB");
        Color sidebarColor = HexToColor(_currentConfig.sidebarColor ?? "#1F2937");

        // Apply to header backgrounds
        foreach (var img in headerBackgrounds)
        {
            if (img != null) img.color = primaryColor;
        }

        // Apply to header texts
        foreach (var text in headerTexts)
        {
            if (text != null) text.color = GetContrastColor(primaryColor);
        }

        // Apply to buttons
        foreach (var btn in buttonBackgrounds)
        {
            if (btn != null) btn.color = primaryColor;
        }

        foreach (var text in buttonTexts)
        {
            if (text != null) text.color = GetContrastColor(primaryColor);
        }

        // Apply to top bar
        if (topBarBackground != null)
            topBarBackground.color = primaryColor;

        // Apply to scenario title
        if (scenarioTitleText != null)
            scenarioTitleText.color = primaryColor;

        // Apply to triage buttons
        ApplyTriageButtonColors(primaryColor);

        // Apply to drop zone
        if (dropZoneBackground != null)
            dropZoneBackground.color = primaryColor;
        if (dropZoneText != null)
            dropZoneText.color = GetContrastColor(primaryColor);

        // Apply to panels and cards
        foreach (var panel in panelBackgrounds)
        {
            if (panel != null) panel.color = sidebarColor;
        }

        foreach (var card in cardBackgrounds)
        {
            if (card != null) card.color = sidebarColor;
        }

        // Load and apply logo if URL exists
        if (!string.IsNullOrEmpty(_currentConfig.logoUrl))
        {
            StartCoroutine(LoadLogoTexture(_currentConfig.logoUrl));
        }

        Debug.Log("Theme applied to all UI elements");
    }

    private void ApplyTriageButtonColors(Color baseColor)
    {
        for (int i = 0; i < triageButtons.Length && i < triageButtonTexts.Length; i++)
        {
            if (triageButtons[i] != null)
            {
                var outline = triageButtons[i].GetComponent<Outline>();
                if (outline == null)
                    outline = triageButtons[i].gameObject.AddComponent<Outline>();
                outline.effectColor = baseColor;
                outline.effectDistance = new Vector2(2, 2);
            }
        }
    }

    private Color HexToColor(string hex)
    {
        if (string.IsNullOrEmpty(hex))
            return Color.white;

        // Remove # if present
        hex = hex.Replace("#", "");

        // Handle 3-digit hex (e.g., "FFF")
        if (hex.Length == 3)
        {
            hex = $"{hex[0]}{hex[0]}{hex[1]}{hex[1]}{hex[2]}{hex[2]}";
        }

        byte r = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
        byte g = byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
        byte b = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);

        return new Color(r / 255f, g / 255f, b / 255f);
    }

    private Color GetContrastColor(Color backgroundColor)
    {
        // Calculate luminance (perceived brightness)
        float luminance = 0.299f * backgroundColor.r + 0.587f * backgroundColor.g + 0.114f * backgroundColor.b;

        // Return white for dark backgrounds, black for light backgrounds
        return luminance > 0.5f ? Color.black : Color.white;
    }

    private IEnumerator LoadLogoTexture(string url)
    {
        using (var www = UnityEngine.Networking.UnityWebRequestTexture.GetTexture(url))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityEngine.Networking.UnityWebRequest.Result.Success)
            {
                Texture2D texture = UnityEngine.Networking.DownloadHandlerTexture.GetContent(www);
                if (appLogo != null)
                    appLogo.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                if (appLogoRaw != null)
                    appLogoRaw.texture = texture;

                Debug.Log("Logo loaded successfully!");
            }
            else
            {
                Debug.LogError($"Failed to load logo: {www.error}");
            }
        }
    }

    public void RefreshTheme()
    {
        LoadAndApplyTheme();
    }

    void OnDestroy()
    {
        FirebaseManager.OnFirebaseReady -= OnFirebaseReady;
    }
}

// Self-contained theme config class
[System.Serializable]
public class AppThemeConfig
{
    public string appName = "ERLink AR";
    public string primaryColor = "#2563EB";
    public string sidebarColor = "#1F2937";
    public string logoUrl = "";
    public string theme = "default";
}