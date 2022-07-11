using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "Abilities/RapidFireAbility")]
public class RapidFireAbility : Ability {

    private RapidFireTriggerable launcher;

    public override void Initialize(GameObject obj)
    {
        launcher = obj.GetComponent<RapidFireTriggerable> ();
    }

    public override void TriggerAbility()
    {
        launcher.Launch ();
    }

}