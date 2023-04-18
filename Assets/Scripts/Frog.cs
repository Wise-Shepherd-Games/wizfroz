using UnityEngine;
using Universe;

public class Frog : MonoBehaviour
{
    public Planet LandedPlanet = null;

    void OnTriggerEnter2D(Collider2D other)
    {
        other.gameObject.TryGetComponent<Planet>(out LandedPlanet);

        if (LandedPlanet != null)
            transform.SetParent(LandedPlanet.transform);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.TryGetComponent<Planet>(out Planet planet))
        {
            if (planet == LandedPlanet)
            {
                LandedPlanet = null;
                transform.SetParent(null);
            }
        }
    }

}