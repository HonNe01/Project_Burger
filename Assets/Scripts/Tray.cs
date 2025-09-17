using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class Tray : MonoBehaviour
{
    public GameObject ingredientPrefab;
    private XRGrabInteractable trayGrab;

    void Awake()
    {
        trayGrab = GetComponent<XRGrabInteractable>();

        if (trayGrab == null)
        {
            trayGrab = gameObject.AddComponent<XRGrabInteractable>();
        }

        trayGrab.selectEntered.AddListener(OnGrabbed);
    }

    private void OeDestroy()
    {
        if (trayGrab != null)
            trayGrab.selectEntered.RemoveListener(OnGrabbed);
    }

    private void OnGrabbed(SelectEnterEventArgs args)
    {
        // 1) 손 가져오기
        var xrInteractor = args.interactableObject as XRBaseInteractor;

        if (xrInteractor == null)
        {
            Debug.LogWarning("TraySlot: interactorObject is not XRBaseInteractor");
            return;
        }
        if (ingredientPrefab == null)
        {
            Debug.LogWarning("TraySlot: ingredientPrefab not set");
            return;
        }

        // 2) 재료 생성
        GameObject ingredient = Instantiate(ingredientPrefab, transform.position + Vector3.up * 0.1f, Quaternion.identity);

        // 3) 그랩 설정
        XRGrabInteractable ingredientGrab = ingredient.GetComponent<XRGrabInteractable>();
        if (ingredientGrab == null)
            ingredientGrab = ingredient.AddComponent<XRGrabInteractable>();

        Rigidbody rb = ingredient.GetComponent<Rigidbody>();
        if (rb == null)
            rb = ingredient.AddComponent<Rigidbody>();

        Collider coll = ingredient.GetComponent<Collider>();
        if (coll = null)
        {
            var sphere = ingredient.AddComponent<BoxCollider>();
            sphere.isTrigger = false;
        }


        IXRSelectInteractable interactable = ingredientGrab;
        xrInteractor.StartManualInteraction(interactable);

        args.interactableObject.transform.localPosition = transform.localPosition;
        xrInteractor.EndManualInteraction();
    }
}
