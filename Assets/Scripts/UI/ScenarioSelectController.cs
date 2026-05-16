using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

/// <summary>
/// Scenario selection screen.
/// Sets ScenarioSelector before loading MainAR.
/// </summary>
public class ScenarioSelectController : MonoBehaviour
{
    [Header("Scenario References")]
    public ScenarioData easyScenario;
    public ScenarioData mediumPatientA;
    public ScenarioData mediumPatientB;

    [Header("Buttons")]
    public Button startEasyButton;
    public Button startMediumButton;
    public Button backButton;

    [Header("Hard Card")]
    public GameObject hardCardLockOverlay;

    void Start()
    {
        if (startEasyButton != null)
            startEasyButton.onClick.AddListener(StartEasy);

        if (startMediumButton != null)
            startMediumButton.onClick.AddListener(StartMedium);

        if (backButton != null)
            backButton.onClick.AddListener(() =>
                SceneManager.LoadScene("DashboardScene"));

        // Hard scenario locked
        if (hardCardLockOverlay != null)
            hardCardLockOverlay.SetActive(true);
    }

    void StartEasy()
    {
        ScenarioSelector.SelectEasy(easyScenario);
        SceneManager.LoadScene("MainAR");
    }

    void StartMedium()
    {
        ScenarioSelector.SelectMedium(
            mediumPatientA, mediumPatientB);
        SceneManager.LoadScene("MainAR");
    }
}