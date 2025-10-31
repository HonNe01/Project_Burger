using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;


[RequireComponent(typeof(XRSimpleInteractable))]
public class ServingButton : MonoBehaviour
{
    public Plate plate;
    public float cooldown = 0.5f;
    private bool locked;


    public void OnButton(SelectEnterEventArgs args)
    {
        plate.SetController(args.interactableObject as XRBaseInputInteractor);

        TryServe();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) TryServe();
    }

    private void TryServe()
    {
        if (locked) return;
        locked = true;

        if (OrderManager.instance.GetCurrentOrder().Count > 0)
        {
            plate.CompleteOrder();
        }

        Invoke(nameof(Unlock), cooldown);
    }

    private void Unlock() => locked = false;
}
