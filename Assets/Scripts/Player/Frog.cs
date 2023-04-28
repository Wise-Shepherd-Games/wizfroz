using System.Collections.Generic;

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

using Universe;

namespace Player
{
    public class Frog : MonoBehaviour
    {
        [Header("Frog Behaviour Characteristics:")]
        public float JumpForce;
        public float MaxSecondsFloating = 3f;

        [Space(20)]
        [Header("Actions:")]
        public InputAction JumpAction;

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
        public bool HasSpellActive = false;
        public List<Spell> Spells;
        Planet LandedPlanet = null;
        Planet LastPlanet = null;

        private void Awake()
        {
            rigidBody = gameObject.GetComponent<Rigidbody2D>();
            JumpAction.performed += OnJump;
            jumpParticle.Stop();

            // THIS IS EXPLODING AND I DO NOT KNOW WHYYYYYYYYYYY :ccccccccccccccccccccccccccccccccccccccccccccc
            Spells.ForEach((s) => s.inputAction.performed += (ctx) => s.Act(this));

            VisualElement rootElement = deathScreenUI.rootVisualElement;
            rootElement.Q<Button>("RestartBtn").clicked += OnRestartClicked;
            rootElement.Q<Button>("HomeBtn").clicked += OnHomeClicked;
            rootElement.Q<Button>("NextBtn").clicked += OnNextClicked;
            rootElement.style.visibility = Visibility.Hidden;
        }

        private void OnEnable()
        {
            JumpAction.Enable();
            Spells.ForEach((s) => s.inputAction.Enable());
        }

        private void OnDisable()
        {
            JumpAction.Disable();
            Spells.ForEach((s) => s.inputAction.Disable());
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

            if ((other.gameObject.tag == "Obstacle" || other.gameObject.tag == "Enemy") && HasSpellActive == false)
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

        private void OnNextClicked()
        {

        }
    }
}