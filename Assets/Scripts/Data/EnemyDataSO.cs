using UnityEngine;

[CreateAssetMenu(fileName = "EnemyDataSO", menuName = "EnemyDataSO/Data", order = 1)]
public class EnemyDataSO : ScriptableObject
{
    public float attackRadio = 30;
    public float walkSpeed = 10;
    public float minSoundRadio = 1;
    public float maxSoundRadio = 20;
}
