using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public Button SaveButton;
    public Button LoadButton;
    public Scene Scene;

    private void Awake()
    {
        SaveHandler.Instance.StartNewGame();
        Scene = SceneManager.GetActiveScene();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SaveKnaapPosition()
    {
        GameObject gameObject = GameObject.Find("Knaap");
        if (gameObject)
        {
            float[] position = new float[3];
            position[0] = gameObject.transform.position.x;
            position[1] = gameObject.transform.position.y;
            position[2] = gameObject.transform.position.z;
            SaveHandler.Instance.UpdateSaveGame(Scene.name, gameObject.name, position);
        }
    }

    private void UpdatePlayerPostion(float posX, float posY, float posZ)
    {
        GameObject gameObject = GameObject.Find("Knaap");
        if (gameObject)
        {
            gameObject.transform.position = new Vector3(posX, posY, posZ);
        }
    }

    public void LoadSaveState()
    {
    }
}