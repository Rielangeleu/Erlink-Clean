using UnityEngine;
using Firebase;
using Firebase.Auth;
using Firebase.Firestore;
using Firebase.Extensions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

/// <summary>
/// Central Firebase manager for ERLink AR.
/// Handles Auth, Firestore, and user role management.
/// Singleton — persists across all scenes.
/// </summary>
public class FirebaseManager : MonoBehaviour
{
    public static FirebaseManager Instance;

    // Firebase references
    private FirebaseApp _app;
    private FirebaseAuth _auth;
    private FirebaseFirestore _db;

    // Current user state
    public static FirebaseUser CurrentUser { get; private set; }
    public static UserProfile CurrentProfile { get; private set; }
    public static bool IsInitialized { get; private set; }

    // Events
    public static event Action OnFirebaseReady;
    public static event Action<UserProfile> OnLoginSuccess;
    public static event Action<string> OnLoginFailed;
    public static event Action OnLogout;

    void Awake()
    {
        // Singleton — persist across scenes
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        InitializeFirebase();
    }

    void InitializeFirebase()
    {
        FirebaseApp.CheckAndFixDependenciesAsync()
            .ContinueWithOnMainThread(task =>
            {
                if (task.Result ==
                    DependencyStatus.Available)
                {
                    _app = FirebaseApp.DefaultInstance;
                    _auth = FirebaseAuth.DefaultInstance;
                    _db = FirebaseFirestore.DefaultInstance;

                    IsInitialized = true;
                    Debug.Log("Firebase initialized ✅");
                    OnFirebaseReady?.Invoke();

                    // Listen for auth state changes
                    _auth.StateChanged += OnAuthStateChanged;
                }
                else
                {
                    Debug.LogError(
                        $"Firebase failed: {task.Result}");
                }
            });
    }

    // Add to FirebaseManager.cs — call this in InitializeFirebase() after ready
    async Task CheckMaintenanceMode()
    {
        DocumentSnapshot doc = await _db
            .Collection("system_config")
            .Document("app_settings")
            .GetSnapshotAsync();

        if (doc.Exists)
        {
            bool maintenance = doc.ContainsField("maintenanceMode") &&
                doc.GetValue<bool>("maintenanceMode");

            if (maintenance)
            {
                string msg = doc.ContainsField("maintenanceMessage")
                    ? doc.GetValue<string>("maintenanceMessage")
                    : "System under maintenance.";

                // Show maintenance screen
                UnityEngine.SceneManagement.SceneManager
                    .LoadScene("MaintenanceScene");
            }
        }
    }

    void OnAuthStateChanged(object sender, EventArgs e)
    {
        CurrentUser = _auth.CurrentUser;
    }

    // ── AUTHENTICATION ────────────────────────────────

    public async Task<bool> LoginAsync(
        string email, string password)
    {
        try
        {
            AuthResult result = await _auth
                .SignInWithEmailAndPasswordAsync(
                    email, password);

            CurrentUser = result.User;
            Debug.Log($"Logged in: {CurrentUser.Email}");

            // Load user profile from Firestore
            await LoadUserProfile(CurrentUser.UserId);

            // Update last login
            await UpdateLastLogin(CurrentUser.UserId);

            OnLoginSuccess?.Invoke(CurrentProfile);
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError($"Login failed: {e.Message}");
            OnLoginFailed?.Invoke(GetFriendlyError(e.Message));
            return false;
        }
    }

    public async Task<bool> RegisterAsync(
        string email,
        string password,
        string displayName,
        string role = "student",
        string studentId = "")
    {
        try
        {
            AuthResult result = await _auth
                .CreateUserWithEmailAndPasswordAsync(
                    email, password);

            CurrentUser = result.User;

            // Create user profile in Firestore
            UserProfile profile = new UserProfile
            {
                userId = CurrentUser.UserId,
                email = email,
                displayName = displayName,
                role = role,
                studentId = studentId,
                institution = "Mapua University",
                isActive = true,
                createdAt = DateTime.UtcNow
            };

            await SaveUserProfile(profile);
            CurrentProfile = profile;

            Debug.Log($"Registered: {email} as {role}");
            OnLoginSuccess?.Invoke(CurrentProfile);
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError($"Register failed: {e.Message}");
            OnLoginFailed?.Invoke(GetFriendlyError(e.Message));
            return false;
        }
    }

    public void Logout()
    {
        _auth.SignOut();
        CurrentUser = null;
        CurrentProfile = null;
        OnLogout?.Invoke();
        Debug.Log("Logged out");
    }

    // ── USER PROFILE ──────────────────────────────────

    async Task LoadUserProfile(string userId)
    {
        DocumentSnapshot doc = await _db
            .Collection("users")
            .Document(userId)
            .GetSnapshotAsync();

        if (doc.Exists)
        {
            CurrentProfile = new UserProfile
            {
                userId = userId,
                email = doc.GetValue<string>("email"),
                displayName = doc.GetValue<string>("displayName"),
                role = doc.GetValue<string>("role"),
                studentId = doc.ContainsField("studentId")
                    ? doc.GetValue<string>("studentId") : "",
                institution = doc.ContainsField("institution")
                    ? doc.GetValue<string>("institution") : "",
                isActive = doc.GetValue<bool>("isActive")
            };

            // Block inactive users
            if (!CurrentProfile.isActive)
            {
                Logout();
                OnLoginFailed?.Invoke(
                    "Your account has been deactivated. " +
                    "Please contact your administrator.");
            }
        }
        else
        {
            Debug.LogWarning("No profile found for user");
        }
    }

    async Task SaveUserProfile(UserProfile profile)
    {
        Dictionary<string, object> data =
            new Dictionary<string, object>
        {
            { "email",       profile.email },
            { "displayName", profile.displayName },
            { "role",        profile.role },
            { "studentId",   profile.studentId },
            { "institution", profile.institution },
            { "isActive",    profile.isActive },
            { "createdAt",
                FieldValue.ServerTimestamp },
            { "lastLogin",
                FieldValue.ServerTimestamp }
        };

        await _db.Collection("users")
            .Document(profile.userId)
            .SetAsync(data);
    }

    async Task UpdateLastLogin(string userId)
    {
        await _db.Collection("users")
            .Document(userId)
            .UpdateAsync("lastLogin",
                FieldValue.ServerTimestamp);
    }

    // ── SESSION / SCORE SAVING ────────────────────────

    public async Task<bool> SaveSessionAsync(
        ScoringResult result,
        string scenarioId,
        string scenarioTitle,
        string difficulty)
    {
        if (CurrentUser == null)
        {
            Debug.LogWarning("No user logged in");
            return false;
        }

        try
        {
            Dictionary<string, object> session =
                new Dictionary<string, object>
            {
                { "userId",           CurrentUser.UserId },
                { "userEmail",        CurrentUser.Email },
                { "displayName",
                    CurrentProfile?.displayName ?? "" },
                { "scenarioId",       scenarioId },
                { "scenarioTitle",    scenarioTitle },
                { "difficulty",       difficulty },
                { "finalScore",       result.finalScore },
                { "accuracyScore",    result.accuracyScore },
                { "speedScore",       result.speedScore },
                { "confidenceScore",  result.confidenceScore },
                { "timeTaken",        result.timeTaken },
                { "selectedCategory",
                    result.selectedCategory.ToString() },
                { "correctCategory",
                    result.correctCategory.ToString() },
                { "isCorrect",        result.isCorrect },
                { "ehrCorrect",       result.ehrCorrect },
                { "interpretation",   result.interpretation },
                { "penaltyApplied",
                    result.timeTaken >= float.MaxValue },
                { "completedAt",
                    FieldValue.ServerTimestamp }
            };

            await _db.Collection("sessions")
                .AddAsync(session);

            Debug.Log("Session saved to Firestore ✅");
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError($"Save failed: {e.Message}");
            return false;
        }
    }

    // ── SYSTEM CONFIG ─────────────────────────────────

    public async Task<SystemConfig> LoadSystemConfig()
    {
        try
        {
            DocumentSnapshot doc = await _db
                .Collection("system_config")
                .Document("app_settings")
                .GetSnapshotAsync();

            if (doc.Exists)
            {
                return new SystemConfig
                {
                    appName = doc.ContainsField("appName")
                        ? doc.GetValue<string>("appName")
                        : "ERLink AR",
                    primaryColor = doc.ContainsField("primaryColor")
                        ? doc.GetValue<string>("primaryColor")
                        : "#2563EB",
                    theme = doc.ContainsField("theme")
                        ? doc.GetValue<string>("theme")
                        : "default"
                };
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Config load failed: {e.Message}");
        }

        return new SystemConfig(); // defaults
    }

    // ── ERROR HANDLING ────────────────────────────────

    string GetFriendlyError(string error)
    {
        if (error.Contains("wrong-password") ||
            error.Contains("invalid-credential"))
            return "Incorrect email or password.";
        if (error.Contains("user-not-found"))
            return "No account found with this email.";
        if (error.Contains("email-already-in-use"))
            return "This email is already registered.";
        if (error.Contains("weak-password"))
            return "Password must be at least 6 characters.";
        if (error.Contains("network"))
            return "Network error. Check your connection.";
        return "An error occurred. Please try again.";
    }

    void OnDestroy()
    {
        if (_auth != null)
            _auth.StateChanged -= OnAuthStateChanged;
    }
}

// ── Data Models ───────────────────────────────────────

[System.Serializable]
public class UserProfile
{
    public string userId;
    public string email;
    public string displayName;
    public string role; // "student" | "instructor" | "it_admin"
    public string studentId;
    public string institution;
    public bool isActive;
    public DateTime createdAt;
}

[System.Serializable]
public class SystemConfig
{
    public string appName = "ERLink AR";
    public string primaryColor = "#2563EB";
    public string logoUrl = "";
    public string theme = "default";
}