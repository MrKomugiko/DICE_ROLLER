using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;

public class MenuScript : MonoBehaviour
{
    [SerializeField] private Canvas _canvas;
    [SerializeField] private Button _buttonShow;
    [SerializeField] private Button _buttonHide;
    [SerializeField] private GameObject _pauseImageObject;

    [SerializeField] menuStatus menuCurrentState = menuStatus.closed;

    enum menuStatus
    {
        open,
        halfOpen,
        closed,
    }
    public UnityAction CurrentButtonAction;
    private UnityAction<float, float> myAction;
    public string Debug_CurrentButtonAction;
    RectTransform canvasRT, menuRT;
    [SerializeField] float width, height;
    [SerializeField] private bool isButtonPressed = false;
    [SerializeField] private bool isPossibleToAnimateMenu = true;

    void Start()
    {
        _pauseImageObject.SetActive(false);

        canvasRT = _canvas.GetComponent<RectTransform>();
        menuRT = this.GetComponent<RectTransform>();
        width = canvasRT.rect.width;
        height = canvasRT.rect.height;
        menuRT.sizeDelta = new Vector2(width, height);
        this.gameObject.transform.localPosition = new Vector3(-width, 0, 0);
        print($"Canvas Size [ {width},{height} ]");

        Debug_CurrentButtonAction = $"Open button = OpenWindowHalf // Close button = null)";

        _buttonShow.onClick.AddListener(() => { OpenWindowHalf(width, height); });
        _buttonShow.interactable = true;

        _buttonHide.interactable = false;
    }

    void OpenWindowHalf(float width, float height)
    {
        if(isButtonPressed == true) return;
        isButtonPressed = true;

        Time.timeScale = 0;
        Color32 currentColor = new Color32(r: 0, g: 164, b: 164, a: 10);
        Color32 newColor = new Color32(r: 0, g: 164, b: 164, a: 200);

        _pauseImageObject.SetActive(true);
        print("przycisk wciśnięty, menu zostanie wysunięte do połowy");

        menuCurrentState = menuStatus.halfOpen;
        _buttonShow.onClick.RemoveAllListeners();
        _buttonHide.onClick.RemoveAllListeners();

        StartCoroutine(AnimateMenu(currentColor, newColor, startPosition: -width, endPosition: (Mathf.RoundToInt(-width / 2))));
        // przypisanie innej funkcji guziczkowi - wysuniecie na maxa

        Debug_CurrentButtonAction = $"Open button = ExpandWindowFull // Close button = CloseWindow";

        _buttonShow.onClick.AddListener(() => { ExpandWindowFull(width, height); });
        _buttonShow.interactable = true;

        _buttonHide.onClick.AddListener(() => { CloseWindow(width, height); });
        _buttonHide.interactable = true;
    }
    void ExpandWindowFull(float width, float height)
    {
        if(isButtonPressed == true) return;

        isButtonPressed = true;
        print("przycisk wciśnięty, menu zostanie wysunięte na maxa");
        
        Time.timeScale = 0;

        
        Color32 currentColor = new Color32(r: 0, g: 164, b: 164, a: 200);
        Color32 newColor = new Color32(r: 0, g: 0, b: 0, a: 255);

        StartCoroutine(AnimateMenu(currentColor, newColor, startPosition: (Mathf.RoundToInt(-width / 2)), endPosition: 0));

        menuCurrentState = menuStatus.open;
        _buttonShow.onClick.RemoveAllListeners();
        _buttonHide.onClick.RemoveAllListeners();


        Debug_CurrentButtonAction = $"Open button = null // Close button = ShrinkWindowToHalf";
        _buttonShow.interactable = false;

        _buttonHide.onClick.AddListener(() => { ShrinkWindowToHalf(width, height); });
        _buttonHide.interactable = true;
    }
    void ShrinkWindowToHalf(float width, float height)
    {
        if(isButtonPressed == true) return;
        isButtonPressed = true;

        Time.timeScale = 0;

        print("przycisk wciśnięty, menu zostanie schowane do połowy");
        menuCurrentState = menuStatus.halfOpen;
        _buttonShow.onClick.RemoveAllListeners();
        _buttonHide.onClick.RemoveAllListeners();

        Color32 currentColor = new Color32(r: 0, g: 0, b: 0, a: 255);
        Color32 newColor = new Color32(r: 0, g: 164, b: 164, a: 200);
        StartCoroutine(AnimateMenu(currentColor, newColor, startPosition: 0, endPosition: (Mathf.RoundToInt(-width / 2))));

        Debug_CurrentButtonAction = $"Open button = ExpandWindowFull // Close button = CloseWindow";
        _buttonShow.onClick.AddListener(() => { ExpandWindowFull(width, height); });
        _buttonShow.interactable = true;

        _buttonHide.onClick.AddListener(() => { CloseWindow(width, height); });
        _buttonHide.interactable = true;

    }
    void CloseWindow(float width, float height)
    {
        if(isButtonPressed == true) return;
        isButtonPressed = true;

        menuCurrentState = menuStatus.closed;
        print("przycisk wciśnięty, menu zostanie wysunięte");

        _buttonShow.onClick.RemoveAllListeners();
        _buttonHide.onClick.RemoveAllListeners();

        Color32 currentColor = new Color32(r: 0, g: 164, b: 164, a: 200);
        Color32 newColor = new Color32(r: 0, g: 164, b: 164, a: 0);
        StartCoroutine(AnimateMenu(currentColor, newColor, startPosition: (Mathf.RoundToInt(-width / 2)), endPosition: (-width)));

        Debug_CurrentButtonAction = $"Open button = OpenWindowHalf // Close button = null)";
        _buttonShow.onClick.AddListener(() => { OpenWindowHalf(width, height); });
        _buttonHide.interactable = true;

        _buttonHide.interactable = false;

        
        Time.timeScale = GameManager.GameSpeedValueModifier;
    }
    IEnumerator AnimateMenu(Color32 startColor, Color32 endColor, float startPosition, float endPosition)
    {
        yield return new WaitUntil(() => isPossibleToAnimateMenu);
        isPossibleToAnimateMenu = false;

        Vector3 startPositionVector3 = new Vector3(startPosition, 0, 0);
        Vector3 endPositionVector3 = new Vector3(endPosition, 0, 0);

        for (float i = 0; i < 1.05; i += 0.05f)
        {
            _pauseImageObject.GetComponent<Image>().color = Color.Lerp(startColor, endColor, i);
            this.gameObject.transform.localPosition = Vector3.Lerp(startPositionVector3, endPositionVector3, i);
            // print("wysuniecie i przyciemnianie tła ednoczesnie");

            yield return new WaitForSecondsRealtime(0.005f);
        }

        if (endColor.a == 0) _pauseImageObject.SetActive(false);
        if (endColor.a != 0) _pauseImageObject.SetActive(true);
        isPossibleToAnimateMenu = true;
        isButtonPressed = false;
    }
}
