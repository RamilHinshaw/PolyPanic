using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamagable
{
    void Death();
    void Damage(float dmg, Vector3 force, float stunDur);
}
