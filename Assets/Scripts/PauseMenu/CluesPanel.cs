using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class CluesPanel : MonoBehaviour
{
    public List<Clue> Clues;
    public Text Name;
    public Text Details;
    public Text Location;
    public Text Description;
    public Image Image;

    private List<string> _collectedClueNames;
    private string _hiddenClueName = "???";

    // Start is called before the first frame update
    void Awake()
    {
        Clues = Resources.LoadAll<Clue>("ScriptableObjects/Clues").ToList();
        _collectedClueNames = new List<string>();
        foreach (Clue clue in Clues)
        {
            if (SaveHandler.Instance.DoesPlayerHaveClue(clue.Name))
            {
                _collectedClueNames.Add(clue.Name);
            }
        }
    }

    public void ShowClue(string clueName)
    {
        Clue activeClue = Clues.FirstOrDefault(c => c.Name == clueName);
        if (_collectedClueNames.Contains(clueName))
        {
            Name.text = activeClue.Name;
            Details.text = activeClue.Details;
            Location.text = activeClue.Location;
            Description.text = activeClue.Description;
            Image.sprite = activeClue.Image;
        } 
        else
        {
            Clue hiddenClue = Clues.FirstOrDefault(c => c.Name == _hiddenClueName);
            Name.text = hiddenClue.Name;
            Details.text = hiddenClue.Details;
            Location.text = hiddenClue.Location;
            Description.text = hiddenClue.Description;
            Image.sprite = hiddenClue.Image;
        }
        
    }
}
