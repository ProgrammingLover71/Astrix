using TMPro;
using UnityEngine;

public class UI : MonoBehaviour
{
    public TMP_Text VelocityText;
    public TMP_Text RelVelocityTxt;
    public string Target;

    PlayerMovement player;


    void Awake()
    {
        // Find the player
        player = FindObjectsByType<PlayerMovement>()[0];
    }


    void Update()
    {
        // Update the velocity text
        float speed = player.currentVelocity.magnitude;
        VelocityText.text = $"Velocity: {speed:F2} m/s";

        // Update the relative velocity text (here w/ Fust)
        Vector3 fustVel = GameObject.Find(Target).GetComponent<CelestialBody>().currentVelocity;
        Vector3 relVelocity = player.currentVelocity - fustVel;
        
        float relSpeed = relVelocity.magnitude;
        RelVelocityTxt.text = $"Relative Velocity ({Target}): {relSpeed:F2} m/s";
    }
}
