using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public abstract class Spell : ScriptableObject
    {
        public float ManaCost = 0f;

        // não sei como que pega as particulas dentro de um scriptable object :cccccccccccccccccccccccc
        //public ParticleSystem particle;

        public InputAction inputAction;


        public void Act(Frog frog)
        {
            /* if (!particle.isPlaying)
                particle.Play(); */

            if (!frog.HasSpellActive && frog.Mana >= ManaCost)
            {
                frog.Mana -= ManaCost;
                HandleAction(frog);
            }

        }

        protected abstract void HandleAction(Frog frog);
    }
}