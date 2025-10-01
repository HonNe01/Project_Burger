using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    public Transform target;

    void Start()
    {
        if (target == null && Camera.main != null)
        {
            target = Camera.main.transform;
        }
    }

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 dir = transform.position - target.position;
        dir.y = 0f;

        if (dir.sqrMagnitude > 0.001f)
        {
            transform.rotation = Quaternion.LookRotation(dir);
        }
    }
}
