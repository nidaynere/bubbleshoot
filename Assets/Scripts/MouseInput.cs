using UnityEngine;

public class MouseInput
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
    private bool lastDown;
    #endregion

    #region events
    private InputEvent OnHoldAndMove;
    private InputEvent OnDown;
    private InputEvent OnUp;
    #endregion

    public void Update()
    {
        Vector2 position = Input.mousePosition;
        bool isMouseDown = Input.GetMouseButtonDown(0);
        if (isMouseDown)
        {
            if (!lastDown)
            {
                lastPosition = position;
            }

            OnDown?.Invoke(position);
        }


        bool isMouseUp = Input.GetMouseButtonUp(0);
        if (isMouseUp)
            OnUp?.Invoke(position);

        lastDown = isMouseDown;

        if (Input.GetMouseButton (0))
        {
            if (position != lastPosition)
            {
                OnHoldAndMove?.Invoke(position);
                lastPosition = position;
            }
        }
    }
}
