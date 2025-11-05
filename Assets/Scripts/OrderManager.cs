using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum Ingredient
{
    BottomBun, Patty, Lettuce, Tomato, Cheese, Onion, TopBun
}

[System.Serializable]
public class IngredientSprite
{
    public Ingredient type;
    public GameObject spritePrefab;
    public float toppingSpacing;
}

public class OrderManager : MonoBehaviour
{
    public static OrderManager instance;

    [Header(" === Order List === ")]
    [SerializeField] List<Ingredient> order = new List<Ingredient>();
    public Customer currentCustomer;        // 현재 주문 손님

    [Header("Order UI")]
    [SerializeField] private GameObject orderPanel;
    [SerializeField] private Image orderTimeImage;
    [SerializeField] private Transform toppingGroup;


    [Header(" === Ingredient Prefab === ")]
    public int easyToppingMin = 2;
    public int easyToppingMax = 4;
    public int hardToppingMin = 4;
    public int hardToppingMax = 6;
    public List<IngredientSprite> ingredientSprites;
    private Dictionary<Ingredient, GameObject> spriteDict = new Dictionary<Ingredient, GameObject>();


    [Header(" === Patty Probability === ")]
    public float patty1Prob = 0.6f;
    public float patty2Prob = 0.3f;
    public float patty3Prob = 0.1f;


    [Header(" === Topping Probability === ")]
    [Header("Lettuce")]
    public float lettuce0Prob = 0.3f;
    public float lettuce1Prob = 0.6f;
    public float lettuce2Prob = 0.1f;

    [Header("Tomato")]
    public float tomato0Prob = 0.5f;
    public float tomato1Prob = 0.4f;
    public float tomato2Prob = 0.1f;

    [Header("Cheese")]
    public float cheese0Prob = 0.2f;
    public float cheese1Prob = 0.6f;
    public float cheese2Prob = 0.2f;

    [Header("Onion")]
    public float onion0Prob = 0.4f;
    public float onion1Prob = 0.5f;
    public float onion2Prob = 0.1f;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);

            return;
        }

        instance = this;

        // Dict Init
        foreach (var entry in ingredientSprites)
        {
            if (!spriteDict.ContainsKey(entry.type))
                spriteDict.Add(entry.type, entry.spritePrefab);
        }
    }

    private void Start()
    {
        orderPanel.SetActive(false);

        NormalizeTopping();
    }

    void NormalizeTopping()
    {
        // Patty
        float pattySum = patty1Prob + patty2Prob + patty3Prob;
        if (Mathf.Abs(pattySum - 1f) > 0.0001f)
        {
            Debug.LogWarning("[OrderManager] Patty 확률 합이 1이 아님: " + pattySum + " -> 보정");
            patty1Prob += (1f - pattySum);
            // 만약 보정 후 음수가 되면 0으로
            patty1Prob = Mathf.Max(0f, patty1Prob);
        }

        // Lettuce
        float lettuceSum = lettuce0Prob + lettuce1Prob + lettuce2Prob;
        if (Mathf.Abs(lettuceSum - 1f) > 0.0001f)
        {
            Debug.LogWarning("[OrderManager] Lettuce 확률 합이 1이 아님: " + lettuceSum + " -> 보정");
            lettuce0Prob += (1f - lettuceSum);
            lettuce0Prob = Mathf.Max(0f, lettuce0Prob);
        }

        // Tomato
        float tomatoSum = tomato0Prob + tomato1Prob + tomato2Prob;
        if (Mathf.Abs(tomatoSum - 1f) > 0.0001f)
        {
            Debug.LogWarning("[OrderManager] Tomato 확률 합이 1이 아님: " + tomatoSum + " -> 보정");
            tomato0Prob += (1f - tomatoSum);
            tomato0Prob = Mathf.Max(0f, tomato0Prob);
        }

        // Cheese
        float cheeseSum = cheese0Prob + cheese1Prob + cheese2Prob;
        if (Mathf.Abs(cheeseSum - 1f) > 0.0001f)
        {
            Debug.LogWarning("[OrderManager] Cheese 확률 합이 1이 아님: " + cheeseSum + " -> 보정");
            cheese0Prob += (1f - cheeseSum);
            cheese0Prob = Mathf.Max(0f, cheese0Prob);
        }

        // Onion
        float onionSum = onion0Prob + onion1Prob + onion2Prob;
        if (Mathf.Abs(onionSum - 1f) > 0.0001f)
        {
            Debug.LogWarning("[OrderManager] Onion 확률 합이 1이 아님: " + onionSum + " -> 보정");
            onion0Prob += (1f - onionSum);
            onion0Prob = Mathf.Max(0f, onion0Prob);
        }
    }

    public List<Ingredient> Order(Customer customer)         // 주문 생성
    {
        order.Clear();
        currentCustomer = null;
        currentCustomer = customer;
        List<Ingredient> middle = new List<Ingredient>();

        // 1) 번 추가
        order.Add(Ingredient.BottomBun);

        // 2) 패티 개수 결정
        int pattyCount = GetWeightedRandom(patty1Prob, patty2Prob, patty3Prob);
        for (int i = 0; i < pattyCount; i++)
            middle.Add(Ingredient.Patty);

        // 3) 토핑 개수 제한 적용
        int toppingCount;
        if (GameManager.instance.currentMode == GameManager.GameMode.Easy)
        {
            toppingCount = Random.Range(easyToppingMin, easyToppingMax + 1);
        }
        else
        {
            toppingCount = Random.Range(hardToppingMin, hardToppingMax + 1);
        }
        toppingCount -= pattyCount;

        List<Ingredient> possible = new List<Ingredient>() {Ingredient.Lettuce,
                                                            Ingredient.Tomato,
                                                            Ingredient.Cheese,
                                                            Ingredient.Onion };

        for (int i = 0; i < toppingCount; i++)
        {
            Ingredient choose = possible[Random.Range(0, possible.Count)];
            middle.Add(choose);
        }

        // 4) 토핑 순서 섞기
        Shuffle(middle);

        // 4) 마지막 빵
        order.AddRange(middle);
        order.Add(Ingredient.TopBun);

        ShowOrder();
        return order;
    }

    public IReadOnlyList<Ingredient> GetCurrentOrder()
    {
        return order;
    }

    public Customer GetCurrentCustomer()
    {
        return currentCustomer;
    }

    void ShowOrder()                        // 주문 패널 표기
    {
        float curY = 0;

        // 주문 UI 생성
        for (int i = 0; i < order.Count; i++)
        {
            Ingredient ing = order[i];
            GameObject prefab = GetTopping(ing);
            if (prefab == null) continue;

            float spacing = 0f;
            if (spriteDict.ContainsKey(ing))
            {
                IngredientSprite entry = ingredientSprites.Find(x => x.type == ing);
                if (entry != null) spacing = entry.toppingSpacing;
            }

            GameObject toppingObj = Instantiate(prefab, toppingGroup);

            toppingObj.transform.localPosition = new Vector3(0, curY, 0);
            toppingObj.transform.localRotation = Quaternion.identity;

            curY += spacing;
        }

        SetOrderTime(0f);
        orderPanel.SetActive(true);
    }

    public void SetOrderTime(float ratio)
    {
        if (orderTimeImage == null) return;

        orderTimeImage.fillAmount = Mathf.Clamp01(ratio);
    }

    public void OrderClear()                // 주문 초기화
    {
        foreach (Transform child in toppingGroup)
        {
            Destroy(child.gameObject);
        }

        orderPanel.SetActive(false);
        currentCustomer = null;
        SetOrderTime(0f);
        order.Clear();
    }

    // 패티 갯수 선택 메소드
    private int GetWeightedRandom(float p1, float p2, float p3)
    {
        float rand = Random.value;

        if (rand < p1) return 1;
        else if (rand < p1 + p2) return 2;
        else return 3;
    }

    private void AddRandomToppings(List<Ingredient> order, Ingredient ingredient, float p0, float p1, float p2)
    {
        float rand = Random.value;
        int count = 0;
        if (rand < p0) count = 0;
        else if (rand < p0 + p1) count = 1;
        else count = 2;

        for (int i = 0; i < count; i++)
            order.Add(ingredient);
    }

    // 토핑 순서 섞기
    private void Shuffle(List<Ingredient> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int randIndex = Random.Range(i, list.Count);
            Ingredient temp = list[i];
            list[i] = list[randIndex];
            list[randIndex] = temp;
        }
    }

    public GameObject GetTopping(Ingredient topping)
    {
        if (spriteDict.ContainsKey(topping))
            return spriteDict[topping];
        return null;
    }
}
