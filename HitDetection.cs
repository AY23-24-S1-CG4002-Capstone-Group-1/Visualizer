using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using System.Threading;

public class HitDetection : MonoBehaviour
{
    public MqttPublisher mqttPublisher;
    public MqttReceiver mqttReceiver;
    public CustomAREffects care;
    public ReloadEffectController reloadEffectController;
    public GrenadeController grenadeController;
    public ShieldController shieldController;
    public ScoreboardController scoreboardController;
    public GunController gunController;
    public PlayerState playerState;
    public OpponentHealth opponentHealth;
    public GameObject logoutScreen;
    public ReloadEffectController invalidPopUpScreen;
    public ReloadEffectController missedPopUpScreen;
    public GameObject mainUI;

    private string player_id = null;

    [Serializable]
    private class Player
    {
        public int hp;
        public int bullets;
        public int grenades;
        public int shield_hp;
        public int deaths;
        public int shields;
    }

    [Serializable]
    private class GameState
    {
        public Player p1;
        public Player p2;
    }

    private class MqttMessage
    {
        public string type;
        public string player_id;
        public string action;
        public bool isHit;
        public GameState game_state;
    }

    private class Result
    {
        public string player_id;
        public string action;
        public bool isHit;
    }

    public void SetLogoutCanvasActive()
    {
        Canvas mainUICanvas = mainUI.GetComponent<Canvas>();
        mainUICanvas.enabled = false;
        logoutScreen.SetActive(true);
    }

    /*public void SetPopUpCanvasActive()
    {
        Canvas mainUICanvas = mainUI.GetComponent<Canvas>();
        mainUICanvas.enabled = false;
        popUpScreen.SetActive(true);
    }*/

    public void QuitApplication()
    {
        logoutScreen.SetActive(false);
        Application.Quit();
    }

    public void BackToMainUI()
    {
        logoutScreen.SetActive(false);
        mainUI.SetActive(true);
        Canvas mainUICanvas = mainUI.GetComponent<Canvas>();
        mainUICanvas.enabled = true;

    }

    public void BackToMainUIWithTimer()
    {
        StartCoroutine(WaitAndExecute());
    }

    private IEnumerator WaitAndExecute()
    {
        yield return new WaitForSeconds(20f);  // Timer = 20 seconds

        logoutScreen.SetActive(false);
        mainUI.SetActive(true);
        Canvas mainUICanvas = mainUI.GetComponent<Canvas>();
        mainUICanvas.enabled = true;
    }


    void Start()
    {
        mqttReceiver.OnMessageArrived += OnMessageArrivedHandler;
    }

    private void OnMessageArrivedHandler(string newMsg)
    {
        MqttMessage x = JsonUtility.FromJson<MqttMessage>(newMsg);

        if (x.type == "QUERY")
        {
            CheckForHit(x);
        }
        else
        { // x.type == "UPDATE"
            UpdateVisualiser(x);
        }
    }

    private void CheckForHit(MqttMessage x)
    {
        if (x.player_id != player_id)
        {
            return;
        }
        Result res = new Result
        {
            player_id = x.player_id,
            action = x.action,
        };

        // detect hit
        if (care.isTargetVisible)
        {
            res.isHit = true;
        }
        else
        {
            res.isHit = false;
        }

        string publishMsg = JsonUtility.ToJson(res);

        // reply with hitdetection
        mqttPublisher.Publish(publishMsg);
    }

    private Coroutine logoutQueryTimeout;

    private void UpdateVisualiser(MqttMessage x)
    {
        if (player_id == null)
        {
            return;
        }

        // Determine if the current player is the one who triggered the action, perform the actions regardless of the isHit value.
        bool isCurrentPlayer = x.player_id == player_id;

        bool isMissed = !x.isHit;

        switch (x.action)
        {
            case "gun":
                if (isCurrentPlayer) care.ShootGun();
                if (isCurrentPlayer && isMissed) missedPopUpScreen.PlayReloadEffect();
                break;

            case "grenade":
                if (isCurrentPlayer) care.OnPlayerGrenadeButtonPressed();
                else care.OnOpponentGrenadeButtonPressed();
                if (isCurrentPlayer && isMissed) missedPopUpScreen.PlayReloadEffect();
                break;

            case "spear":
                if (isCurrentPlayer) care.OnPlayerSpearButtonPressed();
                else care.OnOpponentSpearButtonPressed();
                if (isCurrentPlayer && isMissed) missedPopUpScreen.PlayReloadEffect();
                break;

            case "shield":
                if (isCurrentPlayer) care.OnPlayerShieldButtonPressed();
                else care.OnOpponentShieldButtonPressed();
                break;

            case "hammer":
                if (isCurrentPlayer) care.OnPlayerHammerButtonPressed();
                else care.OnOpponentHammerButtonPressed();
                if (isCurrentPlayer && isMissed) missedPopUpScreen.PlayReloadEffect();
                break;

            case "punch":
                if (isCurrentPlayer) care.OnPlayerPunchButtonClicked();
                else care.OnOpponentPunchButtonClicked();
                if (isCurrentPlayer && isMissed) missedPopUpScreen.PlayReloadEffect();
                break;

            case "portal":
                if (isCurrentPlayer) care.OnPlayerPortalButtonClicked();
                else care.OnOpponentPortalButtonClicked();
                if (isCurrentPlayer && isMissed) missedPopUpScreen.PlayReloadEffect();
                break;

            case "web":
                if (isCurrentPlayer) care.OnPlayerWebButtonPressed();
                else care.OnOpponentWebButtonPressed();
                if (isCurrentPlayer && isMissed) missedPopUpScreen.PlayReloadEffect();
                break;

            case "reload":
                if (isCurrentPlayer) reloadEffectController.PlayReloadEffect();
                break;

            case "invalid":
                if (isCurrentPlayer) invalidPopUpScreen.PlayReloadEffect();
                break;

            case "logout":
                if (isCurrentPlayer)
                {
                    SetLogoutCanvasActive();
                    logoutQueryTimeout = StartCoroutine(WaitAndExecute());
                }
                break;

            default:
                BackToMainUIWithTimer();
                break;
        }

        // UPDATE UI
        GameState game_state = JsonUtility.FromJson<GameState>(JsonUtility.ToJson(x.game_state));
        Player p1, p2;
        if (player_id == "1")
        {
            p1 = JsonUtility.FromJson<Player>(JsonUtility.ToJson(game_state.p1));
            p2 = JsonUtility.FromJson<Player>(JsonUtility.ToJson(game_state.p2));
        }
        else
        {
            p2 = JsonUtility.FromJson<Player>(JsonUtility.ToJson(game_state.p1));
            p1 = JsonUtility.FromJson<Player>(JsonUtility.ToJson(game_state.p2));
        }

        scoreboardController.SetScore(p2.deaths, p1.deaths);
        grenadeController.SetGrenades(p1.grenades);
        shieldController.SetShields(p1.shields);
        gunController.SetAmmo(p1.bullets);

        bool isInit = (x.action == "none") ? true : false;

        playerState.SetShieldHp(p1.shield_hp, isInit);
        playerState.SetHealth(p1.hp, isInit);
        opponentHealth.SetShieldHp(p2.shield_hp, isInit);
        opponentHealth.SetHealth(p2.hp, isInit);

        if (p1.shield_hp > 0)
        {
            care.OnPlayerShieldButtonPressed();
        }
        if (p2.shield_hp > 0)
        {
            care.OnOpponentShieldButtonPressed();
        }
    }

    private IEnumerator PublishMessage(string publishMsg, float seconds)
    {
        yield return new WaitForSeconds(seconds);
        mqttPublisher.Publish(publishMsg);
    }

    public void SetPlayer(string x)
    {
        player_id = x;

        Result res = new Result();
        res.player_id = player_id;
        res.action = "none";

        string publishMsg = JsonUtility.ToJson(res);

        // reply with hitdetection
        mqttPublisher.Publish(publishMsg);
    }

}