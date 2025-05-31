using UnityEngine;

//TODO: Once there are more permanent spells in the codebase, remove this class and change all the references to it.
/// <summary>
/// This is an example SciptableObject for later on how we can create spells that require unique input arguments.
/// </summary>
[CreateAssetMenu(fileName = "ExampleComplexSpell", menuName = "Spells/ExampleComplexSpell")]
public class ExampleComplexSpell : Spell
{

    public int someProperty;

    protected override SpellArgs CreateArgs()
    {
        ExampleComplexArgs myArgs = new ExampleComplexArgs
        {
            someProperty = this.someProperty
        };

        return CopyTo(myArgs);
    }

    //NOTE
    // for complex spells, here we can also include calculations, logic, or even other services, if needed

}

/// <summary>
/// The args classes, used for passing around spell specific input parameters, are included in the spell class 
/// for convenience and simplicity.
/// </summary>
public class ExampleComplexArgs : SpellArgs
{

    public int someProperty;

}
