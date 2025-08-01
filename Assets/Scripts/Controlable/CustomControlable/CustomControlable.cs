using UnityEngine;

public abstract class CustomControlable : MonoBehaviour
{
    protected Animator animator = null;
    private void Start()
    {
        TryGetComponent(out animator);
        SetAnimatorInitialValues();
        OnStart();
    }

    private void OnEnable()
    {
        SetAnimatorInitialValues();
    }

    private void SetAnimatorInitialValues()
    {
        
        if (animator)
        {
            animator.SetBool("IsGrounded", true);
        }
    }

    public abstract void Initialize(Controlable controlable);
    protected virtual void OnStart() { }
}
