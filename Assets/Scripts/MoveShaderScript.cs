using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MoveShaderScript : MonoBehaviour
{

    public Material material;
    public GameObject camera;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        material.SetVector("_CameraLocation", camera.transform.position);
    }
}
