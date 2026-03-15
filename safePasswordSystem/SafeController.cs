using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class SafeController : MonoBehaviour, ITargetable
{
    [Header("References")]
    public SafeUIController safeUI;
    public Transform safeDoor;

    [Header("Door Settings")]
    public float openDoorZRotation = -10f;
    public float doorOpenSpeed = 0.1f;

    [Header("Events")]
    public UnityEvent onSafeOpened;

    public bool isUnlocked = false;
    public bool isPermanentlyLocked = false; // 3 Kez yanlış girilince kilitlenme durumu

    private Quaternion initialDoorRotation;
    private Coroutine doorOpenCoroutine;

    // PlayerRaycaster'ın okuyabileceği dinamik metin
    public string InteractionText 
    {
        get 
        {
            if (isUnlocked) return "Kasa Açık";
            if (isPermanentlyLocked) return "Kasa Bloke Oldu!";
            return "E'ye Bas (Şifre Gir)";
        }
    }

    private void Start()
    {
        if (safeUI != null) safeUI.CloseUI();
        if (safeDoor != null) initialDoorRotation = safeDoor.localRotation;
    }

    // ITargetable arayüzünün zorunlu kıldığı etkileşim metodu
    // PlayerRaycaster tıklandığında burayı otomatik çağıracak!
    public void Interact()
    {
        if (!isUnlocked && !isPermanentlyLocked && safeUI != null)
        {
            safeUI.OpenSafeUI();
        }
    }

    // ITargetable arayüzünün parlatma metodu
    public void ToggleHighlight(bool state)
    {
        // İstersen kasaya outline/parlama efekti ekleyebilirsin
    }

    public void OpenSafeDoor()
    {
        if (isUnlocked) return;
        
        isUnlocked = true;
        onSafeOpened?.Invoke();

        if (doorOpenCoroutine != null) StopCoroutine(doorOpenCoroutine);
        doorOpenCoroutine = StartCoroutine(AnimateDoorOpen());
    }

    private IEnumerator AnimateDoorOpen()
    {
        if (safeDoor == null) yield break;

        Quaternion endRotation = Quaternion.Euler(initialDoorRotation.eulerAngles.x, initialDoorRotation.eulerAngles.y, openDoorZRotation);
        float time = 0;
        Quaternion currentRotation = safeDoor.localRotation;

        while (time < 1)
        {
            safeDoor.localRotation = Quaternion.Slerp(currentRotation, endRotation, time);
            time += Time.deltaTime * doorOpenSpeed;
            yield return null;
        }
        
        safeDoor.localRotation = endRotation;

        Collider doorCollider = safeDoor.GetComponentInParent<Collider>();
        if (doorCollider != null) doorCollider.enabled = false;
    }
}
