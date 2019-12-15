using UnityEngine;
using UnityEngine.UI;
using System;
using System.Net.Sockets;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Threading;

public class NewBehaviourScript2 : MonoBehaviour
{
    [Header("Server Data")]
    public int _port = 54000;

    TcpClient client;

    NetworkStream s;
    StreamReader sr;
    StreamWriter sw;

    bool recievingInCorutine = false;

    [Header("Game Data")]
    [SerializeField] private BoardObject boardObject = null;
    [SerializeField] private Manager manager = null;

    [Header("Logger")]
    [SerializeField] private Text loggerText = null;

    void Start()
    {
        try
        {
            client = new TcpClient("127.0.0.1", _port);

            s = client.GetStream();
            sr = new StreamReader(s);
            sw = new StreamWriter(s);
            sw.AutoFlush = true;

        }
        catch (Exception e)
        {
            Debug.Log("Start " + e);
            //consoleText.text = "exception : " + e;
        }
    }

    private void Update()
    {
        if (!recievingInCorutine && s != null)
        {
            StartCoroutine(RecieveAndParseServerReply());
        }
    }

    private void SendAck()
    {
        Dictionary<string, int> dictionary = new Dictionary<string, int>();
        dictionary.Add("type", (int)MsgsEnum.ack);
        dictionary.Add("msg", 0);

        string ackMsg = JsonConvert.SerializeObject(dictionary);

        ackMsg = ackMsg.Remove(ackMsg.Length - 2);
        ackMsg += "\"\"" + "}";

        sw.WriteLine(ackMsg);
        loggerText.text += "Sent " + ackMsg + "\n";
    }

    public void SendAI_VS_AI_Data(string ip, string port)
    {
        Dictionary<string, int> dictionary = new Dictionary<string, int>();
        dictionary.Add("type", (int)MsgsEnum.AI_VS_AI);
        dictionary.Add("msg", 0);

        string ackMsg = JsonConvert.SerializeObject(dictionary);

        AI_VS_AI_Data aI_VS_AI_Data = new AI_VS_AI_Data();
        aI_VS_AI_Data.IP = ip;
        aI_VS_AI_Data.port = port;

        ackMsg = ackMsg.Remove(ackMsg.Length - 2);
        ackMsg += JsonConvert.SerializeObject(aI_VS_AI_Data) + "}";

        sw.WriteLine(ackMsg);
        loggerText.text += "Sent " + ackMsg + "\n";
    }

    public void SendHuman_VS_AI_Data(char serverColor, int initialCount)
    {
        Dictionary<string, int> dictionary = new Dictionary<string, int>();
        dictionary.Add("type", (int)MsgsEnum.AI_VSHuman);
        dictionary.Add("msg", 0);

        string ackMsg = JsonConvert.SerializeObject(dictionary);

        Human_VS_AI_Data human_VS_AI_Data = new Human_VS_AI_Data();
        human_VS_AI_Data.myColor = serverColor;
        human_VS_AI_Data.initialCount = initialCount;

        ackMsg = ackMsg.Remove(ackMsg.Length - 2);
        ackMsg += JsonConvert.SerializeObject(human_VS_AI_Data) + "}";

        sw.WriteLine(ackMsg);
        loggerText.text += "Sent " + ackMsg + "\n";
    }

    public void SendMoveData(int x, int y, char color)
    {
        Dictionary<string, int> dictionary = new Dictionary<string, int>();
        dictionary.Add("type", (int)MsgsEnum.move);
        dictionary.Add("msg", 0);

        string ackMsg = JsonConvert.SerializeObject(dictionary);

        MoveData human_VS_AI_Data = new MoveData();
        human_VS_AI_Data.x = x;
        human_VS_AI_Data.y = y;
        human_VS_AI_Data.color = color;

        ackMsg = ackMsg.Remove(ackMsg.Length - 2);
        ackMsg += JsonConvert.SerializeObject(human_VS_AI_Data) + "}";

        sw.WriteLine(ackMsg);
        loggerText.text += "Sent " + ackMsg + "\n";
    }

    public void SendForfeit()
    {
        Dictionary<string, int> dictionary = new Dictionary<string, int>();
        dictionary.Add("type", (int)MsgsEnum.forfeit);
        dictionary.Add("msg", 0);

        string ackMsg = JsonConvert.SerializeObject(dictionary);

        ackMsg = ackMsg.Remove(ackMsg.Length - 2);
        ackMsg += "\"\"" + "}";

        sw.WriteLine(ackMsg);
        loggerText.text += "Sent " + ackMsg + "\n";
    }

    public void SendGameExit()
    {
        Dictionary<string, int> dictionary = new Dictionary<string, int>();
        dictionary.Add("type", (int)MsgsEnum.exit);
        dictionary.Add("msg", 0);

        string ackMsg = JsonConvert.SerializeObject(dictionary);

        ackMsg = ackMsg.Remove(ackMsg.Length - 2);
        ackMsg += "\"\"" + "}";

        sw.WriteLine(ackMsg);
        loggerText.text += "Sent " + ackMsg + "\n";
    }

    /*private void SendAndCreateToServer()
    {
        switch (sendOptions)
        {
            case SendOptions.AI_VS_AI:
                SendAI_VS_AI_Data();
                break;

            case SendOptions.AckAI_VS_AI:
                SendAck();
                break;

            case SendOptions.Ack:
                SendAck();
                break;

            default:
                break;
        }
    }*/

    private IEnumerator RecieveAndParseServerReply()
    {
        recievingInCorutine = true;

        while (!s.DataAvailable)
            yield return null;

        string ServerReply = sr.ReadLine();

        Debug.Log("Recvd " + ServerReply);
        loggerText.text = "Recvd " + ServerReply + "\n";

        Dictionary<string, object> ServerReplyDict = JsonConvert.DeserializeObject<Dictionary<string, object>>(ServerReply);

        MsgsEnum ServerReplytype = (MsgsEnum)int.Parse(ServerReplyDict["type"].ToString());

        string serverMsg = ServerReplyDict["msg"].ToString();

        switch (ServerReplytype)
        {
            case MsgsEnum.AI_VS_AI:
                break;

            case MsgsEnum.AckAI_VS_AI:

                AckAI_VS_AI_Data ackAI_VS_AI = JsonConvert.DeserializeObject<AckAI_VS_AI_Data>(serverMsg);

                manager.SetMyTurn(ackAI_VS_AI.myTurn);

                if (ackAI_VS_AI.ourRemainingTime > 0)
                    manager.SetOurTimer(ackAI_VS_AI.ourRemainingTime / 1000.0f);

                if (ackAI_VS_AI.theirRemainingTime > 0)
                    manager.SetTheirTimer(ackAI_VS_AI.theirRemainingTime / 1000.0f);

                SendAck();

                break;

            case MsgsEnum.moveConfigrations:

                MoveData moveConfigData = JsonConvert.DeserializeObject<MoveData>(serverMsg);
                boardObject.PlaceStone(moveConfigData.y, moveConfigData.x, moveConfigData.color != 'b');

                if (moveConfigData.ourScore > 0 || moveConfigData.theirScore > 0)
                    manager.SetScore(moveConfigData.ourScore, moveConfigData.theirScore);

                if (moveConfigData.ourTimer > 0)
                    manager.SetOurTimer(moveConfigData.ourTimer / 1000.0f);

                if (moveConfigData.theirTimer > 0)
                    manager.SetTheirTimer(moveConfigData.theirTimer / 1000.0f);

                manager.GamePlayedAvA();

                //sendOptions = SendOptions.Ack;
                SendAck();

                break;

            case MsgsEnum.gameEnd:

                GameEndData gameEndData = JsonConvert.DeserializeObject<GameEndData>(serverMsg);
                manager.GameEnd(gameEndData.ourScore, gameEndData.theirScore, gameEndData.win);
                
                SendAck();

                break;

            case MsgsEnum.gamePaused:

                manager.PauseGame();

                SendAck();

                break;

            case MsgsEnum.exit:
                break;

            case MsgsEnum.ack:

                AckData ackData = JsonConvert.DeserializeObject<AckData>(serverMsg);
                Debug.Log("Ack data " + ackData.reason + " " + ackData.valid + " " + ackData.valid);

                if (ackData.valid)
                {
                    manager.PlayerMoveAccepted();

                    if (ackData.ourScore > 0 || ackData.theirScore > 0)
                        manager.SetScore(ackData.ourScore, ackData.theirScore);
                }
                else
                    manager.PlayerMoveNotAccepted();

                manager.ShowWarning(ackData.reason);

                SendAck();

                break;

            case MsgsEnum.gameStart:

                GameStartData gameStartData = JsonConvert.DeserializeObject<GameStartData>(serverMsg);

                manager.SetMyTurn(gameStartData.myTurn);

                if (gameStartData.ourRemainingTime > 0)
                    manager.SetOurTimer(gameStartData.ourRemainingTime / 1000.0f);

                if (gameStartData.theirRemainingTime > 0)
                    manager.SetTheirTimer(gameStartData.theirRemainingTime / 1000.0f);

                manager.ResumeGame();

                SendAck();

                break;

            case MsgsEnum.AI_VSHuman:
                break;

            case MsgsEnum.move:

                MoveData moveData = new MoveData();
                moveData.countCaptured = 0;

                moveData = JsonConvert.DeserializeObject<MoveData>(serverMsg);
                boardObject.PlaceStone(moveData.y, moveData.x, moveData.color != 'b');

                manager.AiPlayed();

                if (moveData.ourScore > 0 || moveData.theirScore > 0)
                    manager.SetScore(moveData.ourScore, moveData.theirScore);

                //sendOptions = SendOptions.Ack;
                SendAck();

                break;

            case MsgsEnum.forfeit:
                break;

            case MsgsEnum.remove:

                RemoveData removeData = JsonConvert.DeserializeObject<RemoveData>(serverMsg);
                boardObject.RemoveStone(removeData.y, removeData.x);

                //sendOptions = SendOptions.Ack;
                SendAck();

                break;

            default:
                break;
        }

        recievingInCorutine = false;
    }

    private void OnApplicationQuit()
    {
        if (s == null)
            return;

        SendGameExit();

        s.Close();

        client.Close();
    }
}

/*public enum SendOptions
{
    AI_VS_AI,
    AckAI_VS_AI,
    Ack
}*/
