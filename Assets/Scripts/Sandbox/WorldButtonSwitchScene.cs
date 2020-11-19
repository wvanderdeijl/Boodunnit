using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WorldButtonSwitchScene : MonoBehaviour
{
    public bool Pressed;
    public bool PressSwitch;
    public Camera MainCamera;
    private float _buttonScale;
    private float minButtonScale = 0.1f;
    private float maxButtonScale;
    private int _spawnCount = 0;

    private void Awake()
    {
        _buttonScale = maxButtonScale = transform.localScale.z;
    }

    private void Update()
    {
        if (Input.GetMouseButton(0) && !Pressed)
        {
            Ray ray = MainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hitInfo) && !Pressed && hitInfo.transform == transform)
                Push();
        }
    }

    public void Push()
    {
        Pressed = true;
        PressSwitch = true;
        StartCoroutine(PushButton());
    }

    IEnumerator PushButton()
    {
        bool minComparison = _buttonScale > minButtonScale;
        bool maxComparison = _buttonScale < maxButtonScale;
        if (PressSwitch ? minComparison : maxComparison)
        {
            _buttonScale += (PressSwitch ? -1 : 1) * Time.deltaTime;
        }

        if (Math.Abs(minButtonScale - _buttonScale) < Time.deltaTime) PressSwitch = !PressSwitch;
        if (Math.Abs(_buttonScale - maxButtonScale) < Time.deltaTime)
            if (Pressed) Pressed = !Pressed;

        Vector3 scale = transform.localScale;
        scale.z = _buttonScale;
        transform.localScale = scale;
        yield return new WaitForFixedUpdate();
        if (Pressed) StartCoroutine(PushButton());
        else SwitchScene();
    }

    private void SwitchScene()
    {
        SceneTransitionHandler.Instance.GoToScene("CrimeSceneQuest");
    }
}
