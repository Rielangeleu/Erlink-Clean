using UnityEngine;
using UnityEngine.XR.ARFoundation;
using System.Collections;

/// <summary>
/// Forces AR Simulation to reset between scenes
/// </summary>
public class ARSimulationManager : MonoBehaviour
{
    public static ARSimulationManager Instance;
    
    private GameObject _simulationEnvironment;
    
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
    
    public void ResetSimulationEnvironment()
    {
        StartCoroutine(ResetSimulationCoroutine());
    }
    
    IEnumerator ResetSimulationCoroutine()
    {
        // Find and destroy existing simulation environment
        var existingEnvironment = GameObject.Find("Simulated Environment Scene");
        if (existingEnvironment != null)
        {
            Debug.Log("Destroying existing simulation environment");
            Destroy(existingEnvironment);
            yield return new WaitForSeconds(0.1f);
        }
        
        // Find and destroy DefaultSimulationEnvironment
        var defaultEnv = GameObject.Find("DefaultSimulationEnvironment(Clone)");
        if (defaultEnv != null)
        {
            Debug.Log("Destroying default simulation environment");
            Destroy(defaultEnv);
            yield return new WaitForSeconds(0.1f);
        }
        
        // Find and recreate AR Session
        var arSession = FindFirstObjectByType<ARSession>();
        if (arSession != null)
        {
            arSession.enabled = false;
            yield return new WaitForSeconds(0.2f);
            arSession.enabled = true;
            Debug.Log("AR Session reset for simulation");
        }
    }
}