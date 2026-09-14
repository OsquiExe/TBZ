using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
public class SimoneAim : MonoBehaviour
{
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] Transform sniperSpawn;
    [SerializeField] Transform sniperPivot;

    [SerializeField] float safeTouch = 2;

    Camera cam;
    bool isAiming;
    bool isShooting;
    Vector3 aimDirection;


    private void Awake() {
        cam = Camera.main;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        HandleAimInput();
    }

    private void HandleAimInput() {

        Vector3 _pos;

        if(Input.touchCount > 0) {
            Debug.Log(Input.touchCount);
            for(int i = 0; i < Input.touchCount; i++) {
                Touch touch = Input.GetTouch(i);
                _pos = cam.ScreenToWorldPoint(touch.position);

                ProcessInput(touch.phase, _pos);
            }
        } else {
            _pos = cam.ScreenToWorldPoint(Input.mousePosition);
            TouchPhase phase = Input.GetMouseButtonDown(0) ? TouchPhase.Began : Input.GetMouseButtonUp(0) ? TouchPhase.Ended : TouchPhase.Stationary;
            ProcessInput(phase, _pos);
        }
    }

    void ProcessInput(TouchPhase phase,Vector3 _pos) {
        if(phase == TouchPhase.Began && Vector2.Distance(transform.position, _pos) > safeTouch) {
            isAiming = true;

        } else if(isAiming && phase == TouchPhase.Ended) {

            isAiming = false;
            if(Vector2.Distance(transform.position, _pos) > safeTouch) {
                aimDirection = _pos;
                StartCoroutine(shootCorrutine());
            }
        }
    }

    IEnumerator shootCorrutine() {
        if(isShooting) yield break;

        isShooting = true;
        Debug.DrawLine(transform.position, aimDirection, Color.cyan);

        Vector3 rotDir = (aimDirection - transform.position).normalized;
        rotDir = Mathf.Atan2(rotDir.y, rotDir.x) * Mathf.Rad2Deg * Vector3.forward;
        Vector3 initialDir = sniperPivot.eulerAngles;

        float t = 0;
         while(t < 1) {
             sniperPivot.eulerAngles = LerpAngles(initialDir, rotDir, t);

             t += Time.deltaTime * 16f;
             yield return null;
         }

        yield return null;
        sniperPivot.eulerAngles = rotDir;

        if(CameraMove.Instance != null) {
            CameraMove.Instance.ShakeCamera(0.1f, 0.05f);
        }

        GameObject b = Instantiate(bulletPrefab, sniperSpawn.position, sniperPivot.rotation * Quaternion.Euler(new Vector3(0, 0, 180)));
        Rigidbody2D r = b.GetComponent<Rigidbody2D>();
        r.velocity = -b.transform.right * 10;

        isShooting = false;
    }

    public Vector3 LerpAngles(Vector3 a, Vector3 b, float t) {
        return new Vector3(Mathf.LerpAngle(a.x, b.x, t), Mathf.LerpAngle(a.y, b.y, t), Mathf.LerpAngle(a.z, b.z, t));
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, safeTouch);
    }
}
