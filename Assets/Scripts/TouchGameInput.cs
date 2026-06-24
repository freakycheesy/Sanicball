using Sanicball.Data;
using Sanicball.Logic;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TouchGameInput : MonoBehaviour
{
    public static TouchGameInput instance;
    public static Vector3 GetMovement()
    {
        if(instance == null) return Vector3.zero;
        float x = instance.movementJoystick.Horizontal;
        float z = instance.movementJoystick.Vertical;
        return new Vector3(x, 0 , z).normalized;
    }

    public static Vector2 GetCamera()
    {
        if (instance == null) return Vector2.zero;
        if (ActiveData.GameSettings.useOldControls)
        {
            for (int i = 0; i < Input.touchCount; i++)
            {
                Touch touch = Input.GetTouch(i);
                if (touch.position.x > Screen.width / 2 && touch.phase == TouchPhase.Moved) return touch.deltaPosition * 0.1f;
            }
            return Vector2.zero;
        }
        return instance.cameraJoystick.Direction;
    }
    public static bool jump { get; set; }
    public static bool brake { get; set; }
    public static bool respawn { get; set; }
    public static bool openingMenu { get; set; }
    public static bool canChangeMusic => FindObjectOfType<RaceManager>() != null && FindObjectOfType<RaceManager>().currentState == RaceState.Racing;
    public static bool pause { get; set; }
    public GameObject touchScreenCanvas;
    public Joystick movementJoystick;
    public Joystick cameraJoystick;
    private void LateUpdate()
    {
        respawn = false;
        openingMenu = false;
        pause = false;
    }
    private void OnEnable()
    {
        if(instance == null)
        {
            instance = this;
            if (!Application.isMobilePlatform)
            {
                Destroy(gameObject);
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public Text actionAText;
    public Text actionBText;
    private void Update()
    {
        cameraJoystick.gameObject.SetActive(!ActiveData.GameSettings.useOldControls);
        var matchManager = FindObjectOfType<MatchManager>();
        if (matchManager == null)
        {
            if(touchScreenCanvas.activeSelf) touchScreenCanvas.SetActive(false);
            return;
        }
        if (!touchScreenCanvas.activeSelf) touchScreenCanvas.SetActive(true);
        actionAText.text = matchManager.InLobby ? "Ready" : "Respawn";
        actionBText.text = matchManager.InLobby ? "Join" : (canChangeMusic ? "Change Music" : "Start Race");
    }
}
