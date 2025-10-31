using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class Topping : MonoBehaviour
{
    [Header("재료 타입")]
    public Ingredient ingredientType;

    [Header("리스폰 설정")]
    public GameObject toppingPrefab;

    [Header("리스폰 소스 여부")]
    public bool isRespawnSource = true;

    [Header("리스폰 감지 거리")]
    [Tooltip("원래 자리에서 이 거리 이상 벗어나면 새 오브젝트 리스폰")]
    public float respawnThreshold = 0.2f;

    private XRGrabInteractable grabInteractable;
    private Vector3 initialPosition;
    private Quaternion initialRotation;

    private bool hasRespawned = false; // 중복 리스폰 방지용

    [HideInInspector]
    public Plate CurrentPlate { get; set; }
    public bool isPlate = false;
    Plate plate;

    void Awake()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
        if (grabInteractable != null)
        {
            grabInteractable.selectEntered.AddListener(OnGrabbed);
            grabInteractable.selectExited.AddListener(OnReleased);
        }

        initialPosition = transform.position;
        initialRotation = transform.rotation;
    }

    void Start()
    {
        plate = FindObjectOfType<Plate>();
    }

    void OnDestroy()
    {
        if (grabInteractable != null)
        {
            grabInteractable.selectEntered.RemoveListener(OnGrabbed);
            grabInteractable.selectExited.RemoveListener(OnReleased);
        }
    }

    void Update()
    {
        // 리스폰 소스일 때, 원래 자리에서 멀어지면 새 오브젝트 생성
        if (isRespawnSource && !hasRespawned)
        {
            float distance = Vector3.Distance(transform.position, initialPosition);
            if (distance > respawnThreshold)
            {
                SpawnRespawnCopy();
                hasRespawned = true;
            }
        }

        if (!isPlate && plate.IsPositionOnPlate(transform.position))
        {
            isPlate = true;
            plate.AddTopping(gameObject);
        }
    }

    // Plate에서 제거만 담당 (리스폰 X)
    private void OnGrabbed(SelectEnterEventArgs args)
    {
        if (CurrentPlate != null)
        {
            CurrentPlate.RemoveTopping(gameObject);
            CurrentPlate = null;
        }
    }

    private void OnReleased(SelectExitEventArgs args)
    {
        if (plate == null) return;

        if (plate.IsPositionOnPlate(transform.position))
        {
            plate.AddTopping(gameObject);
        }
        else
        {
            Rigidbody rb = GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = false;
                rb.useGravity = true;
            }
        }
    }

    private void SpawnRespawnCopy()
    {
        if (toppingPrefab == null) return;

        GameObject newTopping = Instantiate(toppingPrefab, initialPosition, initialRotation);
        Topping script = newTopping.GetComponent<Topping>();
        if (script != null)
        {
            script.isRespawnSource = true; // 새로 생긴 애가 원본 역할 유지
        }
    }
}
