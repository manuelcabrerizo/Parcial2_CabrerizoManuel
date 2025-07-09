using UnityEngine;

public class ControlableData
{
    public float mousePosX = 0.0f;
    public float mousePosY = 0.0f;

    public float xInput = 0.0f;
    public float yInput = 0.0f;

    public float smoothSpeed = 8.0f;
    public float smoothXInput = 0.0f;
    public float smoothYInput = 0.0f;


    public int jumpCount = 3;
    public int currentJumpDone = 0;

    public float moveDirLenSq = 0.0f;
    public bool isGrounded = false;
    public Rigidbody body = null;
    public CameraMovement cameraMovement = null;
    public Camera cam = null;
    public GameObject prevControlable = null;
    public Animator animator = null;
}
