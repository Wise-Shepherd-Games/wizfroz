using UnityEngine;
using Universe;

public class MovePlanetSpell : Spell
{
    public MovePlanetSpell() : base(SpellTypes.MovePlanet) { }

    public override void Action(GameObject target)
    {
        target.TryGetComponent<Frog>(out Frog frog);
        if (frog == null) return;

        Planet planet = frog.LandedPlanet;
        if (planet != null)
        {
            planet.RotateDirection *= -1;
        }

    }
}