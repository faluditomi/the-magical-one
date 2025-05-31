using UnityEngine;

[CreateAssetMenu(fileName = "LevitateSpell", menuName = "Spells/LaunchSpell")]
public class LaunchSpell : Spell
{
    
    [Min(0f), Tooltip("How hard the object should be shot away.")]
    public float launchForce;

    protected override SpellArgs CreateArgs()
    {
        LaunchArgs myArgs = new LaunchArgs
        {
            launchForce = this.launchForce
        };

        return CopyTo(myArgs);
    }

}

public class LaunchArgs : SpellArgs
{

    public float launchForce;

}
