using UnityEngine;

public class BurgerIngredient : MonoBehaviour
{
    public string ingredientTag; // 이 재료의 태그 이름 (예: "Patty", "Cheese")

    private void Start()
    {
        // Inspector에서 지정 안 하면 자동으로 GameObject의 태그 사용
        if (string.IsNullOrEmpty(ingredientTag))
        {
            ingredientTag = gameObject.tag;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // 충돌한 오브젝트도 BurgerIngredient 스크립트를 가진 경우
        BurgerIngredient other = collision.gameObject.GetComponent<BurgerIngredient>();

        if (other != null && other.ingredientTag == this.ingredientTag)
        {
            Debug.Log($"{ingredientTag} 끼리 충돌 → 중앙 정렬!");

            // 현재 위치와 충돌한 재료의 중앙 좌표 구하기
            Vector3 centerPos = (transform.position + other.transform.position) / 2f;

            // 두 재료를 중앙에 정렬
            transform.position = new Vector3(centerPos.x, transform.position.y, centerPos.z);
            other.transform.position = new Vector3(centerPos.x, other.transform.position.y, centerPos.z);

            // 위로 쌓기 위해 Y축은 상대적으로 유지 (아래 재료 위에 올라가는 구조)
            if (transform.position.y > other.transform.position.y)
            {
                // 내가 더 위에 있으면 충돌한 애 위에 붙기
                transform.position = new Vector3(centerPos.x, other.transform.position.y + transform.localScale.y, centerPos.z);
            }
            else
            {
                // 내가 아래에 있으면 내가 기준, 다른 애를 위에 붙이기
                other.transform.position = new Vector3(centerPos.x, transform.position.y + other.transform.localScale.y, centerPos.z);
            }
        }
    }
}
