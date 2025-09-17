using UnityEngine;
using System.Collections; // Coroutine 사용을 위해 추가

public class Patty : MonoBehaviour
{
    public GameObject rawPattyPrefab;
    public GameObject cookedPattyPrefab;
    public GameObject burntPattyPrefab;

    private GameObject currentPatty;
    private bool isCooking = false;


    private void OnColliderEnter(Collider other)
    {
        // isCooking 플래그와 태그를 확인
        if (!isCooking && other.CompareTag("Grill"))
        {
            // 요리 코루틴 시작
            StartCoroutine(CookAndBurnProcess());
        }
    }

    // 요리 및 타는 과정을 순차적으로 처리하는 코루틴
    IEnumerator CookAndBurnProcess()
    {
        isCooking = true;
        Debug.Log("불판에 닿음 → 패티 굽기 시작!");

        // 3초 기다림
        yield return new WaitForSeconds(3f);

        ReplacePatty(cookedPattyPrefab);
        Debug.Log("3초 뒤 → 패티가 다 익었습니다!");

        // 추가로 5초 더 기다림 (총 8초)
        yield return new WaitForSeconds(5f);

        ReplacePatty(burntPattyPrefab);
        Debug.Log("8초 뒤 → 패티가 타버렸습니다!");
    }

    void ReplacePatty(GameObject newPrefab)
    {
        // 기존 패티가 있다면 파괴
        if (currentPatty != null)
        {
            Destroy(currentPatty);
        }
        // 새 패티를 생성하고, 이 스크립트가 붙어있는 오브젝트의 자식으로 만듦
        currentPatty = Instantiate(newPrefab, transform.position, Quaternion.identity, transform);
    }
}
