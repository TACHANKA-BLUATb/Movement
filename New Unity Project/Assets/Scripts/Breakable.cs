using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Breakable : MonoBehaviour
{
    [SerializeField] private GameObject _broken;

    void Start()
    {

    }

    void Update()
    {
        
    }

    private void OnTriggerEnter (Collider collider)
    {
        if (collider.gameObject.tag == "MeleeWeapon")
        {
            Instantiate(_broken);
            _broken.transform.position = transform.position;
            StartCoroutine(DestructionDelay());
        }
    }
    
    IEnumerator DestructionDelay()
    {
        yield return new WaitForEndOfFrame();
        gameObject.SetActive(false);
    }
}
