using UnityEngine;
using UnityEngine.InputSystem;
using Universe;

public class Frog : MonoBehaviour
{
    [Header("Frog Behaviour Characteristics:")]
    public float JumpForce;

    [Space(20)]
    [Header("Actions:")]
    public InputAction JumpAction;

    Planet LandedPlanet = null;
    Planet LastPlanet = null;
    Rigidbody2D rigidBody;
    [SerializeField] private ParticleSystem JumpParticle;

    void OnEnable()
    {
        JumpAction.Enable();
    }

    void OnDisable()
    {
        JumpAction.Disable();
    }

    void Awake()
    {
        rigidBody = gameObject.GetComponent<Rigidbody2D>();
        JumpAction.performed += OnJump;
        JumpParticle.Stop();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        other.gameObject.TryGetComponent<Planet>(out LandedPlanet);

        if (LandedPlanet != null)
        {
            rigidBody.velocity = Vector2.zero;

            if (LastPlanet != null)
            {
                LastPlanet.TryGetComponent<CircleCollider2D>(out var collider2D);
                collider2D.enabled = true;
            }
            JumpParticle.Clear();
            JumpParticle.Stop();
            this.transform.SetParent(LandedPlanet.transform, true);

            Vector3 direction = (transform.position - LandedPlanet.transform.position).normalized;
            transform.rotation = Quaternion.FromToRotation(Vector3.up, direction);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.TryGetComponent<Planet>(out Planet planet))
        {
            this.transform.parent = null;
            this.transform.localScale = Vector3.one;

            planet.TryGetComponent<CircleCollider2D>(out var collider2D);
            collider2D.enabled = false;

            LastPlanet = planet;
            LandedPlanet = null;
        }
    }

    void OnJump(InputAction.CallbackContext ctx)
    {
        // if (LandedPlanet != null)
        // {
        rigidBody.AddForce(transform.up * JumpForce);
        JumpParticle.Play();
        //}
    }

}