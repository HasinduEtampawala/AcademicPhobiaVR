// ============================================================
// FILE: VRUIPointer.cs
// PURPOSE: Allows the VR controller to click UI buttons by
//          pointing the laser and pressing the trigger.
//          This is a fallback if OVRRaycaster is not available.
//
// AUTHOR: DulakshiniDharmarathne
// DATE CREATED: 21 June 2026
// ============================================================

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;

public class VRUIPointer : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Transform of the right hand/controller")]
    public Transform controllerTransform;

    [Header("Settings")]
    [Tooltip("Maximum distance the laser can reach")]
    public float maxDistance = 15f;

    [Header("Debug")]
    public bool debugMode = false;

    // Internal
    private Button lastHoveredButton = null;

    void Update()
    {
        if (controllerTransform == null) return;

        // Cast a ray from the controller
        Ray ray = new Ray(controllerTransform.position, controllerTransform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, maxDistance))
        {
            // Check if we hit a Button
            Button button = hit.collider.GetComponent<Button>();
            if (button == null)
            {
                // Also check parent (buttons sometimes have child colliders)
                button = hit.collider.GetComponentInParent<Button>();
            }

            if (button != null)
            {
                lastHoveredButton = button;

                // Check for trigger press
                if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.RTouch))
                {
                    if (debugMode) Debug.Log("VRUIPointer: Clicked button: " + button.name);
                    button.onClick.Invoke();
                }
            }
            else
            {
                lastHoveredButton = null;
            }
        }
        else
        {
            lastHoveredButton = null;
        }
    }
}