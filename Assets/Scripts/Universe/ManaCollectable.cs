using UnityEngine;

public class ManaCollectable : Collectable
{
    [Header("Characteristics:")]
    public float ManaGivenOnCollision;

    public override void ActOnEnter(Frog f)
    {
        f.Mana += ManaGivenOnCollision;
    }

    public override void ActOnExit(Frog f)
    {
        Destroy(this.gameObject);
    }
}
