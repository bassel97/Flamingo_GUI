using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NetMQ;
using NetMQ.Sockets;
using System;
using UnityEngine.Assertions;
using System.Linq;
using Newtonsoft.Json;
using System.Text;
using System.Runtime.Serialization.Json;
using System.IO;
using System.Threading;
using System.Collections.Concurrent;

/*public class NetMqListener
{
    private readonly Thread _listenerWorker;

    private bool _listenerCancelled;

    public delegate void MessageDelegate(string message);

    private readonly MessageDelegate _messageDelegate;

    private readonly ConcurrentQueue<string> _messageQueue = new ConcurrentQueue<string>();

    private void ListenerWork()
    {
        AsyncIO.ForceDotNet.Force();
        using (var subSocket = new SubscriberSocket())
        {
            subSocket.Options.ReceiveHighWatermark = 1000;
            subSocket.Connect("tcp://localhost:12345");
            subSocket.Subscribe("");
            while (!_listenerCancelled)
            {
                string frameString;
                if (!subSocket.TryReceiveFrameString(out frameString)) continue;
                Debug.Log(frameString);
                _messageQueue.Enqueue(frameString);
            }
            subSocket.Close();
        }
        NetMQConfig.Cleanup();
    }

    public void Update()
    {
        while (!_messageQueue.IsEmpty)
        {
            string message;
            if (_messageQueue.TryDequeue(out message))
            {
                _messageDelegate(message);
            }
            else
            {
                break;
            }
        }
    }

    public NetMqListener(MessageDelegate messageDelegate)
    {
        _messageDelegate = messageDelegate;
        _listenerWorker = new Thread(ListenerWork);
    }

    public void Start()
    {
        _listenerCancelled = false;
        _listenerWorker.Start();
    }

    public void Stop()
    {
        _listenerCancelled = true;
        _listenerWorker.Join();
    }
}*/

public class NewBehaviourScript : MonoBehaviour
{
    /*private NetMqListener _netMqListener;

    private void HandleMessage(string message)
    {
        var splittedStrings = message.Split(' ');
        if (splittedStrings.Length != 3) return;
        var x = float.Parse(splittedStrings[0]);
        var y = float.Parse(splittedStrings[1]);
        var z = float.Parse(splittedStrings[2]);
        transform.position = new Vector3(x, y, z);
    }

    private void Start()
    {
        _netMqListener = new NetMqListener(HandleMessage);
        _netMqListener.Start();
    }

    private void Update()
    {
        _netMqListener.Update();
    }

    private void OnDestroy()
    {
        _netMqListener.Stop();
    }*/

    /*Thread client_thread_;
    private UnityEngine.Object thisLock_ = new UnityEngine.Object();
    bool stop_thread_ = false;

    void Start()
    {
        Debug.Log("Start a request thread.");
        client_thread_ = new Thread(NetMQClient);
        client_thread_.Start();
    }

    // Client thread which does not block Update()
    void NetMQClient()
    {
        AsyncIO.ForceDotNet.Force();
        NetMQConfig.ManualTerminationTakeOver();
        NetMQConfig.ContextCreate(true);

        string msg;
        var timeout = new System.TimeSpan(0, 0, 1); //1sec

        Debug.Log("Connect to the server.");
        var requestSocket = new RequestSocket(">tcp://192.168.11.36:50020");
        requestSocket.SendFrame("SUB_PORT");
        bool is_connected = requestSocket.TryReceiveFrameString(timeout, out msg);

        while (is_connected && stop_thread_ == false)
        {
            Debug.Log("Request a message.");
            requestSocket.SendFrame("msg");
            is_connected = requestSocket.TryReceiveFrameString(timeout, out msg);
            Debug.Log("Sleep");
            Thread.Sleep(1000);
        }

        requestSocket.Close();
        Debug.Log("ContextTerminate.");
        NetMQConfig.ContextTerminate();
    }

    void Update()
    {
        /// Do normal Unity stuff
    }

    void OnApplicationQuit()
    {
        lock (thisLock_) stop_thread_ = true;
        client_thread_.Join();
        Debug.Log("Quit the thread.");
    }*/

    public int _port = 7788;

    public ResponseSocket _server;

    public InputField testString;
    public Text text;

    void Start()
    {
        AsyncIO.ForceDotNet.Force();
        NetMQConfig.Linger = new TimeSpan(0, 0, 1);

        _server = new ResponseSocket();
        _server.Options.Linger = new TimeSpan(0, 0, 1);
        _server.Bind($"tcp://*:{_port}");
        Debug.Log($"server on {_port}");

        Assert.IsNotNull(_server);

        StartCoroutine(_CoWorker());
    }

    void OnDisable()
    {
        _server?.Dispose();
        NetMQConfig.Cleanup(false);
    }

    string MyDictionaryToJson(Dictionary<string, List<int>> dict)
    {
        var entries = dict.Select(d =>
            string.Format("\"{0}\": [{1}]", d.Key, string.Join(",", d.Value)));

        string s = "{" + string.Join(",", entries) + "}";

        return s;
    }

    IEnumerator _CoWorker()
    {
        var wait = new WaitForSeconds(1f);

        while (true)
        {
            Debug.Log("poll the recv...");
            if (_server.TryReceiveFrameString(out string recv))
            {
                Debug.Log($"server recv: {recv}");
                text.text = recv;

                Dictionary<string, List<int>> dict = new Dictionary<string, List<int>>();
                List<int> l = new List<int>();
                l.Add(1);
                dict.Add("Bassel", l);
                l = new List<int>();
                l.Add(1);
                l.Add(2);
                dict.Add("Mostafa", l);
                l = new List<int>();
                l.Add(1);
                l.Add(2);
                l.Add(3);
                dict.Add("Kamel", l);

                string s = JsonConvert.SerializeObject(dict);

                _server.SendFrame(s);
            }
            else
            {
                Debug.Log("no recv...");
            }

            yield return wait;
        }
    }
}


