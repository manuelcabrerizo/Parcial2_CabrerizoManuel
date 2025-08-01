using UnityEngine;

[CreateAssetMenu(fileName = "VolumeDataSO", menuName = "VolumeDataSO/Data", order = 1)]

public class VolumeDataSO : ScriptableObject
{
    public float Master = 0;
    public float Music = 0;
    public float Sfx = 0;
    public float UI = 0;
}
