public class ControlableState : State
{
    protected Controlable controlable;

    public ControlableState(Controlable controlable)
    {
        this.controlable = controlable;
    }
}
