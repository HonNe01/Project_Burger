using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class Topping : MonoBehaviour
{
    public Ingredient ingredientType;
    // public Transform spawnTransform; // 리스폰 로직이 없다면 이 줄은 필요 없습니다.

    private XRGrabInteractable grabInteractable;
    // private bool hasSpawned = false; // 리스폰 로직이 없다면 이 줄은 필요 없습니다.

    void Awake()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();

        // Listener 추가는 Awake나 Start에서 한 번만 합니다.
        grabInteractable.selectEntered.AddListener(OnGrabbed);
        grabInteractable.selectExited.AddListener(OnReleased);
    }

    // 오브젝트가 파괴될 때 Listener를 제거하여 메모리 누수를 방지합니다.
    void OnDestroy()
    {
        grabInteractable.selectEntered.RemoveListener(OnGrabbed);
        grabInteractable.selectExited.RemoveListener(OnReleased);
    }

    private void OnGrabbed(SelectEnterEventArgs args)
    {
        // 잡았을 때 특별한 동작이 필요 없다면 비워두거나,
        // Rigidbody의 isKinematic을 true로 설정하는 것도 좋은 방법입니다.
    }

    private void OnReleased(SelectExitEventArgs args)
    {
        Plate plate = FindObjectOfType<Plate>();

        // 1. 접시를 먼저 찾고, 접시가 없다면 아무것도 하지 않습니다.
        if (plate == null) return;

        // 2. 재료가 접시 위에 놓였는지 먼저 확인합니다.
        if (plate.IsPositionOnPlate(transform.position))
        {
            // 3. 성공적으로 놓였다면, AddTopping 함수를 호출합니다.
            // AddTopping 함수 안에서 Rigidbody를 제어하므로 여기서는 아무것도 할 필요가 없습니다.
            plate.AddTopping(gameObject, ingredientType);
        }
        else
        {
            // 4. 접시 위가 아니라면, 중력을 다시 켜서 바닥에 떨어지게 합니다.
            Rigidbody rb = GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = false; // isKinematic을 사용했다면 false로 변경
                rb.useGravity = true;

                // XR Interaction Toolkit이 속도를 제어하므로 굳이 0으로 만들 필요는 없습니다.
                // rb.velocity = Vector3.zero;
                // rb.angularVelocity = Vector3.zero;
            }
        }
    }
}
