using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "Abilities/GrenadeAbility")]
public class GrenadeAbility : Ability {

    public Rigidbody projectile;

    private GrenadeThrowTriggerable launcher;

    public override void Initialize(GameObject obj)
    {
        launcher = obj.GetComponent<GrenadeThrowTriggerable> ();
        launcher.projectile = projectile;
    }

    public override void TriggerAbility()
    {
        launcher.Aim ();
    }

}