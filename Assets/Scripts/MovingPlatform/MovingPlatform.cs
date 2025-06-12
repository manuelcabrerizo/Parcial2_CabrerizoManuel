using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [SerializeField] private List<Transform> positions;
    private int currentPosition;
    private bool isMoving;

    private void Awake()
    {
        currentPosition = 0;
        isMoving = false;
        transform.position = positions[currentPosition].position;
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }

    public void MoveFrom(Vector3 position)
    {
        if (isMoving)
        {
            return;
        }

        Transform curr = GetCurrentPoint();
        Transform prev = GetPrevPoint();
        Transform next = GetNextPoint();

        Vector3 spellDir = (curr.position - position).normalized;

        float nextProj = -1.0f;
        if (next)
        {
            Vector3 toNext = (next.position - curr.position).normalized;
            nextProj = Vector3.Dot(toNext, spellDir);
        }

        float prevProj = -1.0f;
        if (prev)
        {
            Vector3 toPrev = (prev.position - curr.position).normalized;
            prevProj = Vector3.Dot(toPrev, spellDir);
        }

        if (nextProj < 0.0f)
        {
            if (prev && prevProj >= 0.0f)
            {
                MoveToPrev();
            }
        }
        
        if (prevProj < 0.0f)
        {
            if (next && nextProj >= 0.0f)
            {
                MoveToNext();
            }
        }
        
        if(nextProj >= 0.0f && prevProj >= 0.0f)
        {
            if (nextProj > prevProj)
            {
                MoveToNext();
            }
            else
            {
                MoveToPrev();
            }
        }
    }

    private void MoveToNext()
    {
        StartCoroutine(MovePlatform(currentPosition + 1));
    }

    private void MoveToPrev()
    {
        StartCoroutine(MovePlatform(currentPosition - 1));
    }

    private Transform GetCurrentPoint()
    {
        return positions[currentPosition];
    }

    private Transform GetNextPoint()
    {
        if (currentPosition + 1 == positions.Count)
        {
            return null;
        }
        else
        {
            return positions[currentPosition + 1];
        }
    }

    private Transform GetPrevPoint()
    {
        if (currentPosition - 1 < 0)
        {
            return null;
        }
        else
        {
            return positions[currentPosition - 1];
        }
    }

    private IEnumerator MovePlatform(int newPosition)
    {
        isMoving = true;
        Vector3 a = positions[currentPosition].position;
        Vector3 b = positions[newPosition].position;

        float time = 0.0f;
        float distSq = (b - a).sqrMagnitude;
        while (distSq > 0.01f)
        {
            transform.position = Vector3.Lerp(a, b, time);
            distSq = (b - transform.position).sqrMagnitude;
            yield return new WaitForEndOfFrame();
            time += Time.deltaTime;
        }
        transform.position = b;
        currentPosition = newPosition;
        isMoving = false;
    }
}
