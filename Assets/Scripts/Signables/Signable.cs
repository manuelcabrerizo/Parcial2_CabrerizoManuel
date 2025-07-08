using UnityEngine;

public abstract class Signable : MonoBehaviour, ISignable
{
    public abstract bool IsSignal();
}
