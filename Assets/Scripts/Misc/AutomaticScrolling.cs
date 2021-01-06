using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AutomaticScrolling : MonoBehaviour
{

    public ScrollRect ScrollRect;
    public RectTransform Content;
    public float ScrollSpeed;
    private float MaxScroll;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        MoveContent();
    }

    private void MoveContent()
    {
        Vector2 contentPosition = ScrollRect.content.position;
        Vector2 newPosition = new Vector2(contentPosition.x, contentPosition.y + ScrollSpeed);
        ScrollRect.content.position = newPosition;
    }
}
