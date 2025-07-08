using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Book : MonoBehaviour
{
    private void Update()
    {
        transform.Rotate(0, Time.deltaTime*50.0f, 0);
    }
}
