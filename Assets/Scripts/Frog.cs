using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

using Universe;
using Spells;
using Levels;

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


    [Space(20)]
    [Header("Stats and More:")]
    public float Mana = 0;
    public float MaxMana = 50f;
    public float OctobearTrophies = 0;
    public bool IsInvisible = false;
    public List<Spell> Spells;
    public Planet LandedPlanet = null;
    Planet LastPlanet = null;
    public bool Won = false;

    private void Awake()
    {
        rigidBody = gameObject.GetComponent<Rigidbody2D>();
        JumpAction.performed += OnJump;

        CastInvisibleSpellAction.performed += OnCastInvisibleSpell;
        CastChangePlanetDirectionAction.performed += OnCastMovePlanetSpell;
        CastSlowDownPlanetSpellAction.performed += OnCastSlowDownPlanetSpell;
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
            CancelInvoke("DieLostInSpace");
            rigidBody.velocity = Vector2.zero;

            if (LandedPlanet.IsWinPlanet)
            {
                LandedPlanet.StopAllCoroutines();
                Win();
            }

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

        if ((other.gameObject.tag == "Obstacle" || other.gameObject.tag == "Enemy") && IsInvisible == false && Won == false)
        {
            switch (other.gameObject.tag)
            {
                case "Obstacle":
                    Die("These space obstacles are, indeed, a nightmare!");
                    break;
                case "Enemy":
                    Die("Evil space creatures... beware of them!");
                    break;
                default:
                    Die("Let's try again!");
                    break;
            }
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.tag == "Obstacle" && IsInvisible == false)
        {
            Die("Let's try again!");
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

    public void Win()
    {
        Won = true;
        UIEventManager.EmitShowWinUI();
    }

    public void Die(string deathMessage)
    {
        UIEventManager.EmitShowDefeatUI(deathMessage);
        LevelsInfo.Levels[LevelsInfo.CurrentLevel].PlayersDeathCount++;
        Destroy(gameObject);
    }

    public void DieLostInSpace()
    {
        UIEventManager.EmitShowDefeatUI("Don't lose yourself to the endless space...");
        LevelsInfo.Levels[LevelsInfo.CurrentLevel].PlayersDeathCount++;
        Destroy(gameObject);
    }

    private void OnJump(InputAction.CallbackContext ctx)
    {
        if (this.Won) return;
        Invoke("DieLostInSpace", MaxSecondsFloating);
        rigidBody.AddForce(transform.up * JumpForce);
        jumpParticle.Play();
    }

    private void OnCastInvisibleSpell(InputAction.CallbackContext ctx)
    {
        if (this.Won) return;

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
        if (this.Won) return;

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
        if (this.Won) return;

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
}