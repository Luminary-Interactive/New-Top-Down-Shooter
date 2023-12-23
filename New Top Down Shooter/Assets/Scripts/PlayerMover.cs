using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMover : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float _moveSpeed;
    
    private Vector2 _input;

    private void FixedUpdate()
    {
        MovePlayer();
        
        void MovePlayer()
        {
            Vector3 movement = new Vector3(_input.x, 0f, _input.y) * _moveSpeed; // No need for deltaTime multiplication because this is a fixedupdate method
            transform.Translate(movement, Space.World);
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        _input = context.ReadValue<Vector2>();
        _input.Normalize();
    }
}