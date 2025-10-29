using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;


[RequireComponent(typeof(XRSimpleInteractable))]
public class ServingButton : MonoBehaviour
{
    public Plate plate;
    public float cooldown = 0.5f;

    private XRSimpleInteractable interactable;
    private bool locked;

    void Awake() 
    {
        interactable = GetComponent<XRSimpleInteractable>();
        interactable.activated.AddListener(OnActivated);
    }

    private void OnActivated(ActivateEventArgs args)
    {
        plate.SetController(args.interactableObject as XRBaseInputInteractor);

        TryServe();
    }

    private void TryServe() {
        if (locked || plate == null) return;
        locked = true;

        if (OrderManager.instance.GetCurrentOrder().Count > 0) {
            plate.CompleteOrder();
        }
        
        Invoke(nameof(Unlock), cooldown);
    }

    private void Unlock() => locked = false;
}
