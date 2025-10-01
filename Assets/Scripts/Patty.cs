using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class Patty : MonoBehaviour
{
    public enum CookingState { Raw, Cooked, Burnt }

    [Header("패티 모델 프리팹 (껍데기)")]
    public GameObject rawPattyModelPrefab;
    public GameObject cookedPattyModelPrefab;
    public GameObject burntPattyModelPrefab;

    [Header("요리 시간 설정 (누적)")]
    public float timeToCook = 3f;
    public float timeToBurn = 8f;

    private GameObject currentPattyModel;
    private CookingState currentState = CookingState.Raw;
    private float cookingProgress = 0f;
    private bool isOnGrill = false;

    private XRGrabInteractable grabInteractable;

    void Awake()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
        if (grabInteractable != null)
        {
            grabInteractable.selectEntered.AddListener(OnGrabbed);
            grabInteractable.selectExited.AddListener(OnReleased);
        }
    }

    void OnDestroy()
    {
        if (grabInteractable != null)
        {
            grabInteractable.selectEntered.RemoveListener(OnGrabbed);
            grabInteractable.selectExited.RemoveListener(OnReleased);
        }
    }

    void Start()
    {
        ReplaceModel(rawPattyModelPrefab);
    }

    private void OnGrabbed(SelectEnterEventArgs args)
    {
        if (isOnGrill)
        {
            isOnGrill = false;
            Debug.Log("그릴에서 패티를 집었습니다. 시간 측정을 멈춥니다.");
        }
    }

    private void OnReleased(SelectExitEventArgs args)
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
        }
    }

    void Update()
    {
        if (isOnGrill && currentState != CookingState.Burnt)
        {
            cookingProgress += Time.deltaTime;

            if (currentState == CookingState.Raw && cookingProgress >= timeToCook)
            {
                currentState = CookingState.Cooked;
                ReplaceModel(cookedPattyModelPrefab);
                Debug.Log($"누적 {timeToCook}초 → 익은 패티로 변경!");
            }
            else if (currentState == CookingState.Cooked && cookingProgress >= timeToBurn)
            {
                currentState = CookingState.Burnt;
                ReplaceModel(burntPattyModelPrefab);
                Debug.Log($"누적 {timeToBurn}초 → 탄 패티로 변경!");
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Grill"))
        {
            isOnGrill = true;
            Debug.Log($"그릴에 닿음 → 요리 시작 (현재 {cookingProgress:F1}초)");
        }
        else if (collision.gameObject.CompareTag("Trash"))
        {
            Debug.Log("쓰레기통에 닿음 → 패티 제거");
            Destroy(gameObject);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Grill"))
        {
            isOnGrill = false;
            Debug.Log($"그릴에서 떨어짐 → 요리 중단 (현재 {cookingProgress:F1}초)");
        }
    }

    private void ReplaceModel(GameObject newModelPrefab)
    {
        if (newModelPrefab == null) return;

        if (currentPattyModel != null)
        {
            Destroy(currentPattyModel);
        }

        // 모델은 단순히 외형만 → Collider는 본체에서 유지
        currentPattyModel = Instantiate(newModelPrefab, transform);
        currentPattyModel.transform.localPosition = Vector3.zero;
        currentPattyModel.transform.localRotation = Quaternion.identity;
    }
}
