using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    public static CameraMove Instance;

    [Header("Seguimiento")]
    [SerializeField] private Transform target; 
    [SerializeField] private float smoothSpeed = 5f;
    [SerializeField] private Vector3 offset = new Vector3(0, 0, -10);

    
    private Vector3 currentShakeOffset = Vector3.zero;
    private float shakeTimer = 0f;
    private float shakeMagnitude = 0f;

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }

    private void Start()
    {
        if (target == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null) target = player.transform;
        }
    }

    private void Update()
    {
        
        if (shakeTimer > 0)
        {
            shakeTimer -= Time.deltaTime;

          
            float randomX = Random.Range(-1f, 1f) * shakeMagnitude;
            float randomY = Random.Range(-1f, 1f) * shakeMagnitude;
            currentShakeOffset = new Vector3(randomX, randomY, 0);

          
            shakeMagnitude = Mathf.Lerp(shakeMagnitude, 0f, Time.deltaTime * 10f);
        }
        else
        {
            currentShakeOffset = Vector3.zero;
        }
    }

    private void LateUpdate()
    {
        if (target == null) return;

        
        Vector3 desiredPosition = target.position + offset;

      
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);

        
        transform.position = smoothedPosition + currentShakeOffset;
    }


    public void ShakeCamera(float duration, float magnitude)
    {
        shakeTimer = duration;
        shakeMagnitude = magnitude;
    }
}