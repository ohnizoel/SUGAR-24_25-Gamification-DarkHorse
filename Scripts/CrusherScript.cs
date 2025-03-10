// CrusherScript.cs
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;

public enum CrusherState { Empty, Crushing, Broken }

public class CrusherScript : MonoBehaviour
{
    public float mass = 0f;
    public SiteManager SiteManager;
    // Get correspondent stockpile
    public GameObject stockpile; // Assign in Inspector, or find it at runtime
    public GameObject MaterialInput;
    public StockpileScript stockpile_input;
    public CrusherState currentState = CrusherState.Empty;
    private float crushedMaterial = 0f;
    private float inputedMaterial = 0f;
    public float crushSpeed = 0.01f;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Get correspondent stockpile
        if (stockpile == null)
        {
            Debug.LogError("Crusher has no assigned stockpile! Assign it in the Unity Inspector.");
        }
        SiteManager = FindFirstObjectByType<SiteManager>();
    }

    // Update is called once per frame
    void Update()
    {
        switch(currentState)
        {
            case CrusherState.Empty:
                Empty();
                break;
            case CrusherState.Crushing:
                Crush();
                break;
            case CrusherState.Broken:
                // Possibly no movement
                break;
        }
    }
    void Empty()
    {   
        if (mass > 0) {currentState = CrusherState.Crushing;}
        if (MaterialInput == null) return;
        if (MaterialInput.CompareTag("Crusher_outcome")){
            if (stockpile_input.mass > 0){
                inputedMaterial = crushSpeed;
                mass += inputedMaterial;
                stockpile_input.mass -= inputedMaterial;
                currentState = CrusherState.Crushing;
            }
        }
    }
    void Crush()
    {   
        if (MaterialInput.CompareTag("Truck")){
            VehicleController truck = MaterialInput.GetComponent<VehicleController>();
            truck.currentState = VehicleState.Unloading;
            // Assign the crusher as the target
            truck.AssignedUnloadingTarget = gameObject;
        }
        
        if (mass > 0){
            crushedMaterial = crushSpeed;
            mass -= crushedMaterial;
            stockpile.GetComponent<StockpileScript>().AddToStockpileMass(crushedMaterial);
        } else if (mass <= 0)
        {
            mass = 0.00000f;
            currentState = CrusherState.Empty;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Truck")
        {
            VehicleController truck = collision.gameObject.GetComponent<VehicleController>();

            if (truck != null)
            {
                MaterialInput = collision.gameObject;
                
                currentState = CrusherState.Crushing;
            }
        } else
        if (collision.gameObject.tag == "Crusher_outcome")
        {   
            MaterialInput = collision.gameObject;
            stockpile_input = MaterialInput.GetComponent<StockpileScript>();
            currentState = CrusherState.Empty;
        }
    }

}
