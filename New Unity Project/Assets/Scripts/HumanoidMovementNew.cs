using System.Collections;
using UnityEngine;

public class HumanoidMovementNew : MonoBehaviour
{
    [SerializeField] private Animator _anim;
    [SerializeField] private Rigidbody _playerRigidbody;
    [SerializeField] private Transform _turnDetector;
    [SerializeField] private Transform _cameraKernel;
    [SerializeField] private float MoveSpd;
    [SerializeField] private float MouseRotationSpd;

    private float x;
    private float y;
    private float mouseX;

    private Vector3 playerDir;
    private Vector3 movementDir;
    private Vector3 animDir;
    private bool animDirBlock;
    private Vector3 startVector;
    private Vector3 tAroundVector;
    private bool tAroundVecBlock;
    private bool vecMagnitudeBlock;
    private Vector3 stopVector;
    private bool stopVecBlock;
    private float turnAngle;

    private void Update() //not physical
    {
        x = Input.GetAxisRaw("Horizontal");
        y = Input.GetAxisRaw("Vertical");
        mouseX = Input.GetAxisRaw("Mouse X");

        movementDir = _cameraKernel.TransformVector(new Vector3(x, 0, y).normalized);

        if (x == 0 && y == 0)
            startVector = Vector3.zero;
    }

    private void LateUpdate() //not physical
    {
        _cameraKernel.Rotate(0f, mouseX * MouseRotationSpd, 0f);
        _cameraKernel.position = transform.position;

        _turnDetector.position = transform.position + movementDir;
        turnAngle = Vector3.Angle(playerDir, movementDir);

        if (x != 0 || y != 0)
        {

            _anim.SetBool("Walk", true);

            if (_anim.GetCurrentAnimatorStateInfo(0).IsName("Armature|ходьба правая вперед") ||
                _anim.GetCurrentAnimatorStateInfo(0).IsName("Armature|ходьба левая вперед") ||
                _anim.GetCurrentAnimatorStateInfo(0).IsName("Armature|ходьба наклон вправо левая вперед") ||
                _anim.GetCurrentAnimatorStateInfo(0).IsName("Armature|ходьба наклон вправо правая впере") ||
                _anim.GetCurrentAnimatorStateInfo(0).IsName("Armature|ходьба наклон влево правая вперед") ||
                _anim.GetCurrentAnimatorStateInfo(0).IsName("Armature|ходьба наклон влево левая вперед") ||
                _anim.GetCurrentAnimatorStateInfo(0).IsName("Armature|предпоследний левый шаг") ||
                _anim.GetCurrentAnimatorStateInfo(0).IsName("Armature|предпоследний правый шаг"))
                if (!_anim.GetBool("TurnAround"))
                    transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(movementDir), 1.5f);

            StartCoroutine(AnimDelay());

            if (turnAngle > 170)
            {
                _anim.SetBool("TurnAround", true);
                StartCoroutine(TurnAroundReset());
            }
            else if (turnAngle > 10)
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
        else
        {
            _anim.SetBool("LeftIncline", false);
            _anim.SetBool("RightIncline", false);
            _anim.SetBool("Walk", false);
        }

        if (_anim.GetCurrentAnimatorStateInfo(0).IsName("Armature|разворот левая последняя") ||
            _anim.GetCurrentAnimatorStateInfo(0).IsName("Armature|разворот правая последняя"))
        {
            if (tAroundVecBlock == false)
            {
                tAroundVector = playerDir * MoveSpd;
                tAroundVecBlock = true;
            }
        }
        else tAroundVecBlock = false;

        if (_anim.GetCurrentAnimatorStateInfo(0).IsName("Armature|предпоследний левый шаг") ||
            _anim.GetCurrentAnimatorStateInfo(0).IsName("Armature|предпоследний правый шаг") ||
            _anim.GetCurrentAnimatorStateInfo(0).IsName("Armature|последний левый шаг") ||
            _anim.GetCurrentAnimatorStateInfo(0).IsName("Armature|последний правый шаг"))
        {
            if (stopVecBlock == false)
            {
                stopVector = playerDir * MoveSpd;
                stopVecBlock = true;
            }
        }
        else stopVecBlock = false;
    }

    void FixedUpdate() //physical
    {
        if (_anim.GetCurrentAnimatorStateInfo(0).IsName("Armature|начало ходьбы вперед"))
        {
            if (animDirBlock == false)
            { 
                animDir = transform.TransformVector(Vector3.forward);
                animDirBlock = true;
            }
            startVector = Vector3.Lerp(startVector, animDir * MoveSpd, Time.deltaTime * 20f);
            _playerRigidbody.velocity = startVector;
        }
        else if (_anim.GetCurrentAnimatorStateInfo(0).IsName("Armature|начало ходьбы вправо"))
        {
            if (animDirBlock == false)
            { 
                animDir = transform.TransformVector(Vector3.right);
                animDirBlock = true;
            }
            startVector = Vector3.Lerp(startVector, animDir * MoveSpd, Time.deltaTime);
            _playerRigidbody.velocity = startVector;
        }
        else if (_anim.GetCurrentAnimatorStateInfo(0).IsName("Armature|начало ходьбы влево"))
        {
            if (animDirBlock == false)
            {
                animDir = transform.TransformVector(Vector3.left);
                animDirBlock = true;
            }
            startVector = Vector3.Lerp(startVector, animDir * MoveSpd, Time.deltaTime);
            _playerRigidbody.velocity = startVector;
        }
        else if (_anim.GetCurrentAnimatorStateInfo(0).IsName("Armature|начало ходьбы назад"))
        {
            if (animDirBlock == false)
            {
                animDir = -transform.TransformVector(Vector3.forward);
                animDirBlock = true;
            }

            startVector = Vector3.Lerp(startVector, animDir * MoveSpd, Time.deltaTime);
            _playerRigidbody.velocity = startVector;
        }
        else
            animDirBlock = false;

        if (_anim.GetCurrentAnimatorStateInfo(0).IsName("Armature|ходьба правая вперед") ||
            _anim.GetCurrentAnimatorStateInfo(0).IsName("Armature|ходьба левая вперед") ||
            _anim.GetCurrentAnimatorStateInfo(0).IsName("Armature|ходьба наклон вправо левая вперед") ||
            _anim.GetCurrentAnimatorStateInfo(0).IsName("Armature|ходьба наклон вправо правая впере") ||
            _anim.GetCurrentAnimatorStateInfo(0).IsName("Armature|ходьба наклон влево правая вперед") ||
            _anim.GetCurrentAnimatorStateInfo(0).IsName("Armature|ходьба наклон влево левая вперед"))
            _playerRigidbody.velocity = playerDir * MoveSpd;

        if (_anim.GetCurrentAnimatorStateInfo(0).IsName("Armature|разворот левая последняя") ||
            _anim.GetCurrentAnimatorStateInfo(0).IsName("Armature|разворот правая последняя"))
        {
            if (tAroundVector.magnitude > 1.7f && vecMagnitudeBlock == false)
            {
                tAroundVector = Vector3.Lerp(tAroundVector, Vector3.zero, Time.deltaTime * 1.5f);
                _playerRigidbody.velocity = tAroundVector;
            }
            else
            {
                vecMagnitudeBlock = true;
                tAroundVector = Vector3.Lerp(tAroundVector, playerDir * 5, Time.deltaTime * 100f);
                _playerRigidbody.velocity = tAroundVector;
            }
        }

        if (_anim.GetCurrentAnimatorStateInfo(0).IsName("Armature|предпоследний левый шаг") ||
            _anim.GetCurrentAnimatorStateInfo(0).IsName("Armature|предпоследний правый шаг") ||
            _anim.GetCurrentAnimatorStateInfo(0).IsName("Armature|последний левый шаг") ||
            _anim.GetCurrentAnimatorStateInfo(0).IsName("Armature|последний правый шаг"))
        {
            stopVector = Vector3.Lerp(stopVector, Vector3.zero, Time.deltaTime);
            _playerRigidbody.velocity = stopVector;
        }

        playerDir = transform.TransformVector(Vector3.forward);
    }


    IEnumerator AnimDelay()
    {
        yield return new WaitForSeconds(0.03f);
        if (turnAngle <= 45)
        {
            _anim.SetBool("StartForward", true);
            StartCoroutine(AnimReset());
        } 
        else
        {
            if (turnAngle > 135)
            {
                _anim.SetBool("StartBack", true);
                StartCoroutine(AnimReset());
            }
            else
            {
                if (turnAngle > 45 && turnAngle <= 135)
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
    }

    IEnumerator AnimReset()
    {
        yield return new WaitForSeconds(0.4f);
        _anim.SetBool("StartForward", false);
        _anim.SetBool("StartRight", false);
        _anim.SetBool("StartLeft", false);
        _anim.SetBool("StartBack", false);
    }

    IEnumerator TurnAroundReset()
    {
        yield return new WaitForSeconds(1.7f);
        _anim.SetBool("TurnAround", false);
        vecMagnitudeBlock = false;
    }
}
