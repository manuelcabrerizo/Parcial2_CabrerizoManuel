using UnityEngine;

[CreateAssetMenu(fileName = "PlayerDataSO", menuName = "PlayerDataSO/Data", order = 1)]

public class ControlableDataSO : ScriptableObject
{
    public float walkSpeed = 50;
    public float fowardWalkSpeed = 30;
    public float fowardWalkMaxVelocity = 14;
    public float flySpeed = 30;
    public float flyMaxVelocity = 14;
    
    public float fallHorizontalSpeed = 15;
    public float fallMaxHorizontalVel = 4.5f;

    public float normalDrag = 5;
    public float flyDrag = 2.5f;
    public float fallDrag = 0.0f;

    public float jumpForce = 8;
    public float hightJumpForce = 12;
}
