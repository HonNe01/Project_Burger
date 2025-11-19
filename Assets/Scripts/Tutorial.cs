using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Tutorial : MonoBehaviour
{
    public static Tutorial instance;

    public enum Step
    {
        None,
        GrabBottomBun, PutBottomBun,
        CookPatty, GrabPatty, PutPatty,
        GrabCheese, PutCheese,
        GrabTomato, PutTomato,
        GrabOnion, PutOnion,
        GrabLettuce, PutLettuce,
        GrabTopBun, PutTopBun,
        ServingBurger,
        End
    }
    [Header("=== Tutorial State ===")]
    public Step currentStep = Step.None;


    [Header("=== Tutorial UI ===")]
    [SerializeField] private GameObject tutorialPanel;
    [SerializeField] private TextMeshProUGUI tutorialText;


    [Header("=== Tutorial Texts ===")]
    [SerializeField] private string[] introTexts;
    [SerializeField] private string[] grabBottomBunTexts;
    [SerializeField] private string[] putBottomBunTexts;
    [SerializeField] private string[] cookPattyTexts;
    [SerializeField] private string[] grabPattyTexts;
    [SerializeField] private string[] putPattyTexts;
    [SerializeField] private string[] grabCheeseTexts;
    [SerializeField] private string[] putCheeseTexts;
    [SerializeField] private string[] grabTomatoTexts;
    [SerializeField] private string[] putTomatoTexts;
    [SerializeField] private string[] grabOnionTexts;
    [SerializeField] private string[] putOnionTexts;
    [SerializeField] private string[] grabLettuceTexts;
    [SerializeField] private string[] putLettuceTexts;
    [SerializeField] private string[] grabTopBunTexts;
    [SerializeField] private string[] putTopBunTexts;
    [SerializeField] private string[] servingTexts;
    [SerializeField] private string[] endTexts;
    private string[] currentTexts = null;
    private int currentTextIndex = 0;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);

        if (tutorialPanel != null) tutorialPanel.SetActive(false);
        if (tutorialText != null) tutorialText.text = string.Empty;
    }
    
    // ===== Tutorial Start/End =====
    public void StartTutorial()
    {
        GameManager.instance.isTutorial = true;
        Debug.Log("[Tutorial] 버거를 만들어볼까요?");

        if (tutorialPanel != null) tutorialPanel.SetActive(true);

        currentStep = Step.None;
        SetCurrentTexts(introTexts);
    }

    public void EndTutorial()
    {
        GameManager.instance.isTutorial = false;
        Debug.Log("[Tutorial] 튜토리얼 종료");
    }

    // ===== Text Process =====
    public void OnClickNext()   // 다음 버튼
    {
        if (currentTexts == null || currentTexts.Length == 0) return;

        // 다음 문장으로
        currentTextIndex++;

        // 문구 출력
        if (currentTextIndex >= currentTexts.Length)
        {
            currentTextIndex = currentTexts.Length - 1;
            return;
        }

        // 다음 단계 진행
        if (currentStep == Step.None)
        {
            GoToStep(Step.GrabBottomBun);
            return;
        }

        currentTextIndex = currentTexts.Length - 1;
    }

    private void GoToStep(Step nextStep)        // 스텝 설정
    {
        currentStep = nextStep;
        Debug.Log($"[Tutorial] Step 변경 -> {nextStep}");

        string[] texts = GetTextsForStep(currentStep);
        SetCurrentTexts(texts);
    }

    private string[] GetTextsForStep(Step step) // 스텝 별 텍스트 선택
    {
        switch (step)
        {
            case Step.GrabBottomBun:    return grabBottomBunTexts;
            case Step.PutBottomBun:     return putBottomBunTexts;
            case Step.CookPatty:        return cookPattyTexts;
            case Step.GrabPatty:        return grabPattyTexts;
            case Step.PutPatty:         return putPattyTexts;
            case Step.GrabCheese:       return grabCheeseTexts;
            case Step.PutCheese:        return putCheeseTexts;
            case Step.GrabTomato:       return grabTomatoTexts;
            case Step.PutTomato:        return putTomatoTexts;
            case Step.GrabOnion:        return grabOnionTexts;
            case Step.PutOnion:         return putOnionTexts;
            case Step.GrabLettuce:      return grabLettuceTexts;
            case Step.PutLettuce:       return putLettuceTexts;
            case Step.GrabTopBun:       return grabTopBunTexts;
            case Step.PutTopBun:        return putTopBunTexts;
            case Step.ServingBurger:    return servingTexts;
            case Step.End:              return endTexts;
            default:                    return null;
        }
    }

    private void SetCurrentTexts(string[] texts) // 텍스트 출력
    {
        currentTexts = texts;
        currentTextIndex = 0;

        UpdateTutorialText();
    }

    private void UpdateTutorialText()
    {
        if (tutorialText == null) return;
        if (currentTexts == null || currentTexts.Length == 0)
        {
            tutorialText.text = string.Empty;
            return;
        }

        if (currentTextIndex < 0 || currentTextIndex >= currentTexts.Length)
        {
            currentTextIndex = Mathf.Clamp(currentTextIndex, 0, currentTexts.Length - 1);
        }

        tutorialText.text = currentTexts[currentTextIndex];
    }

    // ===== Game Interaction =====
    public void OnGrabIngredient(Ingredient ing) // 재료 집기 판단
    {
        if (!GameManager.instance.isTutorial) return;

        switch (currentStep)
        {
            case Step.GrabBottomBun:
                if (ing == Ingredient.BottomBun)
                {
                    Debug.Log("[Tutorial] 아래 빵 잡기 완료");
                    GoToStep(Step.PutBottomBun);
                }
                break;

            case Step.GrabPatty:
                if (ing == Ingredient.Patty)
                {
                    Debug.Log("[Tutorial] 패티 잡기 완료");
                    GoToStep(Step.PutPatty);
                }
                break;

            case Step.GrabCheese:
                if (ing == Ingredient.Cheese)
                {
                    Debug.Log("[Tutorial] 치즈 잡기 완료");
                    GoToStep(Step.PutCheese);
                }
                break;

            case Step.GrabTomato:
                if (ing == Ingredient.Tomato)
                {
                    Debug.Log("[Tutorial] 토마토 잡기 완료");
                    GoToStep(Step.PutTomato);
                }
                break;

            case Step.GrabOnion:
                if (ing == Ingredient.Onion)
                {
                    Debug.Log("[Tutorial] 양파 잡기 완료");
                    GoToStep(Step.PutOnion);
                }
                break;

            case Step.GrabLettuce:
                if (ing == Ingredient.Lettuce)
                {
                    Debug.Log("[Tutorial] 양상추 잡기 완료");
                    GoToStep(Step.PutLettuce);
                }
                break;

            case Step.GrabTopBun:
                if (ing == Ingredient.TopBun)
                {
                    Debug.Log("[Tutorial] 위 빵 잡기 완료");
                    GoToStep(Step.PutTopBun);
                }
                break;
        }
    }

    public void OnPutIngredientOnPlate(Ingredient ing) // 재료 놓기 판단
    {
        if (!GameManager.instance.isTutorial) return;

        switch (currentStep)
        {
            case Step.PutBottomBun:
                if (ing == Ingredient.BottomBun)
                {
                    Debug.Log("[Tutorial] 아래 빵 놓기 완료");
                    GoToStep(Step.CookPatty);
                }
                break;

            case Step.PutPatty:
                if (ing == Ingredient.Patty)
                {
                    Debug.Log("[Tutorial] 패티 올리기 완료");
                    GoToStep(Step.GrabCheese);
                }
                break;

            case Step.PutCheese:
                if (ing == Ingredient.Cheese)
                {
                    Debug.Log("[Tutorial] 치즈 올리기 완료");
                    GoToStep(Step.GrabTomato);
                }
                break;

            case Step.PutTomato:
                if (ing == Ingredient.Tomato)
                {
                    Debug.Log("[Tutorial] 토마토 올리기 완료");
                    GoToStep(Step.GrabOnion);
                }
                break;

            case Step.PutOnion:
                if (ing == Ingredient.Onion)
                {
                    Debug.Log("[Tutorial] 양파 올리기 완료");
                    GoToStep(Step.GrabLettuce);
                }
                break;

            case Step.PutLettuce:
                if (ing == Ingredient.Lettuce)
                {
                    Debug.Log("[Tutorial] 양상추 올리기 완료");
                    GoToStep(Step.GrabTopBun);
                }
                break;

            case Step.PutTopBun:
                if (ing == Ingredient.TopBun)
                {
                    Debug.Log("[Tutorial] 위 빵 올리기 완료");
                    GoToStep(Step.ServingBurger);
                }
                break;
        }
    }

    public void OnPattyCooked()
    {
        if (!GameManager.instance.isTutorial) return;
        if (currentStep != Step.CookPatty) return;

        Debug.Log("[Tutorial] 패티 굽기 완료");
        GoToStep(Step.GrabPatty);
    }

    public void OnServing()
    {
        if (!GameManager.instance.isTutorial) return;
        if (currentStep != Step.ServingBurger) return;

        Debug.Log("[Tutorial] 서빙 완료, 튜토리얼 끝");
        GoToStep(Step.End);
    }
}