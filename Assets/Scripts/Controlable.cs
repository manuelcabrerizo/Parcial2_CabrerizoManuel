using UnityEngine;
public class Controlable : MonoBehaviour
{
    private Controlable controler = null;
    EntityMovement movement = null;

    private Rigidbody body;

    private void Awake()
    {
        body = GetComponent<Rigidbody>();
        OnAwaken();
    }

    private void OnDestroy()
    {
        OnDestroyed();
    }

    private void Update()
    {
        if (controler == null)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            BreakFree();
            controler = null;
        }
    }

    protected virtual void OnAwaken() {}

    protected virtual void OnDestroyed() {}

    protected virtual void OnControlEnter()
    {
        body.useGravity = false;
    }
    
    protected virtual void OnControlExit()
    { 
        body.useGravity = true;
    }

    public void Control(Controlable controler)
    {
        movement = gameObject.AddComponent<EntityMovement>();

        if (controler != null)
        {
            this.controler = controler;
            this.controler.OnControlExit();
        }

        OnControlEnter();
    }

    public void BreakFree()
    {
        OnControlExit();
        Destroy(movement);
        controler.OnControlEnter();
        controler.gameObject.AddComponent<EntityMovement>();
    }
}
