using System.Collections;
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
        //if (Input.GetKeyDown(KeyCode.Space)) TryServe();
    }

    private void TryServe()
    {
        if (locked) return;
        SoundManager.instance.PlaySFX(SoundManager.SFX.Bell);
        locked = true;

        if (OrderManager.instance.GetCurrentOrder().Count > 0)
        {
            plate.CompleteOrder();
        }

        StartCoroutine(Co_Unlock(cooldown));
    }

    private IEnumerator Co_Unlock(float time) 
    {
        yield return new WaitForSeconds(time);

        locked = false;
    }
}
