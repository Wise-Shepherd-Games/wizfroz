using UnityEngine;

namespace Player
{
    public abstract class Spell : MonoBehaviour
    {
        public float ManaCost = 0f;
        [HideInInspector]
        public SpellTypes Type;
        public ParticleSystem particle;
        public enum SpellTypes
        {
            Invisible
        }

        public Spell(SpellTypes type)
        {
            this.Type = type;
        }

        public void Act(GameObject target)
        {
            if (!particle.isPlaying)
                particle.Play();

            Action(target);
        }

        public abstract void Action(GameObject target);
    }
}