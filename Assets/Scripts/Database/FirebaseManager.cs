using UnityEngine;
using Firebase;
using Firebase.Auth;
using Firebase.Firestore;
using Firebase.Extensions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

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
<<<<<<< HEAD
        // ✅ CHECK: Only run in Play mode, not in Editor edit mode
        if (!Application.isPlaying)
        {
            Debug.Log("FirebaseManager: Skipping initialization in Editor edit mode");
            return;
        }
        
=======
        // Singleton — persist across scenes
>>>>>>> 26ca292180f2e5632fdb78b15fe5f649ef097e93
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
<<<<<<< HEAD
        // ✅ CHECK: Only initialize if playing
        if (!Application.isPlaying)
        {
            Debug.Log("FirebaseManager: Not initializing in Editor edit mode");
            return;
        }
        
        Debug.Log("=== Firebase Initialization Started ===");
        
        FirebaseApp.CheckAndFixDependenciesAsync()
            .ContinueWithOnMainThread(task =>
            {
                if (!Application.isPlaying) return; // ✅ Exit if stopped playing
                
                Debug.Log($"CheckAndFixDependencies result: {task.Result}");
                
                if (task.Result == DependencyStatus.Available)
                {
                    Debug.Log("Firebase dependencies available, initializing...");
=======
        FirebaseApp.CheckAndFixDependenciesAsync()
            .ContinueWithOnMainThread(task =>
            {
                if (task.Result == DependencyStatus.Available)
                {
>>>>>>> 26ca292180f2e5632fdb78b15fe5f649ef097e93
                    _app = FirebaseApp.DefaultInstance;
                    _auth = FirebaseAuth.DefaultInstance;
                    _db = FirebaseFirestore.DefaultInstance;

                    IsInitialized = true;
<<<<<<< HEAD
                    Debug.Log("✅ Firebase initialized successfully!");
                    OnFirebaseReady?.Invoke();
=======
                    Debug.Log("Firebase initialized ✅");
                    OnFirebaseReady?.Invoke();

                    // Listen for auth state changes
>>>>>>> 26ca292180f2e5632fdb78b15fe5f649ef097e93
                    _auth.StateChanged += OnAuthStateChanged;
                }
                else
                {
<<<<<<< HEAD
                    Debug.LogError($"❌ Firebase failed: {task.Result}");
                }
            });
}
=======
                    Debug.LogError($"Firebase failed: {task.Result}");
                }
            });
    }
>>>>>>> 26ca292180f2e5632fdb78b15fe5f649ef097e93

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

    // Update your LoginAsync method in FirebaseManager.cs:

    public async Task<bool> LoginAsync(string email, string password)
    {
        try
        {
            Debug.Log($"LoginAsync called for: {email}");

            AuthResult result = await _auth.SignInWithEmailAndPasswordAsync(email, password);
            CurrentUser = result.User;

            await LoadUserProfile(CurrentUser.UserId);

            if (CurrentProfile != null)
            {
                await UpdateLastLogin(CurrentProfile.userId);
                await AddLoginAudit(true, email);
                OnLoginSuccess?.Invoke(CurrentProfile);
                return true;
            }

            OnLoginFailed?.Invoke("User profile data is missing.");
            return false;
        }
        catch (FirebaseException firebaseEx)
        {
            // ✅ THIS IS THE FIX: Handle Firebase-specific errors
            string friendlyError = GetFriendlyError(firebaseEx.Message);
            Debug.LogError($"Firebase Login Error: {firebaseEx.Message}");

            await AddLoginAudit(false, email, friendlyError);
            OnLoginFailed?.Invoke(friendlyError); // This triggers your UI error popup
            return false;
        }
        catch (Exception e)
        {
            Debug.LogError($"General Login Error: {e.Message}");
            OnLoginFailed?.Invoke("An unexpected error occurred.");
            return false;
        }
    }

    // Add this new method to parse Firebase errors properly:
    string ParseFirebaseError(FirebaseException ex)
    {
        string message = ex.Message.ToLower();
        int errorCode = ex.ErrorCode;

        Debug.Log($"Parsing error - Code: {errorCode}, Message: {message}");

        // Common Firebase Auth error codes
        // 17009 = Invalid email/password
        // 17010 = Wrong password  
        // 17011 = User not found
        // 17020 = Too many requests
        // 17026 = Weak password

        if (errorCode == 17009 || errorCode == 17010)
            return "❌ Incorrect email or password. Please try again.";

        if (errorCode == 17011 || message.Contains("user-not-found"))
            return "❌ No account found with this email address.";

        if (message.Contains("invalid-credential") || message.Contains("wrong-password"))
            return "❌ Incorrect email or password. Please try again.";

        if (message.Contains("email-already-in-use"))
            return "❌ This email is already registered.";

        if (message.Contains("weak-password"))
            return "❌ Password must be at least 6 characters.";

        if (message.Contains("network") || message.Contains("connection"))
            return "❌ Network error. Please check your internet connection.";

        if (errorCode == 17020 || message.Contains("too-many-requests"))
            return "❌ Too many failed attempts. Please try again later.";

        if (message.Contains("internal error"))
        {
            // This often happens with wrong password in some Firebase versions
            return "❌ Incorrect email or password. Please try again.";
        }

        return $"❌ Login failed: {ex.Message}";
    }

    // ── FRIENDLY ERROR METHOD (KEEP ONLY THIS ONE) ──
    string GetFriendlyError(string error)
    {
        string err = error.ToLower();

        if (err.Contains("wrong-password") || err.Contains("invalid-credential"))
            return "Incorrect email or password.";

        if (err.Contains("user-not-found"))
            return "No account found with this email.";

        if (err.Contains("network"))
            return "Network error. Check connection.";

        return "Login failed: " + error;
    }

    public async Task<bool> UpdateUserProfileAsync(string userId, Dictionary<string, object> updates)
    {
        try
        {
            await _db.Collection("users").Document(userId).UpdateAsync(updates);
            Debug.Log("Profile updated in Firestore ✅");
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError($"Profile update failed: {e.Message}");
            return false;
        }
    }

    // ── PASSWORD MANAGEMENT ──────────────────────────

    public async Task<bool> ChangePasswordAsync(string newPassword)
    {
        try
        {
            if (CurrentUser == null) return false;

            // Perform the update
            await CurrentUser.UpdatePasswordAsync(newPassword);
            Debug.Log("Password updated successfully ✅");

            // Add Audit Log
            await AddAuditLog(
                "PASSWORD_CHANGED",
                "User successfully updated their password.",
                CurrentUser.Email
            );

            return true;
        }
        catch (FirebaseException e)
        {
            if (e.ErrorCode == (int)AuthError.RequiresRecentLogin)
            {
                Debug.LogError("Password change requires recent login.");
                OnLoginFailed?.Invoke("For security, please log out and log in again to change your password.");
            }
            else
            {
                Debug.LogError($"Password update failed: {e.Message}");
            }
            return false;
        }
        catch (Exception e)
        {
            Debug.LogError($"Unexpected error: {e.Message}");
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

            // ✅ ADD AUDIT LOG FOR REGISTRATION
            await AddAuditLog(
                "USER_CREATED",
                $"New {role} account created: {displayName} ({email})",
                email
            );

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

    public async void Logout()
    {
        string userEmail = CurrentUser?.Email ?? "unknown";

        // Add audit log before signing out
        await AddLogoutAudit(userEmail);

        _auth.SignOut();
        CurrentUser = null;
        CurrentProfile = null;

        OnLogout?.Invoke();
        Debug.Log("Logged out successfully");
    }

    // ── USER PROFILE ──────────────────────────────────

    async Task LoadUserProfile(string userId)
    {
        // 1. FAST PATH: Assume userId IS the Document ID (standard for Admins/Instructors)
        DocumentSnapshot doc = await _db.Collection("users").Document(userId).GetSnapshotAsync();

        // 2. FLEXIBLE PATH: If not found, search the 'authUid' field (standard for Students)
        if (!doc.Exists)
        {
            QuerySnapshot snap = await _db.Collection("users")
                .WhereEqualTo("authUid", userId)
                .Limit(1)
                .GetSnapshotAsync();

            if (snap.Count > 0)
            {
                doc = snap.Documents.FirstOrDefault();
            }
        }

        // 3. Process the result if found
        if (doc != null && doc.Exists)
        {
            CurrentProfile = new UserProfile
            {
                userId = doc.ContainsField("userId") ? doc.GetValue<string>("userId") : doc.Id,
                email = doc.ContainsField("email") ? doc.GetValue<string>("email") : "",
                displayName = doc.ContainsField("displayName") ? doc.GetValue<string>("displayName") : "User",
                role = doc.ContainsField("role") ? doc.GetValue<string>("role") : "student",
                studentId = doc.ContainsField("studentId") ? doc.GetValue<string>("studentId") : "",
                institution = doc.ContainsField("institution") ? doc.GetValue<string>("institution") : "Mapua University",
                isActive = doc.ContainsField("isActive") ? doc.GetValue<bool>("isActive") : true
            };

            if (!CurrentProfile.isActive)
            {
                Logout();
                OnLoginFailed?.Invoke("Your account has been deactivated.");
            }
        }
        else
        {
            Debug.LogWarning("No profile found for userId: " + userId);
            OnLoginFailed?.Invoke("User profile data is missing.");
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

            // ✅ ADD AUDIT LOG ENTRY FOR SESSION COMPLETION
            await AddAuditLog(
                "SESSION_COMPLETED",
                $"{CurrentProfile?.displayName ?? CurrentUser.Email} completed {scenarioTitle} with score {result.finalScore}%",
                CurrentUser.Email
            );

            Debug.Log("Audit log created for session ✅");
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError($"Save failed: {e.Message}");
            return false;
        }
    }

    // ── AUDIT LOGGING ─────────────────────────────────

    /// <summary>
    /// Adds an audit log entry to Firestore
    /// </summary>
    public async Task AddAuditLog(string action, string details, string performedBy = null)
    {
        try
        {
            if (_db == null)
            {
                Debug.LogWarning("Firestore not initialized");
                return;
            }

            Dictionary<string, object> auditEntry = new Dictionary<string, object>
            {
                { "action", action },
                { "details", details },
                { "performedBy", performedBy ?? CurrentUser?.Email ?? "system" },
                { "timestamp", FieldValue.ServerTimestamp }
            };

            await _db.Collection("audit_logs")
                .AddAsync(auditEntry);

            Debug.Log($"Audit log added: {action}");
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to add audit log: {e.Message}");
        }
    }

    /// <summary>
    /// Adds a login audit entry
    /// </summary>
    public async Task AddLoginAudit(bool success, string email, string reason = null)
    {
        string action = success ? "LOGIN_SUCCESS" : "LOGIN_FAILED";
        string details = success
            ? $"User {email} logged in successfully"
            : $"Failed login attempt for {email}: {reason ?? "invalid credentials"}";

        await AddAuditLog(action, details, email);
    }

    /// <summary>
    /// Adds a logout audit entry
    /// </summary>
    public async Task AddLogoutAudit(string email)
    {
        await AddAuditLog("LOGOUT", $"User {email} logged out", email);
    }

    /// <summary>
    /// Adds a user action audit entry (for admin actions like deactivation, role changes, etc.)
    /// </summary>
    public async Task AddUserActionAudit(string action, string targetUserId, string targetEmail, string details)
    {
        await AddAuditLog(
            $"USER_{action}",
            $"{details} - Target: {targetEmail ?? targetUserId}",
            CurrentUser?.Email ?? "system"
        );
    }

    /// <summary>
    /// Adds a system configuration audit entry
    /// </summary>
    public async Task AddSystemConfigAudit(string configChange, string details)
    {
        await AddAuditLog(
            $"SYSTEM_CONFIG_{configChange}",
            details,
            CurrentUser?.Email ?? "system"
        );
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

    // ── STUDENT REGISTRATION VERIFICATION ──

    public async Task<string> VerifyStudentPin(string studentId, string pin)
    {
        try
        {
            QuerySnapshot snap = await _db.Collection("users")
                .WhereEqualTo("role", "student")
                .WhereEqualTo("studentId", studentId)
                .WhereEqualTo("pinCode", pin)
                .GetSnapshotAsync();

            // Use FirstOrDefault() instead of [0]
            var doc = snap.Documents.FirstOrDefault();

            if (doc == null) return null; // No match found
            return doc.Id; // Return the document ID
        }
        catch (Exception e)
        {
            Debug.LogError($"PIN Verification failed: {e.Message}");
            return null;
        }
    }

    public async Task<bool> CompleteStudentRegistration(string docId, string email, string password, string displayName)
    {
        try
        {
            // 1. Create Auth Account
            AuthResult result = await _auth.CreateUserWithEmailAndPasswordAsync(email, password);

            // 2. Update existing student doc
            await _db.Collection("users").Document(docId).UpdateAsync(new Dictionary<string, object> {
                { "email", email },
                { "displayName", displayName },
                { "authUid", result.User.UserId },
                { "pinCode", null }, // Clear PIN
                { "lastLoginAR", FieldValue.ServerTimestamp }
            });

            // ✅ ADD AUDIT LOG FOR STUDENT REGISTRATION COMPLETION
            await AddAuditLog(
                "STUDENT_REGISTERED",
                $"Student {displayName} ({email}) completed registration via PIN",
                email
            );

            return true;
        }
        catch (Exception e)
        {
            Debug.LogError("Registration Error: " + e.Message);
            return false;
        }
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