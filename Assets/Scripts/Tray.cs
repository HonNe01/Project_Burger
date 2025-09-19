using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class Tray : XRBaseInteractable
{
    [Header("Topping Prefab")]
    public GameObject ingredientPrefab;
    public Transform spawnTransform;

    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        // XRIT 기본 처리
        base.OnSelectEntered(args);

        if (ingredientPrefab == null || spawnTransform == null) return;

        // 어떤 손인지
        var interactor = args.interactableObject as XRBaseInteractor;
        if (interactor == null) return;

        // 트레이 놓기
        interactor.EndManualInteraction();

        // 재료 소환
        GameObject inst = Instantiate(ingredientPrefab, spawnTransform.position, Quaternion.identity);

        var grab = inst.GetComponent<XRGrabInteractable>();
        if (grab == null)
        {
            grab = inst.AddComponent<XRGrabInteractable>();
        }

        // 재료 손에 들기
        IXRSelectInteractable interactable = grab;
        interactor.StartManualInteraction(interactable);
    }
}
