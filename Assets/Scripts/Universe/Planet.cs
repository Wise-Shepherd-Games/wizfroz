using UnityEngine;
using System.Collections.Generic;

namespace Universe
{
    public class Planet : MonoBehaviour
    {
        [Header("Planet Behaviour Characteristics:")]
        public bool ShouldRandomizeData;
        public int RotateDirection;
        public float RotateVelocity;
        public float PlanetSize;
        private float MinSize = 0.1f;
        public int ScaleTime;

        [Space(20)]

        [Header("Planet Skins:")]
        public List<Sprite> PlanetSkins;

        private SpriteRenderer spriteRenderer;
        private Frog frog;

        private void Awake()
        {
            this.TryGetComponent<SpriteRenderer>(out spriteRenderer);
        }

        private void Start()
        {
            SetPlanet();
        }

        private void Update()
        {
            Rotate();

            if (ShouldDestroy())
                Destroy(gameObject);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            other.gameObject.TryGetComponent<Frog>(out frog);

            if (frog != null)
            {
                StopAllCoroutines();
                StartCoroutine(ChangeScaleOverTime(Vector2.zero, ScaleTime));
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            other.gameObject.TryGetComponent<Frog>(out frog);

            if (frog != null)
            {
                frog = null;
                StopAllCoroutines();
                StartCoroutine(ChangeScaleOverTime(Vector2.one * PlanetSize, ScaleTime));
            }
        }

        private void SetPlanet()
        {
            if (ShouldRandomizeData == true)
            {
                spriteRenderer.sprite = PlanetSkins[Random.Range(0, PlanetSkins.Count - 1)];
                PlanetSize = Random.Range(1f, 3.5f);
                RotateDirection = Mathf.Sign(Random.Range(-1.0f, 1.0f)) == -1 ? -1 : 1;
                RotateVelocity = Random.Range(90f, 180f);
                ScaleTime = Random.Range(1, 10);
            }

            transform.localScale = Vector3.one * PlanetSize;
        }

        private void Rotate()
        {
            float angle = Time.deltaTime * RotateVelocity * RotateDirection;
            transform.RotateAround(transform.position, transform.forward, angle);
        }

        private bool ShouldDestroy()
        {
            return transform.localScale.x <= MinSize && transform.localScale.y <= MinSize;
        }

        IEnumerator<float?> ChangeScaleOverTime(Vector3 scale, float time)
        {
            float progress = 0;
            float rate = 1 / time;

            Vector3 fromScale = transform.localScale;
            Vector3 toScale = scale;

            while (progress < 1)
            {
                progress += Time.deltaTime * rate;
                transform.localScale = Vector3.Lerp(fromScale, toScale, progress);
                yield return null;
            }
        }
    }
}