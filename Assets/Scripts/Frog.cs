using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using Universe;

public class Frog : MonoBehaviour
{
    [Header("Frog Behaviour Characteristics:")]
    public float JumpForce;

    [Space(20)]
    [Header("Actions:")]
    public InputAction JumpAction;
    [SerializeField] float MaxSecondsFloating = 3f;
    Planet LandedPlanet = null;
    Planet LastPlanet = null;
    Rigidbody2D rigidBody;

    [Space(20)]
    [Header("Effects and More:")]
    [SerializeField] private ParticleSystem JumpParticle;
    [SerializeField] UIDocument deathScreenUI;
    GameObject deathScreenGameObject;

    [Space(20)]
    [Header("Stats:")]
    public float Mana = 0;
    public float OctobearTrophies = 0;

    void Awake()
    {
        rigidBody = gameObject.GetComponent<Rigidbody2D>();
        JumpAction.performed += OnJump;
        JumpParticle.Stop();

        VisualElement rootElement = deathScreenUI.rootVisualElement;
        rootElement.Q<Button>("RestartBtn").clicked += OnRestartClicked;
        rootElement.Q<Button>("HomeBtn").clicked += OnHomeClicked;
        rootElement.Q<Button>("NextBtn").clicked += OnNextClicked;
        rootElement.style.visibility = Visibility.Hidden;
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
            CancelInvoke("Die");
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

        if (other.gameObject.tag == "Obstacle" || other.gameObject.tag == "Enemy")
        {
            Die();
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

    public void Die()
    {
        deathScreenUI.rootVisualElement.style.visibility = Visibility.Visible;
        Destroy(gameObject);
    }

    void OnJump(InputAction.CallbackContext ctx)
    {
        Invoke("Die", MaxSecondsFloating);
        rigidBody.AddForce(transform.up * JumpForce);
        JumpParticle.Play();
    }

    void OnRestartClicked()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void OnHomeClicked()
    {

    }

    void OnNextClicked()
    {

    }
}