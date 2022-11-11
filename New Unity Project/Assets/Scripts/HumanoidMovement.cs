using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanoidMovement : MonoBehaviour
{
    [SerializeField] private Animator _anim;
    [SerializeField] private Rigidbody _playerRigidbody;
    [SerializeField] private Transform _turnDetector;
    [SerializeField] private Transform _playerDir;
    [SerializeField] private Transform _movementDir;
    [SerializeField] private Transform _cameraKernel;
    [SerializeField] private float MoveSpd;
    [SerializeField] private float MouseRotationSpd;
    [SerializeField] private float StopSpeed;

    private float x;
    private float y;
    private float mouseX;

    private Vector3 startSpeed;
    private Vector3 playerDir;
    private Vector3 movementDir;
    private Vector3 impulse;
    private Vector3 tAroundVek;
    private bool setTAroundVek;
    private float turnAngle;

    private bool animBlock;
    bool AnimCorBlock;

    private void Update() //not physical
    {
        Debug.Log(Time.deltaTime);

        x = Input.GetAxisRaw("Horizontal");
        y = Input.GetAxisRaw("Vertical");
        mouseX = Input.GetAxisRaw("Mouse X");

        movementDir = _cameraKernel.TransformVector(new Vector3(x, 0, y).normalized);

        if (!(_anim.GetBool("TurnAround") || _anim.GetCurrentAnimatorStateInfo(0).IsName("Armature|разворот левая последняя") ||
            _anim.GetCurrentAnimatorStateInfo(0).IsName("Armature|разворот правая последняя")))
            startSpeed = playerDir * MoveSpd;
    }

    private void LateUpdate() //not physical
    {
        _cameraKernel.Rotate(0f, mouseX * MouseRotationSpd, 0f);
        _cameraKernel.position = transform.position;

        _turnDetector.position = transform.position + movementDir;
        turnAngle = Vector3.Angle(playerDir, movementDir);

        if (Mathf.Abs(impulse.x) < 2.1f && Mathf.Abs(impulse.z) < 2.1f)
        {
            animBlock = false;
            startSpeed = Vector3.zero;
        }

        if (x != 0 || y != 0)
        {
            if (animBlock == false)
            {
                if (AnimCorBlock == false)
                    StartCoroutine(AnimBlock());

                StartCoroutine(AnimDelay());
            }
            else if (turnAngle > 170)
            {
                _anim.SetBool("TurnAround", true);
                if (setTAroundVek == true)
                {
                    setTAroundVek = false;
                    tAroundVek = playerDir * MoveSpd;
                }         
                StartCoroutine(AnimReset());
            }
            else if (!_anim.GetBool("TurnAround"))
                transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(movementDir), 1.4f);

            if (turnAngle > 10)
                if (_turnDetector.localPosition.x > 0)
                    _anim.SetBool("RightIncline", true);
                else
                    _anim.SetBool("LeftIncline", true);
            else
            {
                _anim.SetBool("RightIncline", false);
                _anim.SetBool("LeftIncline", false);
            } 
        }
        if (!(_anim.GetCurrentAnimatorStateInfo(0).IsName("Armature|разворот левая последняя") ||
                    _anim.GetCurrentAnimatorStateInfo(0).IsName("Armature|разворот правая последняя")))
            setTAroundVek = true;
    }

    void FixedUpdate() //physical
    {


        playerDir = _playerDir.position - transform.position;

        if (x != 0 || y != 0)
        {
            if (!(_anim.GetCurrentAnimatorStateInfo(0).IsName("Armature|разворот левая последняя") ||
                    _anim.GetCurrentAnimatorStateInfo(0).IsName("Armature|разворот правая последняя") ||
                    _anim.GetCurrentAnimatorStateInfo(0).IsName("Armature|начало ходьбы вперед") ||
                    _anim.GetCurrentAnimatorStateInfo(0).IsName("Armature|начало ходьбы вправо") ||
                    _anim.GetCurrentAnimatorStateInfo(0).IsName("Armature|начало ходьбы влево") ||
                    _anim.GetCurrentAnimatorStateInfo(0).IsName("Armature|начало ходьбы назад") ||
                    _anim.GetCurrentAnimatorStateInfo(0).IsName("Armature|стоит") ||
                    _anim.GetCurrentAnimatorStateInfo(0).IsName("Armature|предпоследний левый шаг") ||
                    _anim.GetCurrentAnimatorStateInfo(0).IsName("Armature|предпоследний правый шаг") ||
                    _anim.GetCurrentAnimatorStateInfo(0).IsName("Armature|последний левый шаг") ||
                    _anim.GetCurrentAnimatorStateInfo(0).IsName("Armature|последний правый шаг")))
                _playerRigidbody.velocity = playerDir * MoveSpd;

            if (_anim.GetCurrentAnimatorStateInfo(0).IsName("Armature|начало ходьбы вперед") ||
                    _anim.GetCurrentAnimatorStateInfo(0).IsName("Armature|начало ходьбы вправо") ||
                    _anim.GetCurrentAnimatorStateInfo(0).IsName("Armature|начало ходьбы влево") ||
                    _anim.GetCurrentAnimatorStateInfo(0).IsName("Armature|начало ходьбы назад"))
                _playerRigidbody.velocity = Vector3.Lerp(Vector3.zero, movementDir * MoveSpd, Time.deltaTime * 30f);

            if (_anim.GetCurrentAnimatorStateInfo(0).IsName("Armature|разворот левая последняя") ||
                    _anim.GetCurrentAnimatorStateInfo(0).IsName("Armature|разворот правая последняя"))
                _playerRigidbody.velocity = Vector3.Lerp(tAroundVek, -tAroundVek, Time.deltaTime * 13f);

            impulse = playerDir * MoveSpd;
            _anim.SetBool("Walk", true);
        }
        else
        {
            if (impulse.x > 0.2f || impulse.z > 0.2f || impulse.x < -0.2f || impulse.z < -0.2f)
            {
                _playerRigidbody.velocity = impulse;
                impulse.x /= 1.025f;
                impulse.z /= 1.025f;
            }
            else
            {
                _playerRigidbody.velocity = Vector3.zero;
                impulse = Vector3.zero;
            }
            _anim.SetBool("Walk", false);
        }   
    }

    IEnumerator AnimBlock()
    {
        AnimCorBlock = true;
        yield return new WaitForSeconds(0.4f);
        AnimCorBlock = false;
        animBlock = true;
    }

    IEnumerator AnimDelay()
    {
        yield return new WaitForSeconds(0.03f);
        if (turnAngle < 10)
        {
            _anim.SetBool("StartForward", true);
            StartCoroutine(AnimReset());
        }
        else if (turnAngle > 170)
        {
            _anim.SetBool("StartBack", true);
            StartCoroutine(AnimReset());
        }
        else if (turnAngle > 80 && turnAngle < 100)
        {
            if (_turnDetector.localPosition.x > 0)
            {
                _anim.SetBool("StartRight", true);
                StartCoroutine(AnimReset());
            }
            else
            {
                _anim.SetBool("StartLeft", true);
                StartCoroutine(AnimReset());
            }
        }
    }

    IEnumerator AnimReset()
    {
        yield return new WaitForSeconds(1.3f);
        _anim.SetBool("StartForward", false);
        _anim.SetBool("StartRight", false);
        _anim.SetBool("StartLeft", false);
        _anim.SetBool("StartBack", false);
        _anim.SetBool("TurnAround", false);
    }
}
