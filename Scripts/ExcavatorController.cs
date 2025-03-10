// EcavatorController.cs
using UnityEngine;

public class ExcavatorController : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Truck")
        {
            collision.gameObject.GetComponent<VehicleController>().currentState = VehicleState.Loading;
        }
    }
}
