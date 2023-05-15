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
        UIEventManager.UpdateManaBarUI(f.Mana);
        UIEventManager.GotCollectableToUI("mana");
    }

    public override void ActOnExit(Frog f)
    {
        Destroy(this.gameObject);
    }
}
