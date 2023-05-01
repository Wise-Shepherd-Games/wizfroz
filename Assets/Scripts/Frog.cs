using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

using Universe;
using Spells;

public class Frog : MonoBehaviour
{
    [Header("Frog Behaviour Characteristics:")]
    public float JumpForce;
    public float MaxSecondsFloating = 3f;

    [Space(20)]
    [Header("Actions:")]
    public InputAction JumpAction;
    public InputAction CastInvisibleSpellAction;
    public InputAction CastChangePlanetDirectionAction;
    public InputAction CastSlowDownPlanetSpellAction;

    [Space(20)]
    [Header("Components")]
    Rigidbody2D rigidBody;
    public SpriteRenderer SpriteRenderer;
    public SpriteRenderer LeftFootSpriteRenderer;
    public SpriteRenderer RightFootSpriteRenderer;

    [Space(20)]
    [Header("Effects and More:")]
    [SerializeField] private ParticleSystem jumpParticle;
    [SerializeField] private UIDocument deathScreenUI;
    GameObject deathScreenGameObject;

    [Space(20)]
    [Header("Stats and More:")]
    public float Mana = 0;
    public float OctobearTrophies = 0;
    public bool IsInvisible = false;
    public List<Spell> Spells;
    public Planet LandedPlanet = null;
    Planet LastPlanet = null;

    private void Awake()
    {
        rigidBody = gameObject.GetComponent<Rigidbody2D>();
        JumpAction.performed += OnJump;

        CastInvisibleSpellAction.performed += OnCastInvisibleSpell;
        CastChangePlanetDirectionAction.performed += OnCastMovePlanetSpell;
        CastSlowDownPlanetSpellAction.performed += OnCastSlowDownPlanetSpell;

        VisualElement rootElement = deathScreenUI.rootVisualElement;
        rootElement.Q<Button>("RestartBtn").clicked += OnRestartClicked;
        rootElement.Q<Button>("HomeBtn").clicked += OnHomeClicked;
        rootElement.style.visibility = Visibility.Hidden;
    }

    private void OnEnable()
    {
        JumpAction.Enable();
        CastInvisibleSpellAction.Enable();
        CastChangePlanetDirectionAction.Enable();
        CastSlowDownPlanetSpellAction.Enable();
    }

    private void OnDisable()
    {
        JumpAction.Disable();
        CastInvisibleSpellAction.Disable();
        CastChangePlanetDirectionAction.Disable();
        CastSlowDownPlanetSpellAction.Disable();
    }

    private void OnTriggerEnter2D(Collider2D other)
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
            jumpParticle.Clear();
            jumpParticle.Stop();
            this.transform.SetParent(LandedPlanet.transform, true);

            Vector3 direction = (transform.position - LandedPlanet.transform.position).normalized;
            transform.rotation = Quaternion.FromToRotation(Vector3.up, direction);
        }

        if ((other.gameObject.tag == "Obstacle" || other.gameObject.tag == "Enemy") && IsInvisible == false)
        {
            Die();
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.tag == "Obstacle" && IsInvisible == false)
        {
            Die();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.TryGetComponent<Planet>(out Planet planet))
        {
            this.transform.parent = null;
            Invoke("ResetScale", 0.1f);

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

    private void OnJump(InputAction.CallbackContext ctx)
    {
        Invoke("Die", MaxSecondsFloating);
        rigidBody.AddForce(transform.up * JumpForce);
        jumpParticle.Play();
    }

    private void OnCastInvisibleSpell(InputAction.CallbackContext ctx)
    {
        if (Mana != 0 && IsInvisible == false)
        {
            var spell = Spells.Where(obj => obj.Type == Spell.SpellTypes.Invisible).FirstOrDefault();

            if (spell.ManaCost > Mana) return;

            Mana -= spell.ManaCost;

            spell.Act(this.gameObject);
        }
    }

    private void OnCastMovePlanetSpell(InputAction.CallbackContext ctx)
    {
        if (Mana != 0 && LandedPlanet != null)
        {
            var spell = Spells.Where(obj => obj.Type == Spell.SpellTypes.ChangePlanetDirection).FirstOrDefault();

            if (spell.ManaCost > Mana) return;

            Mana -= spell.ManaCost;

            spell.Act(this.LandedPlanet.gameObject);
        }
    }

    private void OnCastSlowDownPlanetSpell(InputAction.CallbackContext ctx)
    {
        if (Mana != 0 && LandedPlanet != null)
        {
            if (LandedPlanet.IsSlowed == false)
            {
                var spell = Spells.Where(obj => obj.Type == Spell.SpellTypes.SlowDownPlanetSpell).FirstOrDefault();

                if (spell.ManaCost > Mana) return;

                Mana -= spell.ManaCost;

                spell.Act(this.LandedPlanet.gameObject);
            }
        }
    }

    private void ResetScale()
    {
        this.transform.localScale = Vector3.one;
    }

    private void OnRestartClicked()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void OnHomeClicked()
    {

    }
}