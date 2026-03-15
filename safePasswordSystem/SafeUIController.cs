using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SafeUIController : MonoBehaviour
{
    [Header("Referanslar")]
    public SafeController safeController;

    [Header("UI Elemanlarý")]
    public TextMeshProUGUI codeDisplay;
    public Button[] numberButtons;
    public Button clearButton;
    public Button enterButton;
    public Image[] lightIndicators;
    public Button closeButton;

    [Header("Ayarlar")]
    public Color correctColor = Color.green;
    public Color wrongColor = Color.red;

    private string currentCode = "";
    private string correctCode = "2105333";
    private int maxAttempts;

    // Save System için Public yaptýk
    public int attemptCount = 0;
    public bool isLocked = false;
    public bool isCodeCorrect = false;

    public void SetCorrectCode(string newCode)
    {
        correctCode = newCode;
    }

    // --- YENÝ EKLENEN LOAD METODU ---
    public void LoadState(int attempts, bool locked, bool open)
    {
        attemptCount = attempts;
        isLocked = locked;
        isCodeCorrect = open;

        if (isLocked)
        {
            codeDisplay.text = "KASA KILITLENDI";
            if (safeController != null) safeController.lockedText = "Kasa Kilitlendi";
        }
        else if (isCodeCorrect)
        {
            codeDisplay.text = correctCode;
            UpdateLight(lightIndicators.Length - 1, correctColor);
        }
        else
        {
            // Eðer arada bir yerdeysek ýþýklarý güncelle
            if (attemptCount > 0) UpdateLight(attemptCount - 1, wrongColor);
        }
    }

    void Awake()
    {
        maxAttempts = lightIndicators.Length;
        foreach (Button btn in numberButtons)
            btn.onClick.AddListener(() => AddDigit(btn.GetComponentInChildren<TextMeshProUGUI>().text));

        clearButton.onClick.AddListener(ClearCode);
        enterButton.onClick.AddListener(CheckCode);
        if (closeButton != null) closeButton.onClick.AddListener(CloseUI);

        ClearCode();
    }

    void Start()
    {
        if (CanvasManager.Instance != null)
            CanvasManager.Instance.RegisterCanvas("SafeCanvas", gameObject);
        else
            gameObject.SetActive(false);
    }

    private void AddDigit(string digit)
    {
        if (isLocked || isCodeCorrect) return;
        if (VoiceManager.Instance != null) VoiceManager.Instance.PlayButtonPressed();
        if (currentCode.Length < correctCode.Length)
        {
            currentCode += digit;
            codeDisplay.text = currentCode.PadRight(correctCode.Length, '-');
        }
    }

    private void ClearCode()
    {
        if (VoiceManager.Instance != null) VoiceManager.Instance.PlayButtonPressed();
        currentCode = "";
        if (!isLocked && !isCodeCorrect) codeDisplay.text = "-------";
    }

    private void CheckCode()
    {
        if (isLocked || isCodeCorrect) return;
        if (VoiceManager.Instance != null) VoiceManager.Instance.PlayButtonPressed();

        attemptCount++;
        if (currentCode == correctCode)
        {
            if (VoiceManager.Instance != null) VoiceManager.Instance.safeOpenSound();
            UpdateLight(attemptCount - 1, correctColor);
            isCodeCorrect = true;
            if (safeController != null) safeController.OpenSafeDoor();
            Invoke("CloseUI", 1.5f);
            TaskManager.Instance.CompleteTask("task_find_diary");
        }
        else
        {
            LightManager.Instance.TriggerErrorEffect();
            if (VoiceManager.Instance != null) VoiceManager.Instance.SafeErrorSound();
            UpdateLight(attemptCount - 1, wrongColor);
            if (attemptCount >= maxAttempts)
            {
                isLocked = true;
                codeDisplay.text = "KASA KILITLENDI";
                Invoke("CloseUI", 1.5f);
                if (safeController != null) safeController.isUnlocked = true; // Kilitlendi
            }
            else
            {
                Invoke("ClearCode", 1.5f);
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
        if (safeController != null && safeController.isUnlocked) return;

        if (CanvasManager.Instance != null)
        {
            CanvasManager.Instance.OpenCanvas("SafeCanvas");
        }
        else
        {
            gameObject.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}