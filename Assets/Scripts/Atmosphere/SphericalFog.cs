using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class SphericalFog : MonoBehaviour
{
	public float atmoRadius;
	float pradius;
	protected MeshRenderer sphericalFogObject;
	public Material sphericalFogMaterial;
	public float scaleFactor = 1;

	public Color baseColor;
	public Color DenseColor;
	public float innerRatio;
	public float density;
	public float ColorFalloff;

	Transform sun;

	bool update;
	void Start()
	{
		sun = GameObject.Find("Sun").transform;
		if (!Application.isPlaying)
		{
			pradius = transform.parent.GetComponent<PlanetLOD>().radius;
			sphericalFogObject = gameObject.GetComponent<MeshRenderer>();

			if (sphericalFogObject == null)
				Debug.LogError("Volume Fog Object must have a MeshRenderer Component!");

			//Note: In forward lightning path, the depth texture is not automatically generated.
			if (Camera.main.depthTextureMode == DepthTextureMode.None)
				Camera.main.depthTextureMode = DepthTextureMode.Depth;

			sphericalFogObject.sharedMaterial = new Material(sphericalFogMaterial);
			sphericalFogObject.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
		}
	}

	void Update()
	{
		if (!Application.isPlaying)
        {
			float radius = (transform.lossyScale.x + transform.lossyScale.y + transform.lossyScale.z) / 6;
			sphericalFogObject.sharedMaterial.SetVector("FogParam", new Vector4(transform.position.x, transform.position.y, transform.position.z, radius * scaleFactor));
			sphericalFogObject.sharedMaterial.SetColor("_FogBaseColor", baseColor);
			sphericalFogObject.sharedMaterial.SetVector("sunPosition", sun.position);
			sphericalFogObject.sharedMaterial.SetFloat("planetRadius", pradius);

			if (update)
			{
				pradius = transform.parent.GetComponent<PlanetLOD>().radius;
				transform.localScale = (pradius + atmoRadius) * 2 * Vector3.one;
				//sphericalFogObject.sharedMaterial = new Material(sphericalFogMaterial);
				UpdateMaterial();
			}
			
		}
	} 

    private void FixedUpdate()
    {
		float radius = (transform.lossyScale.x + transform.lossyScale.y + transform.lossyScale.z) / 6;
		sphericalFogObject.sharedMaterial.SetVector("FogParam", new Vector4(transform.position.x, transform.position.y, transform.position.z, radius * scaleFactor));
		sphericalFogObject.sharedMaterial.SetVector("sunPosition", sun.position);
	}

    private void UpdateMaterial()
    {
		sphericalFogObject.sharedMaterial.SetColor("_FogBaseColor", baseColor);
		sphericalFogObject.sharedMaterial.SetColor("_FogDenseColor", DenseColor);
		sphericalFogObject.sharedMaterial.SetFloat("_InnerRatio", innerRatio);
		sphericalFogObject.sharedMaterial.SetFloat("_Density", density);
		sphericalFogObject.sharedMaterial.SetFloat("_ColorFalloff", ColorFalloff);
	}

    private void OnValidate()
    {
		update = true;
    }
}
