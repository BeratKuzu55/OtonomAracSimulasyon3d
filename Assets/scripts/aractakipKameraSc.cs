using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class aractakipKameraSc : MonoBehaviour
{
    // Start is called before the first frame update

    public float moveSmoothness;
    public float rotSmoothness;

    public Vector3 moveOffset;
    public Vector3 rotOffset;

    public Transform cartarget;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        fallowTarget();
    }

    private void fallowTarget()
    {
        handleMovement();
        handleRotation();
    }

    void handleMovement()
    {
        Vector3 targetPos = new Vector3();
        targetPos = cartarget.TransformPoint(moveOffset);

        transform.position = Vector3.Lerp(transform.position, targetPos, moveSmoothness * Time.deltaTime);
    }

    void handleRotation()
    {
        var direction = cartarget.position - transform.position;
        var rotation = new Quaternion();

        rotation = Quaternion.LookRotation(direction + rotOffset, Vector3.up);

        transform.rotation = Quaternion.Lerp(transform.rotation, rotation, rotSmoothness * Time.deltaTime);
    }

}
