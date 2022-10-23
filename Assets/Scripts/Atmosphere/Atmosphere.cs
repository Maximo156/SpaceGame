using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Atmosphere : MonoBehaviour
{
    public float atmoRadius;
    float radius;
    public Material atmoMat;
    Material useThis;
    GameObject atmo;

    public bool update = true;
    // Start is called before the first frame update
    void Start()
    {
        update = false;

        if (Camera.main.depthTextureMode == DepthTextureMode.None)
            Camera.main.depthTextureMode = DepthTextureMode.Depth;

        DestroyImmediate(atmo);
        useThis = new Material(atmoMat);
        radius = transform.GetComponent<PlanetLOD>().radius;
        atmo = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        atmo.transform.localScale = (radius + atmoRadius) * 2 * Vector3.one;
        atmo.transform.parent = transform;
        atmo.transform.position = transform.position;
        atmo.name = "Atmosphere";
        atmo.GetComponent<MeshRenderer>().sharedMaterial = useThis;
        DestroyImmediate(atmo.GetComponent<SphereCollider>());
    }

    private void Update()
    {
        if (!Application.isPlaying && update)
        {
            var t = transform.Find("Atmosphere");
            if (t != null) atmo = t.gameObject;
            update = false;

            if (Camera.main.depthTextureMode == DepthTextureMode.None)
                Camera.main.depthTextureMode = DepthTextureMode.Depth;

            DestroyImmediate(atmo);
            useThis = new Material(atmoMat);
            radius = transform.GetComponent<PlanetLOD>().radius;
            atmo = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            atmo.transform.localScale = (radius + atmoRadius) * 2 * Vector3.one;
            atmo.transform.parent = transform;
            atmo.transform.position = transform.position;
            atmo.name = "Atmosphere";
            atmo.GetComponent<MeshRenderer>().sharedMaterial = useThis;
            DestroyImmediate(atmo.GetComponent<SphereCollider>());

            useThis.SetVector("FogParam", new Vector4(transform.position.x, transform.position.y, transform.position.z, (atmo.transform.lossyScale.x + atmo.transform.lossyScale.y + atmo.transform.lossyScale.z) / 6));
        }
    }

    private void FixedUpdate()
    {
        useThis.SetVector("FogParam", new Vector4(transform.position.x, transform.position.y, transform.position.z, (atmo.transform.lossyScale.x + atmo.transform.lossyScale.y + atmo.transform.lossyScale.z) / 6));
    }


}
