using UnityEngine;

[CreateAssetMenu(fileName = "LevitateSpell", menuName = "Spells/LevitateSpell")]
public class LevitateSpell : Spell
{

    [Min(0f), Tooltip("How much the object should move around while it's levitating.")]
    public float jitterMagnitude;
    [Min(0f), Tooltip("How far from the levitation point the object starts to float around randomly.")]
    public float currentCollectedRadius;

    protected override SpellArgs CreateArgs()
    {
        LevitateArgs myArgs = new LevitateArgs
        {
            shuffleSpeed = this.jitterMagnitude,
            collectedRadius = this.currentCollectedRadius
        };

        return CopyTo(myArgs);
    }

}

public class LevitateArgs : SpellArgs
{

    public float shuffleSpeed;
    public float collectedRadius;

}
