using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WorldSpaceClue : MonoBehaviour
{
    public Clue ClueScriptableObject;
    public Popup Popup;
    public Popup ToBeContinuedPopup;

    private List<Clue> _listOfClues;

    private void Awake()
    {
        _listOfClues = Resources.LoadAll<Clue>("ScriptableObjects/Clues").ToList();
        
        if (SaveHandler.Instance.DoesPlayerHaveClue(ClueScriptableObject.Name))
        {
            //If this throws an error: Check if the clue has an assigned scriptable object
            gameObject.SetActive(false);
        }

        if (!gameObject.GetComponent<Outline>())
        {
            Outline outline = gameObject.AddComponent<Outline>();
            if (outline)
            {
                Color clueColor;
                ColorUtility.TryParseHtmlString("#fabd61", out clueColor);
                outline.OutlineColor = clueColor;
                outline.OutlineMode = Outline.Mode.OutlineVisible;
                outline.OutlineWidth = 5.0f;
                outline.enabled = false;
            }
        }
    }
    
    public void AddToInventory()
    { 
        //Add this clue to the inventory of the player
        SaveHandler.Instance.SaveClue(ClueScriptableObject.Name);

        if (Popup && !DoesPlayerHaveAllCLues()) //todo: remove !DoesPlayerHaveAllCLues() when to be continued popup is not neccisary enymore
        {
            Popup.OpenPopup();
        }
    
        SoundManager.Instance.PlaySound("Clue_pickup");
        gameObject.SetActive(false);

        if (DoesPlayerHaveAllCLues())
            GameManager.PlayerHasAllClues = true;
    }

    private bool DoesPlayerHaveAllCLues()
    {
        return _listOfClues.All(clue => SaveHandler.Instance.DoesPlayerHaveClue(clue.Name));
    }

    private void StartToBeContinuedPopup()
    {
        ToBeContinuedPopup.OpenToBeContinuedPopUp();
    }
}