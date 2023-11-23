using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MoveShaderScript : MonoBehaviour
{

    public Material myceliumMaterial;
    public Material pathMaterial;
    public GameObject camera;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        myceliumMaterial.SetVector("_CameraLocation", camera.transform.position);
        pathMaterial.SetVector("_CameraLocation", camera.transform.position);

    }
}
