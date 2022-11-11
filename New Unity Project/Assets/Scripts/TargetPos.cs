using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetPos : MonoBehaviour
{

    public Transform CamPos;
    public Transform PlayerPos;

    Vector3 direction;

    public static Vector3 _direction;
    public static Vector3 _breakDirection;
    public static Vector3 UnitVector;

    void LateUpdate()
    {
        direction = (CamPos.position - PlayerPos.position + Quaternion.Euler(0, 90, 0) * (CamPos.position - PlayerPos.position)) * 20;

        transform.position = -direction;

        _direction = PlayerPos.position - CamPos.position;

        _breakDirection = Quaternion.Euler(0, 180, 0) * direction;

        UnitVector = _direction.normalized;
    }
}
