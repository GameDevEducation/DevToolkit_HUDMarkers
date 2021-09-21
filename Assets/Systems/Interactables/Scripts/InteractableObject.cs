using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;

public class InteractableObject : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI ObjectName;
    [SerializeField] TextMeshProUGUI Description;
    [SerializeField] TextMeshProUGUI InputPrompt;
    [SerializeField] Slider ActivationProgress;
    [SerializeField] CanvasGroup PromptGroup;
    [SerializeField] RectTransform PromptUI;

    [SerializeField] bool AllowInteraction = true;
    [SerializeField] InteractableConfigSO ConfigSO;
    [SerializeField] float AlphaSlewRate = 2f;

    [SerializeField] UnityEvent OnInteractionCompleted = new UnityEvent();

    float TargetAlpha = 0f;
    float InteractionProgress = 0f;

    public bool CanInteract => AllowInteraction;

    // Start is called before the first frame update
    void Start()
    {
        ObjectName.text = ConfigSO.Name;
        Description.text = ConfigSO.Description;
        InputPrompt.text = "Press [E] to " + ConfigSO.Verb;
        ActivationProgress.gameObject.SetActive(false);
        PromptGroup.alpha = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        // adust the fade on the panel
        if (PromptGroup.alpha != TargetAlpha)
            PromptGroup.alpha = Mathf.MoveTowards(PromptGroup.alpha, TargetAlpha, AlphaSlewRate * Time.deltaTime);

        // face the camera
        if (PromptGroup.alpha > 0)
            PromptUI.LookAt(Camera.main.transform, Vector3.up);
    }

    public void StartLookingAt()
    {
        TargetAlpha = 1f;
    }

    public void StopLookingAt()
    {
        TargetAlpha = 0f;
    }

    public void ContinueLookingAt(bool performInteraction)
    {
        // are performing an interaction
        if (performInteraction)
        {
            // no interaction time set?
            if (ConfigSO.InteractionTime <= 0)
            {
                InteractionCompleted();
                return;
            }

            // update the progress, check if completed
            InteractionProgress += Time.deltaTime / ConfigSO.InteractionTime;
            if (InteractionProgress >= 1f)
            {
                InteractionCompleted();
                return;                                
            }

            // update the progress slider
            ActivationProgress.value = InteractionProgress;
            if (!ActivationProgress.gameObject.activeInHierarchy)
                ActivationProgress.gameObject.SetActive(true);
        }
        else if (ConfigSO.InteractionTime > 0)
        {
            ActivationProgress.value = InteractionProgress = 0f;
            ActivationProgress.gameObject.SetActive(false);
        }
    }

    void InteractionCompleted()
    {
        AllowInteraction = false;
        PromptGroup.alpha = TargetAlpha = 0f;

        OnInteractionCompleted.Invoke();

        Destroy(gameObject);
    }
}
