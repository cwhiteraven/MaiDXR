using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class RayManager : MonoBehaviour
{
    public bool RaySwitch = true;
    public float Distance = 0.4f;
    UnityEngine.XR.Interaction.Toolkit.Interactors.XRRayInteractor interactor;
    UnityEngine.XR.Interaction.Toolkit.Interactors.Visuals.XRInteractorLineVisual lineVisual;
    LineRenderer lineRenderer;
    void Start()
    {
        interactor = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactors.XRRayInteractor>();
        lineVisual = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactors.Visuals.XRInteractorLineVisual>();
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
