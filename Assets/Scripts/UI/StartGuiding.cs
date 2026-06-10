using System;
using UnityEngine;
using UnityEngine.Events;

public class StartGuiding : MonoBehaviour
{
    public UnityEvent startGuidingEvent;

    private void Start()
    {
        startGuidingEvent.Invoke();
    }
}
