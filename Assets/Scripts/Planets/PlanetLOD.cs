using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class PlanetLOD : MonoBehaviour
{
    public int radius;
    public bool HighQuality;
    public float HighQualityRange;
    bool settingChanged;
    Camera cam;
    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        if (settingChanged && !Application.isPlaying)
        {
            transform.GetChild(1).localScale = new Vector3(1, 1, 1) * radius * 2;
            if (HighQuality)
            {
                transform.GetChild(1).gameObject.SetActive(false);
                transform.GetChild(0).gameObject.SetActive(true);
            }
            else
            {
                transform.GetChild(0).gameObject.SetActive(false);
                transform.GetChild(1).gameObject.SetActive(true);
            }
        }
        if (Application.isPlaying)
        {
            if((cam.transform.position - transform.position).magnitude < HighQualityRange)
            {
                transform.GetChild(1).gameObject.SetActive(false);
                transform.GetChild(0).gameObject.SetActive(true);
            }
            else
            {
                transform.GetChild(0).gameObject.SetActive(false);
                transform.GetChild(1).gameObject.SetActive(true);
            }
        }
    }

    private void OnValidate()
    {
        settingChanged = true;
    }
}
