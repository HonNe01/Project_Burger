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
        GrabPatty, CookPatty, CookedPatty, PutPatty,
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
    [SerializeField][TextArea] private string[] introTexts;
    [SerializeField][TextArea] private string[] grabBottomBunTexts;
    [SerializeField][TextArea] private string[] putBottomBunTexts;
    [SerializeField][TextArea] private string[] grabPattyTexts;
    [SerializeField][TextArea] private string[] cookPattyTexts;
    [SerializeField][TextArea] private string[] cookedPattyTexts;
    [SerializeField][TextArea] private string[] putPattyTexts;
    [SerializeField][TextArea] private string[] grabCheeseTexts;
    [SerializeField][TextArea] private string[] putCheeseTexts;
    [SerializeField][TextArea] private string[] grabTomatoTexts;
    [SerializeField][TextArea] private string[] putTomatoTexts;
    [SerializeField][TextArea] private string[] grabOnionTexts;
    [SerializeField][TextArea] private string[] putOnionTexts;
    [SerializeField][TextArea] private string[] grabLettuceTexts;
    [SerializeField][TextArea] private string[] putLettuceTexts;
    [SerializeField][TextArea] private string[] grabTopBunTexts;
    [SerializeField][TextArea] private string[] putTopBunTexts;
    [SerializeField][TextArea] private string[] servingTexts;
    [SerializeField][TextArea] private string[] endTexts;
    [TextArea]private string[] currentTexts = null;
    private int currentTextIndex = 0;

    [Header("=== Tutorial Light ===")]
    [SerializeField] private Light focusLight;    

    [SerializeField] private Transform bottomBunPoint;
    [SerializeField] private Transform pattyPoint;
    [SerializeField] private Transform grillPoint;
    [SerializeField] private Transform cheesePoint;
    [SerializeField] private Transform tomatoPoint;
    [SerializeField] private Transform onionPoint;
    [SerializeField] private Transform lettucePoint;
    [SerializeField] private Transform topbunPoint;
    [SerializeField] private Transform platePoint;
    [SerializeField] private Transform servingPoint;
    private Transform currentFocusTarget;

    
    private float originAmbIntensity;
    private float originRefIntensity;
    [SerializeField] private float originFocusIntensity;
    [SerializeField] private float focusHeightOffset = 0.5f;
    [SerializeField] private float focusDistance = 1.0f;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);

        if (tutorialPanel != null) tutorialPanel.SetActive(false);
        if (tutorialText != null) tutorialText.text = string.Empty;

        if (focusLight != null)
        {
            originFocusIntensity = focusLight.intensity;
            focusLight.enabled = false;
        }
    }
    

    // ===== Tutorial Start/End =====
    public void StartTutorial()
    {
        GameManager.instance.isTutorial = true;
        Debug.Log("[Tutorial] 버거를 만들어볼까요?");

        if (tutorialPanel != null) tutorialPanel.SetActive(true);

        // 라이트 세팅
        EnterTutorialLights();

        currentStep = Step.None;
        SetCurrentTexts(introTexts);
    }

    public void EndTutorial()
    {
        GameManager.instance.isTutorial = false;
        Debug.Log("[Tutorial] 튜토리얼 종료");

        // 라이트 복구
        ExitTutorialLights();

        // UI 복구
        if (tutorialPanel != null) tutorialPanel.SetActive(false);

        // 상태 초기화
        currentStep = Step.None;
        currentTexts = null;
        currentTextIndex = 0;
        currentFocusTarget = null;

        if (tutorialText != null) tutorialText.text = string.Empty;
    }


    // ===== Text Process =====
    private void SetCurrentTexts(string[] texts) // 현재 스텝의 텍스트 바운딩
    {
        Debug.Log("[Tutorial] Set Text ");
        currentTexts = texts;
        currentTextIndex = 0;

        UpdateTutorialText();
    }

    private void UpdateTutorialText()           // 현재 스텝의 텍스트 출력
    {
        if (tutorialText == null) return;
        if (currentTexts == null || currentTexts.Length == 0)
        {
            tutorialText.text = string.Empty;
            return;
        }

        // 현재 스텝의 텍스트 길이 측정
        if (currentTextIndex < 0 || currentTextIndex >= currentTexts.Length)
        {
            currentTextIndex = Mathf.Clamp(currentTextIndex, 0, currentTexts.Length - 1);
        }

        // 텍스트 출력
        tutorialText.text = currentTexts[currentTextIndex];
    }

    public void OnClickNext()   // 다음 버튼
    {
        if (currentTexts == null || currentTexts.Length == 0) return;

        // 다음 문장으로
        currentTextIndex++;

        // 문구 출력
        if (currentTextIndex < currentTexts.Length)
        {
            UpdateTutorialText();
            return;
        }

        currentTextIndex = currentTexts.Length - 1;

        // 다음 단계 진행
        if (currentStep == Step.None)
        {
            GoToStep(Step.GrabBottomBun);
            return;
        }
        else if (currentStep == Step.End)
        {
            EndTutorial();
        }
    }

    private void GoToStep(Step nextStep)        // 스텝 설정
    {
        currentStep = nextStep;
        Debug.Log($"[Tutorial] Step 변경 -> {nextStep}");

        string[] texts = GetTextsForStep(currentStep);
        SetCurrentTexts(texts);
        UpdateLight();
    }

    private string[] GetTextsForStep(Step step) // 스텝 별 텍스트 선택
    {
        switch (step)
        {
            case Step.GrabBottomBun:    return grabBottomBunTexts;
            case Step.PutBottomBun:     return putBottomBunTexts;
            case Step.GrabPatty:        return grabPattyTexts;
            case Step.CookPatty:        return cookPattyTexts;
            case Step.CookedPatty:      return cookedPattyTexts;
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
                    GoToStep(Step.CookPatty);
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
                    GoToStep(Step.GrabPatty);
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
        GoToStep(Step.CookedPatty);
    }

    public void OnGrabCookedPatty()
    {
        if (!GameManager.instance.isTutorial) return;
        if (currentStep != Step.CookPatty) return;

        Debug.Log("[Tutorial] 구운 패티 집기 완료");
        GoToStep(Step.PutPatty);
    }

    public void OnServing()
    {
        if (!GameManager.instance.isTutorial) return;
        if (currentStep != Step.ServingBurger) return;

        Debug.Log("[Tutorial] 서빙 완료, 튜토리얼 끝");
        GoToStep(Step.End);
    }


    // ===== Tutorial Light =====
    private void EnterTutorialLights()
    {
        // 원본 값 저장
        originAmbIntensity = RenderSettings.ambientIntensity;
        originRefIntensity = RenderSettings.reflectionIntensity;

        // 라이트 세팅
        RenderSettings.ambientIntensity = 0.2f;
        RenderSettings.reflectionIntensity = 0.3f;

        if (focusLight != null)
        {
            focusLight.intensity = originFocusIntensity;
            focusLight.enabled = true;
        }
    }

    private void ExitTutorialLights()
    {
        // 라이트 복구
        RenderSettings.ambientIntensity = originAmbIntensity;
        RenderSettings.reflectionIntensity = originRefIntensity;
        
        if (focusLight != null) focusLight.enabled = false;

        currentFocusTarget = null;
    }

    private void UpdateLight()
    {
        switch (currentStep)
        {
            // === Grab 계열: 재료를 집는 곳 ===
            case Step.None:
                FocusOn();
                break;

            case Step.GrabBottomBun:
                FocusOn(bottomBunPoint);
                break;

            case Step.GrabPatty:
                FocusOn(pattyPoint);       // 패티 집는 곳(패티 트레이)
                break;

            case Step.GrabCheese:
                FocusOn(cheesePoint);
                break;

            case Step.GrabTomato:
                FocusOn(tomatoPoint);
                break;

            case Step.GrabOnion:
                FocusOn(onionPoint);
                break;

            case Step.GrabLettuce:
                FocusOn(lettucePoint);
                break;

            case Step.GrabTopBun:
                FocusOn(topbunPoint);
                break;

            // === Cook: 패티 굽는 곳 ===
            case Step.CookPatty:
            case Step.CookedPatty:
                FocusOn(grillPoint);
                break;

            // === Put 계열: 집어온 재료를 놓는 곳 (PlatePoint) ===
            case Step.PutBottomBun:
            case Step.PutPatty:
            case Step.PutCheese:
            case Step.PutTomato:
            case Step.PutOnion:
            case Step.PutLettuce:
            case Step.PutTopBun:
                FocusOn(platePoint);
                break;

            // === 서빙 ===
            case Step.ServingBurger:
                FocusOn(servingPoint);
                break;

            // 그 외 (인트로 / End 등)
            default:
                ClearFocus();
                break;
        }
    }

    private void FocusOn(Transform target = null)   // 라이트 상태 조정
    {
        currentFocusTarget = target;
        if (focusLight != null && !focusLight.enabled) focusLight.enabled = true;
        else if (target == null) focusLight.enabled = false;

        UpdateFocus();
    }

    private void UpdateFocus()  // 라이트 위치 조정
    {
        if (focusLight == null || currentFocusTarget == null) return;

        // 라이트 위치 조정
        Vector3 targetPos = currentFocusTarget.position;
        focusLight.transform.position = targetPos;
    }
    
    private void ClearFocus()
    {
        currentFocusTarget = null;
        focusLight.enabled = false; 
    }
}