using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitRules : MonoBehaviour
{
    /// <summary>
    /// to keep all the hitting rules in one place
    /// </summary>
    /// how much will enemy bounce when being hit
    [SerializeField] public float hitBounce = 1.5f;
    /// <summary>
    /// how long will the hit animation last
    /// </summary>
    [SerializeField] public float hitAnimationDuration = 0.55f;
    /// <summary>
    /// how much will the bounce sticks push players off
    /// </summary>
    [SerializeField] public float bounceStickConstant = 2f;
    [SerializeField] public float scoreMultiplier = 20f;

}
