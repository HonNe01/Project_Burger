using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class Topping : MonoBehaviour
{
    public Ingredient ingredientType;
    public Transform spawnTransform;

    private XRGrabInteractable grabInteractable;
    private bool hasSpawned = false;

    void Awake()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();

        grabInteractable.selectEntered.AddListener(OnGrabbed);
        grabInteractable.selectExited.AddListener(OnReleased);
    }

    void OeDestroy()
    {
        grabInteractable.selectEntered.RemoveListener(OnGrabbed);
        grabInteractable.selectExited.RemoveListener(OnReleased);
    }

    private void OnGrabbed(SelectEnterEventArgs args)
    {
        var rb = GetComponent<Rigidbody>();
        if (rb != null)
            rb.useGravity = false;
    }

    private void OnReleased(SelectExitEventArgs args)
    {
        Plate plate = FindObjectOfType<Plate>();
        if (plate == null) return;

        var rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.useGravity = true;

            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        if (plate.IsPositionOnPlate(transform.position))
        {
            plate.AddTopping(gameObject, ingredientType);
        }
    }
}
