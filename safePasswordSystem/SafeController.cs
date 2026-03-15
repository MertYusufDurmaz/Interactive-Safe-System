using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class SafeController : MonoBehaviour
{
    public SafeUIController safeUI;
    public string lockedText = "Kilitli";
    public string unlockedText = "Kilit Acildi";
    public TextMeshPro interactionText;

    public PlayerControllerHandler playerControllerHandler;
    public InspectionHandler inspectionHandler;
    public MovementController movementController;
    public MouseLook MouseLook;
    public GameObject crosshairUI;

    public Transform safeDoor;
    public float openDoorZRotation = -10f;
    public float doorOpenSpeed = 0.1f;

    public GameObject tabPanel;

    public bool isUnlocked = false;
    private Quaternion initialDoorRotation;
    private Coroutine doorOpenCoroutine;
    // internal object raycastProcess; // Bu gereksiz görünüyor, silebilirsin veya kalabilir.

    private void Start()
    {
        if (safeUI != null)
        {
            safeUI.CloseUI();
        }
        if (interactionText != null)
        {
            interactionText.text = "";
        }
        if (safeDoor != null)
        {
            initialDoorRotation = safeDoor.localRotation;
        }
        playerControllerHandler = FindObjectOfType<PlayerControllerHandler>();
    }

    private void Update()
    {
        // 1. GÜVENLÝK KONTROLÜ (YENÝ EKLENDÝ)
        // Eðer oyuncu yakalandýysa Main Camera kapanýr. 
        // Camera.main null ise bu kodu çalýþtýrma, yoksa oyun çöker.
        if (Camera.main == null)
        {
            // Ýstersen ekrandaki yazýlarý da temizleyebilirsin
            if (interactionText != null) interactionText.text = "";
            return;
        }

        // UI veya menüler açýkken raycast ve etkileþimleri durdur
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject() ||
            (safeUI != null && safeUI.gameObject.activeInHierarchy) ||
            (tabPanel != null && tabPanel.activeInHierarchy))
        {
            return;
        }

        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        // Raycast logic devamý...
        if (Physics.Raycast(ray, out hit, 3f))
        {
            if (hit.collider.gameObject == this.gameObject)
            {
                if (interactionText != null && !isUnlocked)
                {
                    interactionText.text = lockedText;
                }
                if (Input.GetMouseButtonDown(0) && !isUnlocked)
                {
                    OpenSafeUI();
                }
            }
            else
            {
                if (interactionText != null)
                {
                    interactionText.text = "";
                }
            }
        }
        else
        {
            if (interactionText != null)
            {
                interactionText.text = "";
            }
        }
    }

    public void OpenSafeUI()
    {
        if (safeUI != null) safeUI.OpenSafeUI();
    }

    public void OpenSafeDoor()
    {
        Debug.Log("Kasa kapaðý açýlýyor...");
        isUnlocked = true;
        if (interactionText != null)
        {
            interactionText.text = unlockedText;
        }
        if (doorOpenCoroutine != null)
        {
            StopCoroutine(doorOpenCoroutine);
        }
        doorOpenCoroutine = StartCoroutine(AnimateDoorOpen());
    }

    private IEnumerator AnimateDoorOpen()
    {
        if (safeDoor == null)
        {
            Debug.LogError("SafeDoor transform bulunamadý!");
            yield break;
        }
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
        if (doorCollider != null)
        {
            doorCollider.enabled = false;
        }
    }
}