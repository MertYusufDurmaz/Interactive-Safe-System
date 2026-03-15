using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;

public class SafeUIController : MonoBehaviour
{
    [Header("References")]
    public SafeController safeController;
    [SerializeField] private string canvasName = "SafeCanvas";

    [Header("UI Elements")]
    public TextMeshProUGUI codeDisplay;
    public Button[] numberButtons;
    public Button clearButton;
    public Button enterButton;
    public Image[] lightIndicators;
    public Button closeButton;

    [Header("Settings")]
    public string correctCode = "2105333";
    public Color correctColor = Color.green;
    public Color wrongColor = Color.red;

    [Header("Events (Manager Entegrasyonları)")]
    public UnityEvent onButtonPressed;
    public UnityEvent onCorrectCode;
    public UnityEvent onWrongCode;
    public UnityEvent onPermanentlyLocked;

    private string currentCode = "";
    private int maxAttempts;
    
    // Save System için
    public int attemptCount = 0;
    public bool isLocked = false;
    public bool isCodeCorrect = false;

    void Awake()
    {
        maxAttempts = lightIndicators.Length;
        foreach (Button btn in numberButtons)
        {
            btn.onClick.AddListener(() => AddDigit(btn.GetComponentInChildren<TextMeshProUGUI>().text));
        }

        clearButton.onClick.AddListener(ClearCode);
        enterButton.onClick.AddListener(CheckCode);
        if (closeButton != null) closeButton.onClick.AddListener(CloseUI);

        ClearCode();
    }

    void Start()
    {
        if (CanvasManager.Instance != null)
            CanvasManager.Instance.RegisterCanvas(canvasName, gameObject);
        else
            gameObject.SetActive(false);
    }

    public void LoadState(int attempts, bool locked, bool open)
    {
        attemptCount = attempts;
        isLocked = locked;
        isCodeCorrect = open;

        if (isLocked)
        {
            codeDisplay.text = "KASA KILITLENDI";
            if (safeController != null) safeController.isPermanentlyLocked = true;
        }
        else if (isCodeCorrect)
        {
            codeDisplay.text = correctCode;
            UpdateLight(lightIndicators.Length - 1, correctColor);
        }
        else
        {
            for (int i = 0; i < attemptCount; i++)
            {
                UpdateLight(i, wrongColor);
            }
        }
    }

    private void AddDigit(string digit)
    {
        if (isLocked || isCodeCorrect) return;
        
        onButtonPressed?.Invoke();

        if (currentCode.Length < correctCode.Length)
        {
            currentCode += digit;
            codeDisplay.text = currentCode.PadRight(correctCode.Length, '-');
        }
    }

    private void ClearCode()
    {
        onButtonPressed?.Invoke();
        currentCode = "";
        if (!isLocked && !isCodeCorrect) codeDisplay.text = "-------";
    }

    private void CheckCode()
    {
        if (isLocked || isCodeCorrect) return;
        
        onButtonPressed?.Invoke();
        attemptCount++;

        if (currentCode == correctCode)
        {
            isCodeCorrect = true;
            UpdateLight(attemptCount - 1, correctColor);
            onCorrectCode?.Invoke(); // Ses ve Görevler buradan tetiklenir
            
            if (safeController != null) safeController.OpenSafeDoor();
            Invoke(nameof(CloseUI), 1.5f);
        }
        else
        {
            onWrongCode?.Invoke(); // Işık patlama efekti ve hata sesi buradan tetiklenir
            UpdateLight(attemptCount - 1, wrongColor);
            
            if (attemptCount >= maxAttempts)
            {
                isLocked = true;
                codeDisplay.text = "KASA KILITLENDI";
                if (safeController != null) safeController.isPermanentlyLocked = true;
                
                onPermanentlyLocked?.Invoke();
                Invoke(nameof(CloseUI), 1.5f);
            }
            else
            {
                Invoke(nameof(ClearCode), 1.5f);
            }
        }
    }

    private void UpdateLight(int index, Color color)
    {
        if (index >= 0 && index < lightIndicators.Length)
        {
            lightIndicators[index].color = color;
        }
    }

    public void CloseUI()
    {
        if (CanvasManager.Instance != null) CanvasManager.Instance.CloseAllCanvases();
        else gameObject.SetActive(false);
    }

    public void OpenSafeUI()
    {
        if (safeController != null && (safeController.isUnlocked || safeController.isPermanentlyLocked)) return;

        if (CanvasManager.Instance != null) CanvasManager.Instance.OpenCanvas(canvasName);
    }
}
