using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrailController : MonoBehaviour
{

    public ParticleSystem ps;

    public void SetParticleColor(Color color)
    {
        Gradient gradient = new Gradient();
        var colorOverLifetime = ps.colorOverLifetime;
        gradient.SetKeys(
            new GradientColorKey[] { new GradientColorKey(color, 0.0f), new GradientColorKey(color, 1.0f) },
            new GradientAlphaKey[] { new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(0f, 1.0f) }
        );
        colorOverLifetime.color = new ParticleSystem.MinMaxGradient(gradient);

    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
