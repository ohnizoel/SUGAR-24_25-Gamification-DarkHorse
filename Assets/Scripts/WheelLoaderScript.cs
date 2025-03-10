using UnityEngine;

public enum WheelLoaderState { Idle, Travelling, HandleStockpile, Loading, Unloading, Broken }

public class WheelLoaderScript : MonoBehaviour
{
    public SiteManager SiteManager;
    public Transform[] waypoints;
    public StockpileScript[] stockpiles;
    public float speed = 4f;
    public float capacity = 3f;
    public float currentLoad = 0f;
    public WheelLoaderState currentState = WheelLoaderState.Idle;
    public GameObject AssignedUnloadingTarget;
    public GameObject AssignedloadingTarget;
    public float loadTime = 4f;
    public float unloadTime = 2f;
    private float timer = 0f;

    private int currentWaypointIndex = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SiteManager = FindFirstObjectByType<SiteManager>();
        stockpiles = GameObject.FindObjectsOfType<StockpileScript>();
    }

    // Update is called once per frame
    void Update()
    {
        switch(currentState)
        {
            case WheelLoaderState.Idle:
                Idle();
                break;
            case WheelLoaderState.Travelling:
                Travel();
                break;
            case WheelLoaderState.HandleStockpile:
                HandleStockpile();
                break;
            case WheelLoaderState.Loading:
                Load();
                break;
            case WheelLoaderState.Unloading:
                Unload();
                break;
            case WheelLoaderState.Broken:
                // Possibly no movement
                break;
        }
    }

    void Idle()
    {
        // Check if any Crushers Stockpiles are over capacity
        // If so, go to the one with the most material
        for (int i = 0; i < stockpiles.Length; i++)
        {
            if (stockpiles[i].mass > 20 && (stockpiles[i].tag == "Crusher1_outcome" || stockpiles[i].tag == "Crusher2_outcome" || stockpiles[i].tag == "Crusher3_outcome" || stockpiles[i].tag == "Crusher4_outcome"))
            {
                waypoints[0] = stockpiles[i].transform;
                currentState = WheelLoaderState.Travelling;
            }
        }
    }
    void Travel()
    {
        if (waypoints.Length == 0) return;

        Transform targetWaypoint = waypoints[currentWaypointIndex];
        float step = speed * Time.deltaTime;
        transform.position = Vector2.MoveTowards(transform.position, targetWaypoint.position, step);

        if (Vector2.Distance(transform.position, targetWaypoint.position) < 0.01f)
        {
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;

        }
    }

    void HandleStockpile()
    {
        if (waypoints[currentWaypointIndex] == AssignedUnloadingTarget.transform)
        {
            currentState = WheelLoaderState.Unloading;
        }
        else
        {
            currentState = WheelLoaderState.Loading;
        }
    }
    void Load()
    {
        timer += Time.deltaTime;
        if (timer >= loadTime)
        {
            if (AssignedloadingTarget != null)
            {
                if (AssignedloadingTarget.CompareTag("Crusher_outcome"))
                {
                    // Get StockpileScript and add mass
                    AssignedloadingTarget.GetComponent<StockpileScript>().mass -= capacity;
                }
            }
            else
            {
                Debug.LogError("No assigned loading target! The truck doesn't know where to load.");
            }
            // Assume full load
            currentLoad = capacity;
            timer = 0f;
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
            transform.localScale = new Vector3(-1, 1, 1);
            currentState = WheelLoaderState.Travelling;
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
                    AssignedUnloadingTarget.GetComponent<CrusherScript>().currentState = CrusherState.Crushing; // Start crushing process
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
            currentState = WheelLoaderState.Idle;
        }
    }
}
