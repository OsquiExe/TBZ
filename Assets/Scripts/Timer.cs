using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Timer : MonoBehaviour
{
    [SerializeField] UnityEvent onTrigger;
    [SerializeField] float activaDespuesDe = 2f;

    private void OnEnable() {
        Invoke("Trigger", activaDespuesDe);
    }

    private void OnDisable() {
        CancelInvoke();
    }

    void Trigger() {
        onTrigger?.Invoke();
    }
}
