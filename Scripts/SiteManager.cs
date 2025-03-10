// SiteManager.cs
using UnityEngine;
using UnityEngine.UI;

public class SiteManager : MonoBehaviour
{
    public VehicleController[] vehicles; // Assign in Inspector, or find them at runtime
    public ExcavatorController[] excavators; // Assign in Inspector, or find them at runtime
    public Text totalMaterialText;       // Link a UI Text for display
    
    void Start()
    {
        // Find all vehicles
        vehicles = GameObject.FindObjectsOfType<VehicleController>();
    }

    public float totalMaterialMoved; // This will keep growing over time

    // Called by any truck when it finishes unloading
    public void AddToTotalMaterialMoved(float unloadedAmount)
    {
        totalMaterialMoved = unloadedAmount;
    }
    void Update()
    {
        // If totalMaterialMoved changes over time
        totalMaterialText.text = totalMaterialMoved.ToString("F2") + " tons";
    }

}
