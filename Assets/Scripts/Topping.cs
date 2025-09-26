using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class Topping : MonoBehaviour
{
    public Ingredient ingredientType;

    [Header("리스폰 설정")]
    [Tooltip("스폰될 원본 프리팹 (자기 자신 프리팹 연결)")]
    public GameObject toppingPrefab;

    // 이 재료가 현재 어느 접시 위에 있는지 추적합니다.
    public Plate CurrentPlate { get; set; } = null;

    private XRGrabInteractable grabInteractable;
    private bool hasSpawnedNext = false;
    private Vector3 initialPosition;
    private Quaternion initialRotation;

    void Awake()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
        grabInteractable.selectEntered.AddListener(OnGrabbed);
        grabInteractable.selectExited.AddListener(OnReleased);
        initialPosition = transform.position;
        initialRotation = transform.rotation;
    }

    void OnDestroy()
    {
        grabInteractable.selectEntered.RemoveListener(OnGrabbed);
        grabInteractable.selectExited.RemoveListener(OnReleased);
    }

    private void OnGrabbed(SelectEnterEventArgs args)
    {
        // 1. 만약 이 재료가 특정 접시 위에 있었다면,
        if (CurrentPlate != null)
        {
            // 그 접시에서 이 재료를 제거하라고 알립니다.
            CurrentPlate.RemoveTopping(gameObject);
            CurrentPlate = null; // 이제 더 이상 그 접시 위가 아닙니다.
        }

        // 2. 잡았으니 물리 효과를 다시 켜줍니다.
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
        }

        // 3. 리스폰 로직
        if (!hasSpawnedNext && toppingPrefab != null)
        {
            hasSpawnedNext = true;
            Instantiate(toppingPrefab, initialPosition, initialRotation);
        }
    }

    private void OnReleased(SelectExitEventArgs args)
    {
        // 손에서 놓았을 때, 주변에 Plate가 있는지 찾아봅니다.
        Plate plate = FindObjectOfType<Plate>();
        if (plate != null && plate.IsPositionOnPlate(transform.position))
        {
            // 만약 접시의 감지 영역 안이라면, 그 접시에 이 재료를 추가하라고 알립니다.
            plate.AddTopping(gameObject);
        }
        // 접시 위가 아니라면, 그냥 중력에 의해 떨어집니다 (별도 처리 필요 없음).
    }
}

