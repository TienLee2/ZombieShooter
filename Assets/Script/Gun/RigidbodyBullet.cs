using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class RigidbodyBullet : MonoBehaviour
{
    private Rigidbody Rigidbody;


    [field: SerializeField]
    public Vector3 SpawnLocation
    {
        get; private set;
    }


    private void Awake()
    {
        Rigidbody = GetComponent<Rigidbody>();
    }

    public void Spawn(Vector3 SpawnForce)
    {
        SpawnLocation = transform.position;
        transform.forward = SpawnForce.normalized;
        Rigidbody.AddForce(SpawnForce);
        StartCoroutine(DelayedDisable(2));
    }

    private IEnumerator DelayedDisable(float Time)
    {
        yield return new WaitForSeconds(Time);
    }



    private void OnDisable()
    {
        StopAllCoroutines();
        Rigidbody.velocity = Vector3.zero;
        Rigidbody.angularVelocity = Vector3.zero;
    }
}
