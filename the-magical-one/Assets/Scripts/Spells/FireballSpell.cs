using UnityEngine;

[CreateAssetMenu(fileName = "FireballSpell", menuName = "Spells/FireballSpell")]
public class FireballSpell : Spell
{
    public float speed;

    public float radius;

    protected override SpellArgs CreateArgs()
    {
        FireballArgs myArgs = new FireballArgs
        {
            speed = this.speed,
            radius = this.radius
        };

        return CopyTo(myArgs);
    }
}

public class FireballArgs : SpellArgs
{
    public float speed;
    public float radius;
}