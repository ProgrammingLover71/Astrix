using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CelestialBody : MonoBehaviour
{
    public float SurfaceGravity;
    public float Radius;
    public Vector3 InitialVelocity;
    public CelestialBody Parent;
    public Color Color;

    public bool AutoOrbit;
    public bool HasSurface = true;


    public Vector3 currentVelocity;

    Rigidbody parentRigidbody;
    Rigidbody thisRigidbody;

    public float Mass;

    public Transform soiTransform;


    /// <summary>
    /// Initialize this celestial body.
    /// </summary>
    public void Initialize()
    {
        ApplyColors();

        thisRigidbody = GetComponent<Rigidbody>();

        // SOI visualization: Rs = sma * (m / M)^0.4
        // SMA (semi-major axis) is for now equal to transform.position.x
        if (Parent != null && Parent != this)
        {
            float soiRadius = transform.position.x * Mathf.Pow(Mass / Parent.Mass, 0.4f);
            float soiScale = soiRadius;
            soiTransform.localScale = new Vector3(soiScale, soiScale, soiScale);
        }

        // Rigidbody initialization
        thisRigidbody.useGravity = false;
        thisRigidbody.angularDamping = 0f;
        thisRigidbody.mass = Mass;
        thisRigidbody.detectCollisions = HasSurface;
        
        // Transform initialization
        transform.localScale = new Vector3(Radius, Radius, Radius);

        // Compute mass: Mass = surfaceGravity * Radius^2 / (G * MNC)
        Mass = SurfaceGravity * Radius * Radius / (Universe.gravitationalConstant * Universe.massNormalizationConstant);

        // Try to get the parent rigidbody after initializing this one
        if (Parent == null || Parent == this) return;
        parentRigidbody = Parent.GetComponent<Rigidbody>();

        // Set the velocity to (0, 0, sqrt(G * Mass / Radius)) if Auto Orbit is on
        if (AutoOrbit) 
        {
            float distance = (thisRigidbody.position - parentRigidbody.position).magnitude;
            InitialVelocity = new Vector3(0, 0, Mathf.Sqrt(Universe.gravitationalConstant * Parent.Mass / distance));
        }

        // Initialize velocity
        currentVelocity = InitialVelocity + Parent.currentVelocity;
    }

    
    /// <summary>
    /// Apply the initial colors to this planet's material.
    /// </summary>
    public void ApplyColors()
    {
        Renderer renderer = GetComponent<Renderer>();

        // Set the color
        renderer.material.color = Color;
    }


    /// <summary>
    /// Update the velocity of this celestial body.
    /// </summary>
    public void UpdateVelocity(float timeStep)
    {
        if (Parent == null || Parent == this) return;
        
        Vector3 positionDiff = parentRigidbody.position - thisRigidbody.position;

        float sqrDistance = positionDiff.sqrMagnitude;

        Vector3 forceDirection = positionDiff.normalized;
        Vector3 force = forceDirection * Universe.gravitationalConstant * Mass * Parent.Mass / sqrDistance;
        Vector3 acceleration = force / Mass;

        currentVelocity += acceleration * timeStep;
    }


    /// <summary>
    /// Update the position of this celestial body.
    /// </summary>
    public void UpdatePosition(float timeStep)
    {
        if (Parent == null || Parent == this) return;

        thisRigidbody.position += currentVelocity * timeStep;
    }
}
