using UnityEngine;

namespace Universe
{
    public class Planet : MonoBehaviour
    {
        public Direction RotateTo = Direction.Right;
        public float AngularVelocity = 90f;
        public enum Direction { Left = 1, Right = -1 }

        [SerializeField]
        float Radius = 1f;
        [SerializeField]
        float ShrinkRate = 0.005f;
        [SerializeField]
        float MinimumRadius = 0.1f;

        void Start()
        {
            transform.localScale = Vector3.one * Radius;
        }

        void Update()
        {
            Rotate();

            if (ShouldDestroy())
                Destroy(gameObject);
        }

        public void Shrink()
        {
            float scale = Mathf.Lerp(Radius, MinimumRadius, Time.time * ShrinkRate);
            transform.localScale = new Vector2(scale, scale);
        }

        void Rotate()
        {
            float angle = Time.deltaTime * AngularVelocity * (int)RotateTo;
            transform.RotateAround(transform.position, transform.forward, angle);
        }

        bool ShouldDestroy()
        {
            Vector2 scale = transform.localScale;
            return scale.x == MinimumRadius && scale.y == MinimumRadius;
        }
    }
}