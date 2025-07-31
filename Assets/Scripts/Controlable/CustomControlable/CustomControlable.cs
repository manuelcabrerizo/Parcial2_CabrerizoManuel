using UnityEngine;

public abstract class CustomControlable : MonoBehaviour
{
    private Animator animator = null;
    private void Start()
    {
        TryGetComponent(out animator);
        SetAnimatorInitialValues();
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
}
