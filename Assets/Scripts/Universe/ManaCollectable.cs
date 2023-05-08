using UnityEngine;

public class ManaCollectable : Collectable
{
    [Header("Characteristics:")]
    public float ManaGivenOnCollision;

    public override void ActOnEnter(Frog f)
    {
        particle.Play();
        if (f.Mana > f.MaxMana) return;
        f.Mana += ManaGivenOnCollision;
        UIEventManager.EmitUpdateManaBarUI(f.Mana);
        UIEventManager.EmitGotCollectableToUI("mana");
    }

    public override void ActOnExit(Frog f)
    {
        Destroy(this.gameObject);
    }
}
