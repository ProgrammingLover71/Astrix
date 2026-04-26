using UnityEngine;

public class Universe : MonoBehaviour
{
    // The gravitational constant, used in Newton's law of universal gravity, orbital velocity calculation, and mass calculation
    public static float gravitationalConstant = 6.674e-11f;

    // Shrinks the masses by this much to make everything more realistic, given Unity's FP limits (thx for not using doubles)
    public static float massNormalizationConstant = 1e6f;
    public float TimeStep = 1f;

    public CelestialBody[] celestialBodies;
    public CelestialBody centerCelestial;
    PlayerMovement player;


    void Awake()
    {
        celestialBodies = FindObjectsByType<CelestialBody>(FindObjectsInactive.Exclude);
        player = FindObjectsByType<PlayerMovement>()[0];

        centerCelestial = GameObject.FindGameObjectWithTag("CenterCelestial").GetComponent<CelestialBody>();

        // Initialize all objects, starting with the center one
        centerCelestial.Initialize();

        foreach (var body in celestialBodies)
            if (body != centerCelestial) body.Initialize();
    }


    void Update()
    {
        // Update every single body's position
        foreach (var body in celestialBodies)
            body.UpdatePosition(TimeStep);
        
        // And then their velocities
        foreach (var body in celestialBodies)
            body.UpdateVelocity(TimeStep);

        // And finally, the player
        player.UpdateVelocity(TimeStep);

        foreach (var body in celestialBodies)
            player.UpdateGravity(TimeStep, body);
            
        player.UpdatePosition(TimeStep);
    }
}
