// VehicleController.cs
using UnityEngine;

public enum VehicleState { Travelling, Loading, Unloading, Broken }

public class VehicleController : MonoBehaviour
{
    public SiteManager SiteManager;
    public Transform[] waypoints;
    public float speed = 8f;
    public float capacity = 10f;
    public float currentLoad = 0f;
    public VehicleState currentState = VehicleState.Travelling;
    public GameObject AssignedUnloadingTarget;
    public GameObject Target;

    // Example: how long to load/unload
    public float loadTime = 4f;
    public float unloadTime = 2f;
    private float timer = 0f;

    private int currentWaypointIndex = 0;

    void Start()
    {
        SiteManager = FindFirstObjectByType<SiteManager>();
    }

    void Update()
    {
        switch(currentState)
        {
            case VehicleState.Travelling:
                Travel();
                break;
            case VehicleState.Loading:
                Load();
                break;
            case VehicleState.Unloading:
                Unload();
                break;
            case VehicleState.Broken:
                // Possibly no movement
                break;
        }
    }

    void Travel()
    {
        if (waypoints.Length == 0) return;

        Transform targetWaypoint = waypoints[currentWaypointIndex];
        Target = targetWaypoint.gameObject;
        float step = speed * Time.deltaTime;
        transform.position = Vector2.MoveTowards(transform.position, targetWaypoint.position, step);

        if (Vector2.Distance(transform.position, targetWaypoint.position) < 0.01f)
        {
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;

        }
    }

    void Load()
    {
        timer += Time.deltaTime;
        if (timer >= loadTime)
        {
            // Assume full load
            currentLoad = capacity;
            timer = 0f;
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
            transform.localScale = new Vector3(-1, 1, 1);
            currentState = VehicleState.Travelling;
        }
    }

    void Unload()
    {
        timer += Time.deltaTime;
        if (timer >= unloadTime)
        {
            if (AssignedUnloadingTarget != null)
            {
                if (AssignedUnloadingTarget.CompareTag("Stockpile"))
                {
                    // Get StockpileScript and add mass
                    AssignedUnloadingTarget.GetComponent<StockpileScript>().AddToStockpileMass(currentLoad);
                }
                else if (AssignedUnloadingTarget.CompareTag("Crusher"))
                {
                    // Get CrusherScript and add mass
                    AssignedUnloadingTarget.GetComponent<CrusherScript>().mass += currentLoad;
                }
            }
            else
            {
                Debug.LogError("No assigned unloading target! The truck doesn't know where to unload.");
            }

            // Switch back to traveling, etc.
            // Unload
            currentLoad = 0f;
            timer = 0f;
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
            transform.localScale = new Vector3(1, 1, 1);
            currentState = VehicleState.Travelling;
        }
    }
}
