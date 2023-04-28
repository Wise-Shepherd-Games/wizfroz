using System.Collections;
using UnityEngine;

namespace Player
{
    [CreateAssetMenu(fileName = "InvisibilitySpell", menuName = "Spell/Invisibility", order = 1)]
    public class InvisibleSpell : Spell
    {
        private const float ALPHA_VALUE_STARTS_AT = 0.3f;
        public float InvisibilityDuration = 0f;
        private float countdown = 0f;
        private float frag;

        protected override void HandleAction(Frog frog)
        {
            countdown = 0f;
            frog.HasSpellActive = true;
            var color = frog.SpriteRenderer.material.color;
            frog.SpriteRenderer.material.color = new Color(color.r, color.g, color.b, ALPHA_VALUE_STARTS_AT);
            frog.LeftFootSpriteRenderer.material.color = new Color(color.r, color.g, color.b, ALPHA_VALUE_STARTS_AT);
            frog.RightFootSpriteRenderer.material.color = new Color(color.r, color.g, color.b, ALPHA_VALUE_STARTS_AT);
            this.frag = ALPHA_VALUE_STARTS_AT / InvisibilityDuration;
            frog.StartCoroutine(CountdownInvisible(frog));
        }

        IEnumerator CountdownInvisible(Frog frog)
        {
            while (frog.SpriteRenderer.material.color.a < 1 && countdown < InvisibilityDuration)
            {
                float t = countdown / InvisibilityDuration;
                var color = frog.SpriteRenderer.material.color;
                float alpha = Mathf.Lerp(ALPHA_VALUE_STARTS_AT, color.a + frag, t);
                frog.SpriteRenderer.material.color = new Color(color.r, color.g, color.b, alpha);
                frog.LeftFootSpriteRenderer.material.color = new Color(color.r, color.g, color.b, alpha);
                frog.RightFootSpriteRenderer.material.color = new Color(color.r, color.g, color.b, alpha);
                countdown++;
                yield return new WaitForSeconds(1f);
            }

            frog.SpriteRenderer.material.color = Color.white;
            frog.LeftFootSpriteRenderer.material.color = Color.white;
            frog.RightFootSpriteRenderer.material.color = Color.white;
            frog.HasSpellActive = false;
        }
    }
}