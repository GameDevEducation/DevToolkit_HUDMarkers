using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InteractableScanner : MonoBehaviour
{
    [SerializeField] float InteractionRange = 2f;
    [SerializeField] LayerMask InteractionMask = ~0;

    InteractableObject CurrentInteractable;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // have we hit something
        RaycastHit hitResult;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hitResult, 
                            InteractionRange, InteractionMask, QueryTriggerInteraction.Collide))
        {
            var candidateInteractable = hitResult.collider.GetComponent<InteractableObject>();

            // if we can't interact then null
            if (candidateInteractable != null && !candidateInteractable.CanInteract)
                candidateInteractable = null;

            // started looking at something, previously looking at nothing
            if (candidateInteractable != null && CurrentInteractable == null)
            {
                OnStartLookingAt(candidateInteractable);
            } // still looking at the same thing
            else if (candidateInteractable != null && candidateInteractable == CurrentInteractable)
            {
                OnContinueLookingAt();
            } // looking at something, previously looking at something elswe
            else if (candidateInteractable != null && candidateInteractable != CurrentInteractable)
            {
                OnStopLookingAt();
                OnStartLookingAt(candidateInteractable);
            } // not looking at something, previously were
            else if (candidateInteractable == null && CurrentInteractable != null)
            {
                OnStopLookingAt();
            }
        } // not looking at something, previously were
        else if (CurrentInteractable != null)
        {
            OnStopLookingAt();
        }
    }

    void OnStartLookingAt(InteractableObject interactable)
    {
        CurrentInteractable = interactable;

        CurrentInteractable.StartLookingAt();
    }

    void OnStopLookingAt()
    {
        CurrentInteractable.StopLookingAt();

        CurrentInteractable = null;
    }

    void OnContinueLookingAt()
    {
        CurrentInteractable.ContinueLookingAt(_Internal_PerformInteract);
    }

    protected bool _Internal_PerformInteract;
    public void OnInteract(InputValue value)
    {
        _Internal_PerformInteract = value.Get<float>() > 0.5f;
    }    
}
