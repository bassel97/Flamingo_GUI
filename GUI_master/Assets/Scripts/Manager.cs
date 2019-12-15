using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Manager : MonoBehaviour
{
    [Header("Connect To Server")]
    [SerializeField] NewBehaviourScript2 connectToServer = null;

    [Header("Board")]
    [SerializeField] private BoardObject boardObject = null;

    [Header("Score")]
    [SerializeField] private Text scoreText = null;
    [SerializeField] private Text wonState = null;

    [SerializeField] private GameObject restartButton = null;
    [SerializeField] private GameObject forfeitButton = null;
    [SerializeField] private GameObject passButton = null;

    [Header("Initial Board")]
    [SerializeField] private GameObject initialBoardOptions = null;
    [SerializeField] private Dropdown initialBoardOptionsDropDown = null;

    [Header("GUI")]
    [SerializeField] private Text warningMessge = null;
    [SerializeField] private Text gamePausedText = null;
    [SerializeField] private Text turnsText = null;

    [Header("Main Menu")]
    public GameObject mainMenu;
    public InputField[] inputMainMenu;
    public Dropdown[] dropdownMainMenu;

    private bool myTurn = false;

    private bool humanVsAI = false;
    private bool humanInputEnabled = false;

    private int lastMoveI = 0, lastMoveJ = 0;

    private int initialCount = 0;
    public void SetInitialCount(string initCount)
    {
        try
        {
            int initialCount = int.Parse(initCount);
            if (initialCount > 0)
                this.initialCount = initialCount;
        }
        catch
        {
            initialCount = 0;
        }
    }

    bool initialBoardOptionBlack = true;
    public void SetinitialBoardBlack(Int32 initCount)
    {
        if (initCount == 0)
        {
            initialBoardOptionBlack = true;
        }
        else
        {
            initialBoardOptionBlack = false;
        }
    }

    private char playerColor = 'b';
    private char serverColor = 'w';
    public void SetPlayerColor(Int32 playerColorInteger)
    {
        if (playerColorInteger == 0)
        {
            playerColor = 'b';
            serverColor = 'w';
        }
        else
        {
            playerColor = 'w';
            serverColor = 'b';
        }
    }

    private string ip;
    public void SetIP(string _ip)
    {
        ip = _ip;
    }

    private string port;
    public void SetPort(string _port)
    {
        port = _port;
    }

    void Start()
    {
    }

    void Update()
    {
    }

    public void AiVsAi()
    {
        connectToServer.SendAI_VS_AI_Data(ip, port);
    }

    public void StartHumanVsAI()
    {
        connectToServer.SendHuman_VS_AI_Data(serverColor, initialCount);

        if (playerColor == 'b')
            humanInputEnabled = true;
        else
            humanInputEnabled = false;

        if (initialCount > 0)
        {
            initialBoardOptions.SetActive(true);
        }
        else if (humanInputEnabled)
        {
            forfeitButton.SetActive(true);
            passButton.SetActive(true);
        }

        humanVsAI = true;
    }

    public void HumanPlayed(int i, int j)
    {
        if (initialCount > 0)
        {
            connectToServer.SendMoveData(j, i, initialBoardOptionBlack ? 'b' : 'w');

            boardObject.PlaceStone(i, j, !initialBoardOptionBlack);

            initialCount--;
            humanInputEnabled = !humanInputEnabled;

            initialBoardOptionsDropDown.value = (1 - initialBoardOptionsDropDown.value);

            if (initialCount <= 0)
            {
                initialBoardOptions.SetActive(false);

                if (humanInputEnabled)
                    forfeitButton.SetActive(true);
                    passButton.SetActive(true);
            }
        }
        else if (humanInputEnabled)
        {
            connectToServer.SendMoveData(j, i, playerColor);

            lastMoveI = i;
            lastMoveJ = j;

            humanInputEnabled = false;
            forfeitButton.SetActive(false);
            passButton.SetActive(false);
        }
    }

    [Header("Timers")]
    public TimerObject ourTimer;
    public TimerObject theirTimer;
    public void SetOurTimer(float stime)
    {
        ourTimer.OverrideTimer(stime);
    }

    public void SetTheirTimer(float stime)
    {
        theirTimer.OverrideTimer(stime);
    }

    public void HumanPassed()
    {
        connectToServer.SendMoveData(-1, -1, playerColor);

        humanInputEnabled = false;
        forfeitButton.SetActive(false);
        passButton.SetActive(false);
    }

    public void AiPlayed()
    {
        if (humanVsAI)
        {
            humanInputEnabled = true;
            forfeitButton.SetActive(true);
            passButton.SetActive(true);
        }
    }

    public void PlayerMoveAccepted()
    {
        humanInputEnabled = false;
        forfeitButton.SetActive(false);
        passButton.SetActive(false);
        boardObject.PlaceStone(lastMoveI, lastMoveJ, playerColor == 'w');
    }

    public void PlayerMoveNotAccepted()
    {
        humanInputEnabled = true;
        forfeitButton.SetActive(true);
        passButton.SetActive(true);
    }

    public void ShowWarning(string warning)
    {
        warningMessge.text = warning;
    }

    public void GameEnd(float gameScore, float theirScore, bool won)
    {
        if (won)
        {
            if (gameScore == theirScore)
                wonState.text = "Draw";
            else
                wonState.text = "You Won";
        }
        else
            wonState.text = "You Lost";

        scoreText.text = gameScore + "-" + theirScore;

        restartButton.SetActive(true);
        forfeitButton.SetActive(false);
        passButton.SetActive(false);
    }

    public void SetScore(float ourScore, float theirScore)
    {
        scoreText.text = ourScore.ToString() + "-" + theirScore.ToString();
    }

    public void Forfeit()
    {
        connectToServer.SendForfeit();

        RestartGame();
    }

    public void GameExit()
    {
        connectToServer.SendGameExit();

        Application.Quit();
    }

    public void SetMyTurn(bool isMyTurn)
    {
        myTurn = isMyTurn;

        if (myTurn)
        {
            turnsText.text = "Our Turn";
            ourTimer.StartTimer(15 * 60);
        }
        else
        {
            turnsText.text = "Their Turn";
            theirTimer.StartTimer(15 * 60);
        }
    }

    public void GamePlayedAvA()
    {
        myTurn = !myTurn;

        if (myTurn)
        {
            turnsText.text = "Our Turn";

            ourTimer.ResumeTimer();
            theirTimer.PauseTimer();
        }
        else
        {
            turnsText.text = "Their Turn";

            ourTimer.PauseTimer();
            theirTimer.ResumeTimer();
        }
    }

    public void RestartGame()
    {
        boardObject.ClearBoard();

        ourTimer.StopTimer();
        theirTimer.StopTimer();

        restartButton.SetActive(false);
        forfeitButton.SetActive(false);
        passButton.SetActive(false);

        scoreText.text = "";
        wonState.text = "";

        turnsText.text = "";

        for (int i = 0; i < inputMainMenu.Length; i++)
        {
            inputMainMenu[i].text = "";
        }

        warningMessge.text = "";

        for (int i = 0; i < dropdownMainMenu.Length; i++)
        {
            dropdownMainMenu[i].value = 0;
        }

        mainMenu.SetActive(true);

        humanVsAI = false;
        humanInputEnabled = false;

        initialCount = 0;

        lastMoveI = 0;
        lastMoveJ = 0;

        playerColor = 'b';
        serverColor = 'w';

        ip = "";
        port = "";

        boardObject.enabled = false;

        initialBoardOptionsDropDown.value = 0;
    }

    public void PauseGame()
    {
        boardObject.ClearBoard();

        gamePausedText.gameObject.SetActive(true);
    }

    public void ResumeGame()
    {
        gamePausedText.gameObject.SetActive(false);
    }
}
