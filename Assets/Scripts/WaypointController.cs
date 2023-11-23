using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class WaypointController : MonoBehaviour
{
    public int type;
    public WaypointManager manager;
    public VisualEffect effect;


    public void SetColor(Color color)
    {
        effect.SetVector4("Color", color);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        manager.Trigger(type);
        Destroy(gameObject);
    }
}
