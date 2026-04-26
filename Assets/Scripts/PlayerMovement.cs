using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    public float Acceleration = 100f;
    public float MaxSpeed = 1e8f;
    public float RotationSpeed = 10f;

    public Vector3 currentVelocity;
    Vector3 currentAcceleration;
    Vector3 currentRotation;

    Transform cameraTransform;
    Rigidbody thisRigidbody;

    void Start()
    {
        currentVelocity = new Vector3(0, 0, 0);
        currentAcceleration = new Vector3(0, 0, 0);

        cameraTransform = Camera.main.transform;

        thisRigidbody = GetComponent<Rigidbody>();
        
        // Rigidbody initialization
        thisRigidbody.useGravity = false;
        thisRigidbody.angularDamping = 0f;
        thisRigidbody.freezeRotation = true;
    }

    
    /// <summary>
    /// Updates the player's velocity and rotation according to key input.
    /// </summary>
    public void UpdateVelocity(float timeStep)
    {
        Keyboard keyboard = Keyboard.current;

        Vector3 camRight = cameraTransform.right;
        Vector3 camForward = cameraTransform.forward;
        Vector3 camUp = cameraTransform.up;

        // Set camRight.y to 0 to prevent wrong moving.
        camRight.y = 0f;

        // Normalize the camera vectors so we only keep the direction
        camRight.Normalize();
        camForward.Normalize();
        camUp.Normalize();

        currentAcceleration = new Vector3(0, 0, 0);
        currentRotation = new Vector3(0, 0, 0);

        Vector3 yRotation = new Vector3(0, RotationSpeed, 0);

        if (keyboard.wKey.isPressed)
            currentAcceleration += camForward;
        
        if (keyboard.sKey.isPressed)
            currentAcceleration -= camForward;

        if (keyboard.dKey.isPressed)
            currentAcceleration += camRight;

        if (keyboard.aKey.isPressed)
            currentAcceleration -= camRight;

        if (keyboard.eKey.isPressed)
            currentAcceleration += camUp;

        if (keyboard.qKey.isPressed)
            currentAcceleration -= camUp;
        
        if (keyboard.leftArrowKey.isPressed)
            currentRotation -= yRotation;

        if (keyboard.rightArrowKey.isPressed)
            currentRotation += yRotation;
        
        currentAcceleration *= Acceleration;
    }


    /// <summary>
    /// Updates the player's velocity according to gravity.
    /// </summary>
    public void UpdateGravity(float timeStep, CelestialBody body)
    {
        if (body == null) return;
        
        Rigidbody otherRigidbody = body.GetComponent<Rigidbody>();
        Vector3 positionDiff = otherRigidbody.position - thisRigidbody.position;

        float sqrDistance = positionDiff.sqrMagnitude;

        Vector3 forceDirection = positionDiff.normalized;
        Vector3 force = forceDirection * Universe.gravitationalConstant * body.Mass / sqrDistance * Universe.massNormalizationConstant;
        currentAcceleration += force;

        currentVelocity += currentAcceleration * timeStep * Time.deltaTime;

        // Prevent the velocity from exceeding the max limit
        if (currentVelocity.sqrMagnitude >= MaxSpeed * MaxSpeed)
        {
            currentVelocity.Normalize();
            currentVelocity *= MaxSpeed;
        }
    }


    /// <summary>
    /// Updates the player's position based on the current velocity.
    /// </summary>
    public void UpdatePosition(float timeStep)
    {
        thisRigidbody.position += currentVelocity * timeStep * Time.deltaTime;

        // Because you apparently multiply quaternions to combine rotations
        thisRigidbody.rotation *= Quaternion.Euler(currentRotation * Time.deltaTime);
    }
}