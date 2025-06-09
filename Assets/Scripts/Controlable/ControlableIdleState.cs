public class ControlableIdleState : ControlableState
{
    public ControlableIdleState(Controlable controlable)
        : base(controlable) { }

    public override void OnEnter()
    {
        ControlableData data = controlable.Data;
        if (data.animator != null)
        {
            data.animator.SetFloat("VelocityZ", 0);
            data.animator.SetFloat("VelocityX", 0);
        }
    }
}
