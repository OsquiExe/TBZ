using UnityEngine;
using UnityEngine.EventSystems;

public class VirtualJoystick : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler
{
    [Header("Referencias UI")]
    [Tooltip("Arrastrá acá el JoystickBackground")]
    public RectTransform joystickBackground;
    [Tooltip("Arrastrá acá el JoystickHandle")]
    public RectTransform joystickHandle;

    private Vector2 inputVector;

    private void Start()
    {
        if (joystickBackground != null)
        {
            joystickBackground.gameObject.SetActive(false);
        }
    }


    public void ResetJoystick()
    {
        inputVector = Vector2.zero;
        if (joystickHandle != null) joystickHandle.anchoredPosition = Vector2.zero;
        if (joystickBackground != null) joystickBackground.gameObject.SetActive(false);
    }

    public virtual void OnPointerDown(PointerEventData eventData)
    {
       
        if (PowerUpManager.Instance != null && PowerUpManager.Instance.uiPowerUpSelect != null && PowerUpManager.Instance.uiPowerUpSelect.activeInHierarchy) return;

        if (eventData.position.y > Screen.height / 2f) return;

        if (joystickBackground != null)
        {
            joystickBackground.position = eventData.position;
            joystickBackground.gameObject.SetActive(true);
        }

        OnDrag(eventData);
    }

    public virtual void OnDrag(PointerEventData eventData)
    {
        if (PowerUpManager.Instance != null && PowerUpManager.Instance.uiPowerUpSelect != null && PowerUpManager.Instance.uiPowerUpSelect.activeInHierarchy) return;
        if (joystickBackground == null || !joystickBackground.gameObject.activeSelf) return;

        Vector2 position;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(joystickBackground, eventData.position, eventData.pressEventCamera, out position))
        {
            position.x = (position.x / joystickBackground.sizeDelta.x) * 2;
            position.y = (position.y / joystickBackground.sizeDelta.y) * 2;

            inputVector = new Vector2(position.x, position.y);
            inputVector = (inputVector.magnitude > 1.0f) ? inputVector.normalized : inputVector;

            if (joystickHandle != null)
            {
                joystickHandle.anchoredPosition = new Vector2(
                    inputVector.x * (joystickBackground.sizeDelta.x / 2.5f),
                    inputVector.y * (joystickBackground.sizeDelta.y / 2.5f)
                );
            }
        }
    }

    public virtual void OnPointerUp(PointerEventData eventData)
    {
        ResetJoystick();
    }

    public float GetHorizontal() { return inputVector.x; }
    public float GetVertical() { return inputVector.y; }
}