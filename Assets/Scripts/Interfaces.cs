using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IKillable
{
    void Kill(GameObject WhatKilledMe);
}

public interface IDamageable<T, G>
{
    void Damage(T damageTaken, G gameObject);
}

public interface IBloodGainable
{
    void GainBlood();
}
