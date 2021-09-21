using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Interactables/Interactable Config", fileName = "InteractableConfig")]
public class InteractableConfigSO : ScriptableObject
{
    public string Name;
    public string Description;
    public string Verb;
    public float InteractionTime = 0f;
}
