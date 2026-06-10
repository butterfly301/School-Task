using System;
using System.Collections;
using UnityEngine;

public class MakeAPitAfterBoom : MonoBehaviour
{
    public GameObject pit;

    private void Start()
    {
        StartCoroutine(MakeAPit());
    }

    IEnumerator MakeAPit()
    {
        yield return new WaitForSeconds(0.3f);
        Instantiate(pit, transform.position, Quaternion.identity);
    }
}
