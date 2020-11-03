using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CluesPanel : MonoBehaviour
{
    public Clue[] Clues;
    public Text Name;
    public Text Details;
    public Text Location;
    public Text Description;
    public Texture2D Image;

    private List<int> _collectedClues;

    // Start is called before the first frame update
    void Awake()
    {
        Clues = Resources.LoadAll<Clue>("ScriptableObjects/Clues");

        // TODO Intialize collected from saved data and/or update when collected
        _collectedClues = new List<int>();
    }

    public void ShowClue(int clueIndex)
    {
        Clue activeClue = Clues[clueIndex];
        if (_collectedClues.Contains(clueIndex))
        {
            Name.text = activeClue.Name;
            Details.text = activeClue.Details;
            Location.text = activeClue.Location;
            Description.text = activeClue.Description;
            Image = activeClue.Image;
        }
        
    }
}
