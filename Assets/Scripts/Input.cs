using UnityEngine;
using UnityEngine.EventSystems;

public class Input : MonoBehaviour
{
    public delegate void InputEvent (Vector2 vector2);

    #region input events
    public void RegisterInput(InputEvent holdAndMoveEvent, InputEvent downEvent, InputEvent upEvent)
    {
        if (holdAndMoveEvent != null)
            OnHoldAndMove += holdAndMoveEvent;

        if (downEvent != null)
            OnDown += downEvent;

        if (upEvent != null)
            OnUp += upEvent;
    }

    public void UnRegisterInput(InputEvent holdAndMoveEvent, InputEvent downEvent, InputEvent upEvent)
    {
        if (holdAndMoveEvent != null)
            OnHoldAndMove -= holdAndMoveEvent;

        if (downEvent != null)
            OnDown -= downEvent;

        if (upEvent != null)
            OnUp -= upEvent;
    }
    #endregion

    #region private input variables
    private Vector2 lastPosition;
    #endregion

    #region events
    private InputEvent OnHoldAndMove;
    private InputEvent OnDown;
    private InputEvent OnUp;
    #endregion

    private void Update()
    {
        if (EventSystem.current.IsPointerOverGameObject())
            return;

        bool isMouseDown = false;
        Vector2 position;
        bool isMouseUp = false;
        bool isTouching = false;

        if (UnityEngine.Input.touchCount > 0)
        {
            if (UnityEngine.Input.touches[0].phase == TouchPhase.Began)
                isMouseDown = true;
            else if (UnityEngine.Input.touches[0].phase == TouchPhase.Ended)
                isMouseUp = true;
            else if (UnityEngine.Input.touches[0].phase == TouchPhase.Moved)
                isTouching = true;

            position = UnityEngine.Input.GetTouch(0).position;
        }
        else
        {
            position = UnityEngine.Input.mousePosition;
            isMouseDown = UnityEngine.Input.GetMouseButtonDown(0);
            isMouseUp = UnityEngine.Input.GetMouseButtonUp(0);
            isTouching = UnityEngine.Input.GetMouseButton(0);
        }

        if (isMouseDown)
        {
            lastPosition = position;
            OnDown?.Invoke(position);
        }

        if (isMouseUp)
            OnUp?.Invoke(position);

        if (isTouching)
        {
            if (position != lastPosition)
            {
                OnHoldAndMove?.Invoke(position);
                lastPosition = position;
            }
        }
    }
}
