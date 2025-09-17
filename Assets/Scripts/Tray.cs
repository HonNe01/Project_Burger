using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class Tray : XRBaseInteractable
{
    [Header("Topping Prefab")]
    public GameObject ingredientPrefab;
    private Transform spawnTransform;

    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        base.OnSelectEntered(args);

        if (ingredientPrefab == null) return;

        // 재료 소환
        GameObject inst = Instantiate(ingredientPrefab, spawnTransform.position, Quaternion.identity);

        var grab = inst.GetComponent<XRGrabInteractable>();
        IXRSelectInteractable interactable = grab;
        var interactor = args.interactableObject as XRBaseInteractor;
        if (interactor != null)
            interactor.StartManualInteraction(interactable);

        args.interactableObject = null;
    }
}
