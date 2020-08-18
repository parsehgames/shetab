using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AutoRotate : MonoBehaviour
{
    [SerializeField] private Vector3 rotation = Vector3.zero;

    private void Update()
    {
        transform.Rotate(rotation.x * Time.deltaTime * 50f, rotation.y * Time.deltaTime * 50f, rotation.z * Time.deltaTime * 50f, Space.Self);
    }
}
