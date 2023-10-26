using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class carController : MonoBehaviour
{
    // Start is called before the first frame update
    private const string Horizontal = "Horizontal";
    private const string Vertical = "Vertical";

    private float horizontal_input;
    private float vertical_input;
    private float currentSteerAngle;
    private float currentberakForce;
    private bool isBraking;


    [SerializeField] private float otospeed;
    [SerializeField] private float donusAcisi;
    [SerializeField] private float donmeBeklemeSuresi;
    [SerializeField] private float motorForce;
    [SerializeField] private float breakForce;
    [SerializeField] private float SteerAngle;
    [SerializeField] private float hidrolikSuresi;

    [SerializeField] private WheelCollider frontLeftWheelCollider;
    [SerializeField] private WheelCollider frontRightWheelCollider;
    [SerializeField] private WheelCollider backLeftWheelCollider;
    [SerializeField] private WheelCollider backRightWheelCollider;

    [SerializeField] private Transform frontRightWheelTransform;
    [SerializeField] private Transform frontLeftWheelTransform;
    [SerializeField] private Transform backRightWheelTransform;
    [SerializeField] private Transform backLeftWheelTransform;
     
    GameObject arabaDireksiyonu;
    float direksiyonAcisi = 0;

    public Camera carInCamera;
    public Camera carOutCamera;
    public Camera carFrontCamera;
    private void Start()
    {
        arabaDireksiyonu = GameObject.Find("sport_car_1_steering_wheel");
        donusAcisi = 3.0f;
        donmeBeklemeSuresi = 0.015f;
        hidrolikSuresi = 0.4f;
    }

    bool direksiyon = false;
    private void FixedUpdate()
    {
        GetInput();
        HandleMotor();
        HandleSteering();
        UpdateWheels();
        handleCamera();
        donusAcisiAyarla();
        hidrolikSuresiAyarla();
        aracDuzelt();
    }


    private void GetInput()
    {
        horizontal_input = Input.GetAxis(Horizontal);
        vertical_input = Input.GetAxis(Vertical);
        isBraking = Input.GetKey(KeyCode.Space);
    }

    private void HandleMotor()
    {
        frontLeftWheelCollider.motorTorque = vertical_input * motorForce;
        frontRightWheelCollider.motorTorque = vertical_input * motorForce;
        //GetComponent<Rigidbody>().AddForce(transform.forward * 20f);
        currentberakForce = isBraking ? breakForce : 0f;
        ApplyBreaking();
    }

    private void ApplyBreaking()
    {
        frontRightWheelCollider.brakeTorque = currentberakForce;
        frontLeftWheelCollider.brakeTorque = currentberakForce;
        backLeftWheelCollider.brakeTorque = currentberakForce;
        backRightWheelCollider.brakeTorque = currentberakForce;
    }

    private void HandleSteering()
    {
        if(horizontal_input != 0 && !direksiyon)
        {
            StartCoroutine(yonDegistir(horizontal_input));
        }
        else if (horizontal_input == 0 && SteerAngle != 0)
        {
            StartCoroutine(hidrolikCalistir());
        }
        currentSteerAngle = SteerAngle;
        frontLeftWheelCollider.steerAngle = currentSteerAngle;
        frontRightWheelCollider.steerAngle = currentSteerAngle;
    }

    private void UpdateWheels()
    {
        updateSingleWheel(frontRightWheelCollider, frontRightWheelTransform);
        updateSingleWheel(frontLeftWheelCollider, frontLeftWheelTransform);
        updateSingleWheel(backLeftWheelCollider, backLeftWheelTransform);
        updateSingleWheel(backRightWheelCollider, backRightWheelTransform);
    }

    private void updateSingleWheel(WheelCollider wheelCollider , Transform wheelTransform)
    {
        Vector3 pos;
        Quaternion rot;
        wheelCollider.GetWorldPose(out pos, out rot);
        wheelTransform.rotation = rot;
        wheelTransform.position = pos;
    }

    private void donusAcisiAyarla()
    {
        otospeed = gameObject.GetComponent<Rigidbody>().velocity.magnitude;

        float basespeed = 5.0f;
        if(otospeed > basespeed)
        {
            donusAcisi = 15 / (basespeed + 2 + ((otospeed / 3)));
        }
        else
        {
            donusAcisi = 2.5f;
        }
    }


    private void hidrolikSuresiAyarla()
    {
        otospeed = gameObject.GetComponent<Rigidbody>().velocity.magnitude;
        float basespeedForHidrolik = 5.0f;
        if (otospeed > basespeedForHidrolik)
        {
            hidrolikSuresi = 1 / (basespeedForHidrolik + ((otospeed / 1.5f)));
        }
        else
        {
            hidrolikSuresi = 0.4f;
        }

    }

    private IEnumerator yonDegistir(float yon)
    {
        direksiyon = true;
        print(SteerAngle);
        if(yon > 0 && SteerAngle <= 32.5f)
        {
            SteerAngle += donusAcisi;
            direksiyonAcisi += 20f;
        }
        else if(yon < 0 && SteerAngle >= -32.5)
        {
            SteerAngle -= donusAcisi;
            direksiyonAcisi -= 20f;
        }
        else
        {
            
        }
        yield return new WaitForSeconds(donmeBeklemeSuresi);

        //arabaDireksiyonu.transform.rotation = Quaternion.Euler(-30, -180, direksiyonAcisi);
        direksiyon = false;
    }

    private IEnumerator hidrolikCalistir()
    {
        SteerAngle = SteerAngle / 2;
        yield return new WaitForSeconds(hidrolikSuresi);
        SteerAngle = 0;
        //arabaDireksiyonu.transform.rotation = Quaternion.Euler( -30 , -180 , 0);
    }


    void handleCamera()
    {
        if (Input.GetKey(KeyCode.Alpha1))
        {
            enableCarOutCamera();
        }
        else if (Input.GetKey(KeyCode.Alpha2))
        {
            enableCarInCamera();
        }
        else if (Input.GetKey(KeyCode.Alpha3))
        {
            enableFrontCamera();
        }
    }

    public void enableCarInCamera()
    {
        carOutCamera.enabled = false;
        carFrontCamera.enabled = false;
        carInCamera.enabled = true;

    }

    public void enableCarOutCamera()
    {
        carInCamera.enabled = false;
        carFrontCamera.enabled = false;
        carOutCamera.enabled = true;
    }

    public void enableFrontCamera()
    {
        carInCamera.enabled = false;
        carOutCamera.enabled = false;
        carFrontCamera.enabled = true;
    }


    private void aracDuzelt()
    {
        if (Input.GetKey(KeyCode.U))
        {
            gameObject.transform.rotation = Quaternion.Euler( new Vector3(0, transform.rotation.y, 0));
        }
    }


}

