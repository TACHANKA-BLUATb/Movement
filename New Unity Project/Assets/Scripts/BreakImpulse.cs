using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakImpulse : MonoBehaviour
{
    private Rigidbody Impulse;
    void Start()
    {
        Impulse = GetComponent<Rigidbody>();
        Impulse.velocity = (TargetPos._breakDirection * 0.01f);
    }
}
