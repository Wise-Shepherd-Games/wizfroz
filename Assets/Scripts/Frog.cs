using UnityEngine;
using UnityEngine.InputSystem;
using Universe;

public class Frog : MonoBehaviour
{
    public float JumpForce = 5f;
    public InputAction JumpAction;

    Planet LandedPlanet = null;
    Rigidbody2D rb;

    void Awake()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        JumpAction.performed += OnJump;
    }

    void Update()
    {
        if (LandedPlanet != null)
            LandedPlanet.Shrink();

    }

    void OnEnable()
    {
        JumpAction.Enable();
    }

    void OnDisable()
    {
        JumpAction.Disable();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        other.gameObject.TryGetComponent<Planet>(out LandedPlanet);

        if (LandedPlanet != null)
        {
            rb.velocity = Vector2.zero;

            Vector2 direction = (transform.position - LandedPlanet.transform.position).normalized;
            transform.rotation = Quaternion.FromToRotation(Vector3.up, direction);

            transform.SetParent(LandedPlanet.transform);
        }
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

    void OnJump(InputAction.CallbackContext ctx)
    {
        rb.AddForce(transform.up * JumpForce);
    }

}