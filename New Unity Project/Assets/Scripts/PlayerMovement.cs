using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private Animator _anim;

    public GameObject Weapon;
    private Collider WeaponColl;

    private Rigidbody _playerRigidbody;

    public Transform PosTarget;

    public Transform WalkTarget;
    Vector3 WalkVector;

    public byte turnSpeed;
    float MovementForce;

    Vector3 MovementDir;

    float YkfAttack = 8;
    float YkfNotAttack = -16;
    float ActualYkf = -16;

    void Start()
    {
        _playerRigidbody = GetComponent<Rigidbody>();

        WeaponColl = Weapon.GetComponent<Collider>();
        WeaponColl.enabled = false;
    }


    void Update()
    {
        Animation();
        Movement(new Vector3(MovementDir.x, 0, MovementDir.z));
    }

    void Movement(Vector3 _direction)
    {
        float vertical = Input.GetAxis("Vertical");
        float horizontal = Input.GetAxis("Horizontal");

        if ((vertical != 0) || (horizontal != 0))
        {
            WalkVector = WalkTarget.position - transform.position;
            _anim.SetBool("Walk", true);
        }
        else
        {
            WalkVector = Vector3.zero;
            _anim.SetBool("Walk", false);
        }

        MovementDir = TargetPos.UnitVector * vertical + Quaternion.Euler(0, 90, 0) * TargetPos.UnitVector * horizontal;

        //ROTATION==\/===========================================================================================================
        if ((vertical == 0) && (horizontal == 0) && (Input.GetKeyDown(KeyCode.Mouse0)) && (Input.GetKey(KeyCode.Mouse1)))
        {
            WeaponColl.enabled = true;
            StartCoroutine(CollTimer());
        }
        Vector3 dir = Quaternion.Euler(0, ActualYkf, 0) * PosTarget.position - transform.position;
        dir.y = 0;

        if ((vertical != 0) && (horizontal != 0))
        {
            MovementForce *= Mathf.Sqrt(0.5f);
        }

        if (_anim.GetBool("OnAttack") == true)
        {
            MovementForce = 8f;
            ActualYkf = Mathf.MoveTowards(YkfAttack, YkfNotAttack, Time.deltaTime);
            _playerRigidbody.velocity = MovementDir * MovementForce;

            if (!(Input.GetKey(KeyCode.Mouse0)))
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), turnSpeed * Time.deltaTime);
        }
        else
        {
            Vector3 targetForward = Vector3.RotateTowards(transform.forward, _direction, Time.deltaTime * 3f, .1f);
            Quaternion _newRotation = Quaternion.LookRotation(targetForward);
            transform.rotation = _newRotation;

            MovementForce = 15f;
            ActualYkf = Mathf.MoveTowards(YkfNotAttack, YkfAttack, Time.deltaTime);
            _playerRigidbody.velocity = WalkVector * MovementForce;
        }

        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        PosTarget.position = ray.GetPoint(15);
    }

    IEnumerator CollTimer()
    {
        yield return new WaitForSeconds(1f);
        WeaponColl.enabled = false;
    }

    void Animation()
    {
        if (Input.GetKeyDown(KeyCode.W))
            _anim.SetBool("W", true);

        if (Input.GetKeyUp(KeyCode.W))
            _anim.SetBool("W", false);

        if (Input.GetKeyDown(KeyCode.D))
            _anim.SetBool("D", true);

        if (Input.GetKeyUp(KeyCode.D))
            _anim.SetBool("D", false);

        if (Input.GetKeyDown(KeyCode.S))
            _anim.SetBool("S", true);

        if (Input.GetKeyUp(KeyCode.S))
            _anim.SetBool("S", false);

        if (Input.GetKeyDown(KeyCode.A))
            _anim.SetBool("A", true);

        if (Input.GetKeyUp(KeyCode.A))
            _anim.SetBool("A", false);

        if (Input.GetKeyDown(KeyCode.Mouse1)) //ENABLE ATTACKING
        {
            _anim.SetBool("OnAttack", true);
        }

        if (Input.GetKeyUp(KeyCode.Mouse1)) //DISABLE ATTACKING
        {
            _anim.SetBool("OnAttack", false);
        }

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            _anim.SetBool("Attack", true);
        }

        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            _anim.SetBool("Attack", false);
        }    
    }   
}
