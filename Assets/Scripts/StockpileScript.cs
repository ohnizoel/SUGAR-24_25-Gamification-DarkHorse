// StockpileScript.cs
using System;
using UnityEngine;

public class StockpileScript : MonoBehaviour
{
    public float mass = 0f;
    public SiteManager SiteManager;
    // Stockpile position
    public Vector3 stockpilePosition;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SiteManager = GameObject.FindFirstObjectByType<SiteManager>();
        stockpilePosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {   
        // update scale of the object
        if (mass > 0)
        { 
            transform.localScale = new Vector3(MathF.Log10(mass),MathF.Log10(mass),MathF.Log10(mass));
            transform.position = stockpilePosition;
        } else
        {
            transform.localScale = new Vector3(0,0,0);
        }
        SiteManager.AddToTotalMaterialMoved(mass);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Truck")
        {
            collision.gameObject.GetComponent<VehicleController>().currentState = VehicleState.Unloading;
        }
        if (collision.gameObject.tag == "WheelLoader")
        {
            collision.gameObject.GetComponent<WheelLoaderScript>().currentState = WheelLoaderState.HandleStockpile;
        }
    }

    public void AddToStockpileMass(float load)
    {   
        // Set Rb mass
        mass += load;
    }
}
