using UnityEngine;
using UnityEngine.UI;
using System;
using System.Net.Sockets;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;

/*public class Message
{
    public int type { get; set; }
    public Messagemsg msg { get; set; }
}
public class Messagemsg
{
    public bool initialBoard { get; set; }
    public int theirRemainingTime { get; set; }
    public int initialCount { get; set; }
}*/

public class NewBehaviourScript2 : MonoBehaviour
{
    [Header("Server Data")]
    public int _port = 54000;

    public InputField testString;

    public Text consoleText;

    TcpClient client;

    Stream s;
    StreamReader sr;
    StreamWriter sw;

    SendOptions sendOptions;
    string ip;
    string port;


    [Header("Game Data")]
    [SerializeField] private BoardObject boardObject = null;
    [SerializeField] private Manager manager = null;

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
            //Debug.Log("Start " + e);
            consoleText.text = "exception : " + e;
        }
    }

    //string ip, string port
    public void AI_VS_AI()
    {
        this.ip = "1270";
        this.port = "1547";

        sendOptions = SendOptions.AI_VS_AI;

        StartProtocol();
    }

    public void StartProtocol()
    {
        StartCoroutine(ExecuteClientThread());
    }

    public IEnumerator ExecuteClientThread()
    {
        while (true)
        {
            try
            {
                SendAndCreateToServer();
                s.Flush();

                //sr.p
                if (sr.Peek() != -1)
                    RecieveAndParseServerReply();

                Debug.Log(s);
            }
            catch (Exception e)
            {
                Debug.Log("ExecuteClient() " + e);
            }
            yield return null;
        }
    }

    private void SendAck()
    {
        Dictionary<string, int> dictionary = new Dictionary<string, int>();
        dictionary.Add("type", (int)MsgsEnum.AI_VS_AI);
        dictionary.Add("msg", 0);

        string ackMsg = JsonConvert.SerializeObject(dictionary);

        ackMsg = ackMsg.Remove(ackMsg.Length - 2);
        ackMsg += "\"\"" + "}";

        sw.WriteLine(ackMsg);
    }

    private void SendAI_VS_AI_Data()
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
    }

    private void SendAndCreateToServer()
    {
        switch (sendOptions)
        {
            case SendOptions.AI_VS_AI:
                SendAI_VS_AI_Data();
                break;

            case SendOptions.AckAI_VS_AI:
                SendAck();
                break;

            default:
                break;
        }
    }

    private void RecieveAndParseServerReply()
    {
        string ServerReply = sr.ReadLine();

        Dictionary<string, object> ServerReplyDict = JsonConvert.DeserializeObject<Dictionary<string, object>>(ServerReply);

        MsgsEnum ServerReplytype = (MsgsEnum)int.Parse(ServerReplyDict["type"].ToString());

        string serverMsg = ServerReplyDict["msg"].ToString();

        switch (ServerReplytype)
        {
            case MsgsEnum.AI_VS_AI:
                break;

            case MsgsEnum.AckAI_VS_AI:

                AckAI_VS_AI_Data ackAI_VS_AI = JsonConvert.DeserializeObject<AckAI_VS_AI_Data>(serverMsg);

                sendOptions = SendOptions.AckAI_VS_AI;

                break;

            case MsgsEnum.moveConfigrations:

                MoveData moveConfigData = JsonConvert.DeserializeObject<MoveData>(serverMsg);
                boardObject.PlaceStone(moveConfigData.x, moveConfigData.y, moveConfigData.color != 'b');

                break;

            case MsgsEnum.gameEnd:

                GameEndData gameEndData = JsonConvert.DeserializeObject<GameEndData>(serverMsg);
                manager.EndGame(gameEndData.ourScore, gameEndData.theirScore);

                break;

            case MsgsEnum.gamePaused:
                break;

            case MsgsEnum.exit:
                break;

            case MsgsEnum.ack:
                break;

            case MsgsEnum.gameStart:
                break;

            case MsgsEnum.AI_VSHuman:
                break;

            case MsgsEnum.move:

                MoveData moveData = new MoveData();
                moveData.countCaptured = 0;

                moveData = JsonConvert.DeserializeObject<MoveData>(serverMsg);
                boardObject.PlaceStone(moveData.x, moveData.y, moveData.color != 'b');

                break;

            case MsgsEnum.forfeit:
                break;

            case MsgsEnum.remove:

                RemoveData removeData = JsonConvert.DeserializeObject<RemoveData>(serverMsg);
                boardObject.RemoveStone(removeData.x, removeData.y);

                break;

            default:
                break;
        }
    }

    private void OnApplicationQuit()
    {
        if (s == null)
            return;

        s.Close();

        client.Close();
    }
}

public enum SendOptions
{
    AI_VS_AI,
    AckAI_VS_AI
}