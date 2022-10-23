using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class LightManager : MonoBehaviour
{
    public bool update;
    [SerializeField] private LightSettings settings;
    Camera main;
    // Start is called before the first frame update
    void Start()
    {
        SetGameLayerRecursive(gameObject, 7);
        main = Camera.main;
    }

    private void Update()
    {
        if (!Application.isPlaying && update)
        {
            update = false;
            SetGameLayerRecursive(gameObject, 6);
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(Vector3.Distance(main.transform.position, transform.position) > settings.distance && gameObject.layer != 7)
        {
             SetGameLayerRecursive(gameObject, 7);
        }
        else if(gameObject.layer != 6)
        {
            SetGameLayerRecursive(gameObject, 6);
        }
    }

    private void SetGameLayerRecursive(GameObject _go, int _layer)
    {
        _go.layer = _layer;
        if (_layer == 6 && _go.tag == "Terrain") _go.layer = 8;
        foreach (Transform child in _go.transform)
        {
              SetGameLayerRecursive(child.gameObject, _layer);
        }
    }
}
