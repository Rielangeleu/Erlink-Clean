using UnityEngine;
using TMPro;
using DG.Tweening;

/// <summary>
/// Countdown timer system.
/// Thesis Rule 2: Time pressure per scenario.
/// Easy = 3min, Medium = 5min, Hard = 8min
/// Delayed Care penalty if timer expires.
/// </summary>
public class TimerSystem : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI timerText;

    [Header("Dependencies")]
    public UIManager uiManager;
    public ScoringSystem scoringSystem;
    public ScenarioManager scenarioManager;

    [Header("Settings")]
    public Color normalColor = Color.white;
    public Color warningColor = new Color(1f, 0.8f, 0f);
    public Color criticalColor = new Color(1f, 0.2f, 0.2f);

    private float _timeRemaining;
    private float _initialTime;         // FIX: must be set in StartTimer
    private bool _isRunning = false;
    private bool _penaltyApplied = false;
    private Tween _pulseTween;

    public void StartTimer(float seconds)
    {
        _timeRemaining = seconds;
        _initialTime = seconds;         // FIX: capture initial value here
        _penaltyApplied = false;
        _isRunning = true;
        UpdateTimerDisplay();
        Debug.Log($"Timer started: {seconds}s");
    }

    public void StopTimer()
    {
        _isRunning = false;
        _pulseTween?.Kill();
    }

    public void PauseTimer() => _isRunning = false;
    public void ResumeTimer() => _isRunning = true;

    public float GetInitialTime() => _initialTime;

    public float GetTimeElapsed()
    {
        if (_initialTime <= 0f) return 0f;
        return _initialTime - _timeRemaining;
    }

    void Update()
    {
        if (!_isRunning) return;

        _timeRemaining -= Time.deltaTime;
        _timeRemaining = Mathf.Max(0, _timeRemaining);

        UpdateTimerDisplay();
        CheckWarnings();

        if (_timeRemaining <= 0 && !_penaltyApplied)
        {
            OnTimerExpired();
        }
    }

    void UpdateTimerDisplay()
    {
        int minutes = Mathf.FloorToInt(_timeRemaining / 60f);
        int seconds = Mathf.FloorToInt(_timeRemaining % 60f);
        string display = string.Format("{0:00}:{1:00}", minutes, seconds);

        if (timerText != null)
            timerText.text = display;

        if (uiManager != null)
            uiManager.UpdateTimerDisplay(_timeRemaining);
    }

    void CheckWarnings()
    {
        if (timerText == null) return;

        if (_timeRemaining <= 30f)
        {
            timerText.color = criticalColor;
            if (_pulseTween == null || !_pulseTween.IsActive())
            {
                _pulseTween = timerText
                    .DOFade(0.3f, 0.4f)
                    .SetLoops(-1, LoopType.Yoyo)
                    .SetEase(Ease.InOutSine);
            }
        }
        else if (_timeRemaining <= 60f)
        {
            timerText.color = warningColor;
            _pulseTween?.Kill();
            timerText.alpha = 1f;
        }
        else
        {
            timerText.color = normalColor;
        }
    }

    void OnTimerExpired()
    {
        _penaltyApplied = true;
        _isRunning = false;

        Debug.Log("Timer expired! Delayed Care penalty applied.");

        if (scoringSystem != null)
            scoringSystem.ApplyTimerPenalty();

        if (timerText != null)
        {
            timerText.text = "00:00";
            timerText.color = criticalColor;
        }

        Invoke(nameof(ForceAdvance), 2f);
    }

    void ForceAdvance()
    {
        if (scenarioManager != null)
            scenarioManager.LoadNextPatient();
    }

    public float GetTimeRemaining() => _timeRemaining;
}