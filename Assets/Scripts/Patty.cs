using UnityEngine;
using System.Collections;

// 이 스크립트는 '보이지 않는 본체' 역할을 하는 빈 오브젝트에 넣습니다.
// 이 오브젝트가 잡기, 물리, 충돌, 상태 변경의 모든 것을 담당합니다.
public class PattyController : MonoBehaviour
{
    // Enum을 사용해 패티의 현재 상태를 명확하게 관리합니다.
    public enum CookingState { Raw, Cooked, Burnt }

    [Header("패티 모델 프리팹 (껍데기)")]
    [Tooltip("처음에 보여줄 안 익은 패티 모델")]
    public GameObject rawPattyModelPrefab;

    [Tooltip("익은 상태로 변할 때의 모델")]
    public GameObject cookedPattyModelPrefab;

    [Tooltip("탄 상태로 변할 때의 모델")]
    public GameObject burntPattyModelPrefab;

    [Header("요리 시간 설정 (누적)")]
    [Tooltip("패티가 익는데 필요한 총 시간 (초)")]
    public float timeToCook = 3f;
    [Tooltip("패티가 타는데 필요한 총 시간 (초)")]
    public float timeToBurn = 8f;

    private GameObject currentPattyModel; // 현재 자식으로 붙어있는, 눈에 보이는 패티 모델
    private CookingState currentState = CookingState.Raw; // 패티의 현재 상태
    private float cookingProgress = 0f;   // 그릴 위에 있었던 총 누적 시간을 저장
    private bool isOnGrill = false;       // 현재 그릴 위에 있는지 실시간으로 확인

    void Start()
    {
        // 게임이 시작되면 '안 익은 패티' 모델을 이 오브젝트의 자식으로 즉시 생성합니다.
        if (rawPattyModelPrefab != null)
        {
            currentPattyModel = Instantiate(rawPattyModelPrefab, transform.position, transform.rotation, transform);
        }
    }

    void Update()
    {
        // 그릴 위에 있을 때만, 그리고 아직 타지 않았을 때만 시간을 계산합니다.
        if (isOnGrill && currentState != CookingState.Burnt)
        {
            // 총 요리 시간에 매 프레임의 시간을 더해줍니다.
            cookingProgress += Time.deltaTime;

            // 상태가 'Raw'이고, 누적 시간이 '익는 시간'을 넘었을 경우
            if (currentState == CookingState.Raw && cookingProgress >= timeToCook)
            {
                currentState = CookingState.Cooked;
                ReplaceModel(cookedPattyModelPrefab);
                Debug.Log($"누적 시간 {timeToCook}초 경과 -> 익은 패티로 변경!");
            }
            // 상태가 'Cooked'이고, 누적 시간이 '타는 시간'을 넘었을 경우
            else if (currentState == CookingState.Cooked && cookingProgress >= timeToBurn)
            {
                currentState = CookingState.Burnt;
                ReplaceModel(burntPattyModelPrefab);
                Debug.Log($"누적 시간 {timeToBurn}초 경과 -> 탄 패티로 변경!");
            }
        }
    }

    // 그릴과 물리적으로 처음 부딪혔을 때 호출됩니다.
    private void OnCollisionEnter(Collision collision)
    {
        // Grill 태그와 충돌했을 때
        if (collision.gameObject.CompareTag("Grill"))
        {
            isOnGrill = true;
            Debug.Log("그릴에 닿았습니다. 시간 측정을 시작합니다. (현재 누적: " + cookingProgress.ToString("F1") + "초)");
        }
        // Trash 태그와 충돌했을 때
        else if (collision.gameObject.CompareTag("Trash"))
        {
            Debug.Log("쓰레기통에 닿았습니다. 패티를 제거합니다.");
            Destroy(gameObject); // 이 스크립트가 붙어있는 '본체' 오브젝트를 파괴합니다.
        }
    }

    // 그릴에서 물리적으로 떨어졌을 때 호출됩니다.
    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Grill"))
        {
            isOnGrill = false;
            Debug.Log("그릴에서 떨어졌습니다. 시간 측정을 멈춥니다. (현재 누적: " + cookingProgress.ToString("F1") + "초)");
        }
    }

    // 자식으로 붙어있는 '모델'만 교체하는 함수
    void ReplaceModel(GameObject newModelPrefab)
    {
        if (newModelPrefab == null) return;

        // 기존 모델이 있다면 파괴합니다.
        if (currentPattyModel != null)
        {
            Destroy(currentPattyModel);
        }

        // 새로운 모델을 이 오브젝트의 자식으로, 같은 위치에 생성합니다.
        currentPattyModel = Instantiate(newModelPrefab, transform.position, transform.rotation, transform);
    }
}

