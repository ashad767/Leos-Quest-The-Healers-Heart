using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSword : PlayerWeapon
{
    public float damage;

    public PlayerSword(Sprite _weaponImage, string _weaponName) : base(_weaponImage, _weaponName)
    {
    }

    public virtual void Attack(List<GameObject> hitTargets, int extraDamage)
    {
        foreach (GameObject target in hitTargets)
        {   
            if( target != null)
            {
                Entity current = target.GetComponentInChildren<Entity>();

                if (current != null && !target.CompareTag("Player"))
                {
                    current.TakeDamage(damage + extraDamage);
                }
            }
        }
    }
}
