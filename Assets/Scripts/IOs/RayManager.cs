using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class RayManager : MonoBehaviour
{
    public bool RaySwitch = true;
    public float Distance = 0.4f;
    XRRayInteractor interactor;
    XRInteractorLineVisual lineVisual;
    LineRenderer lineRenderer;
    void Start()
    {
        interactor = GetComponent<XRRayInteractor>();
        lineVisual = GetComponent<XRInteractorLineVisual>();
        lineRenderer = lineVisual.GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (gameObject.transform.position.z > Distance || !RaySwitch)
        {
            interactor.enabled = false;
            lineRenderer.enabled = false;
            lineVisual.enabled = false;
        }
        else
        {
            interactor.enabled = true;
            lineRenderer.enabled = true;
            lineVisual.enabled = true;
        }
    }
}
