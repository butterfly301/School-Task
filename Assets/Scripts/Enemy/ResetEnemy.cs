using System;
using UnityEngine;

public class ResetEnemy : MonoBehaviour
{
    private void OnEnable()
    {
        MyPooler.ObjectPooler.Instance.ResetAllPools();
    }
}
