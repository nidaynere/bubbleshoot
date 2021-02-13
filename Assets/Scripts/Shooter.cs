using UnityEngine;
using System.Collections;

namespace Assets.Scripts
{
    public class Shooter : MonoBehaviour
    {
        #region events
        public delegate void ShootEvent(Vector2 direction);
        private ShootEvent OnShoot;
        public void RegisterShootEvent(ShootEvent shootEvent)
        {
            OnShoot += shootEvent;
        }
        public void UnRegisterShootEvent(ShootEvent shootEvent)
        {
            OnShoot -= shootEvent;
        }
        #endregion

        #region serialized variables
        [SerializeField] private AimArrow aimArrow;

        /// <summary>
        /// Focus point will be 3D object, we will get the screenpos of it.
        /// </summary>
        [SerializeField] private Transform focusPoint;
        #endregion

        private MouseInput mouseInput;
        
        void Start()
        {
            mouseInput = new MouseInput();
            mouseInput.RegisterInput(OnInputHoldAndMove, OnInputDown, OnInputUp);
        }

        private void Update()
        {
            mouseInput.Update(); // keep input listener in loop
        }

        private Vector2 getAimDirection(ref Vector2 position)
        {
            Vector2 findFocusOn2D = Camera.main.WorldToScreenPoint(focusPoint.position);
            return findFocusOn2D - position;
        }

        private void OnInputHoldAndMove(Vector2 position)
        {
            Debug.Log("[Shooter] OnInputHoldAndMove, position => " + position);

            aimArrow.SetDirection(getAimDirection(ref position));
        }

        private void OnInputDown(Vector2 position)
        {
            Debug.Log("[Shooter] OnInputDown, position => " + position);

            aimArrow.SetVisual(true);
            OnInputHoldAndMove(position);
        }

        private void OnInputUp(Vector2 position)
        {
            Debug.Log("[Shooter] OnInputUp, position => " + position);

            aimArrow.SetVisual(false);
            OnShoot?.Invoke(getAimDirection (ref position));
        }
    }
}