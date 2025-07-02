using System.Collections;
using UnityEngine;

public class PatrolPoints : MonoBehaviour
{
    [SerializeField] private Transform[] points;

    private int currentPoint = 0;

    public Transform GetClosest(Vector3 position)
    {
        if (points.Length == 0)
        {
            return null;
        }

        Transform closest = null;
        float minDistanceSq = float.MaxValue;
        for (int i = 0; i < points.Length; ++i)
        {
            float distanceSq = (points[i].position - position).sqrMagnitude;
            if (distanceSq < minDistanceSq)
            { 
                minDistanceSq = distanceSq;
                closest = points[i];
                currentPoint = i;
            }
        }
        return closest;
    }

    public Transform GetCurrent()
    {
        if (points.Length == 0)
        {
            return null;
        }

        return points[currentPoint];
    }

    public Transform GetNext()
    {
        if (points.Length == 0)
        {
            return null;
        }

        int next = (currentPoint + 1) % points.Length;
        currentPoint = next;
        return points[next];
    }

    public Transform GetPrev()
    {
        if (points.Length == 0)
        {
            return null;
        }

        int prev = currentPoint - 1;
        if (prev < 0)
        {
            prev += points.Length;
        }
        currentPoint = prev;
        return points[prev];
    }
}
