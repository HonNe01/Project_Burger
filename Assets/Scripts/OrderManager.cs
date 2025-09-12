using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Ingredient
{
    Bun, Patty, Lettuce, Tomato, Cheese, Onion
}

public class OrderManager : MonoBehaviour
{
    public static OrderManager instance;

    [Header(" === Patty Probability === ")]
    [Range(0f, 1f)] public float patty1Prob = 0.6f;
    [Range(0f, 1f)] public float patty2Prob = 0.3f;
    [Range(0f, 1f)] public float patty3Prob = 0.1f;

    [Header(" === Topping Probability === ")]
    [Header("Lettuce")]
    [Range(0f, 1f)] public float lettuce0Prob = 0.3f;
    [Range(0f, 1f)] public float lettuce1Prob = 0.6f;
    [Range(0f, 1f)] public float lettuce2Prob = 0.1f;

    [Header("Tomato")]
    [Range(0f, 1f)] public float tomato0Prob = 0.5f;
    [Range(0f, 1f)] public float tomato1Prob = 0.4f;
    [Range(0f, 1f)] public float tomato2Prob = 0.1f;

    [Header("Cheese")]
    [Range(0f, 1f)] public float cheese0Prob = 0.2f;
    [Range(0f, 1f)] public float cheese1Prob = 0.6f;
    [Range(0f, 1f)] public float cheese2Prob = 0.2f;

    [Header("Onion")]
    [Range(0f, 1f)] public float onion0Prob = 0.4f;
    [Range(0f, 1f)] public float onion1Prob = 0.5f;
    [Range(0f, 1f)] public float onion2Prob = 0.1f;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);

            return;
        }

        instance = this;
    }

    public List<Ingredient> Order()
    {
        List<Ingredient> order = new List<Ingredient>();
        List<Ingredient> middle = new List<Ingredient>();

        // 1) 번 추가
        order.Add(Ingredient.Bun);

        // 2) 패티 개수 결정
        int pattyCount = GetWeightedRandom(patty1Prob, patty2Prob, patty3Prob);
        for (int i = 0; i < pattyCount; i++)
            middle.Add(Ingredient.Patty);

        // 3) 토핑 개수 결정
        AddRandomToppings(middle, Ingredient.Lettuce, lettuce0Prob, lettuce1Prob, lettuce2Prob);
        AddRandomToppings(middle, Ingredient.Tomato, tomato0Prob, tomato1Prob, tomato2Prob);
        AddRandomToppings(middle, Ingredient.Cheese, cheese0Prob, cheese1Prob, cheese2Prob);
        AddRandomToppings(middle, Ingredient.Onion, onion0Prob, onion1Prob, onion2Prob);

        // 4) 토핑 순서 섞기
        Shuffle(middle);
        order.AddRange(middle);

        // 4) 마지막 빵
        order.Add(Ingredient.Bun);

        return order;
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
}
