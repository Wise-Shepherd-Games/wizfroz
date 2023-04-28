using System.Collections;
using Player;
using UnityEngine;

public abstract class Collectable : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;

    public ParticleSystem particle;

    private void Awake()
    {
        this.TryGetComponent<SpriteRenderer>(out spriteRenderer);
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<Frog>(out var player))
        {
            this.spriteRenderer.hideFlags = HideFlags.None;
            this.spriteRenderer.color = new Color(0, 0, 0, 0);
            StopAllCoroutines();
            ActOnEnter(player);
        }

    }

    public void OnTriggerExit2D(Collider2D other)
    {
        if (other.TryGetComponent<Frog>(out var player))
        {
            StartCoroutine(WaitTillParticleEndsPlaying(player));
        }
    }

    public abstract void ActOnEnter(Frog f);

    public abstract void ActOnExit(Frog f);

    private IEnumerator WaitTillParticleEndsPlaying(Frog f)
    {
        while (particle.isPlaying)
        {
            yield return new WaitForSeconds(1f);
        }

        ActOnExit(f);
    }
}
