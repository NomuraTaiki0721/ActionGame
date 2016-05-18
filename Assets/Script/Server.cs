using UnityEngine;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System;

namespace ActionGameServer
{

    public class Server : MonoBehaviour
    {
        private TcpListener _lister;
        private TcpClient _client;
        private StreamReader _reader;
        private StreamWriter _writer;

        /// <summary>
        /// 障害物データを受信したときの動作を登録してください
        /// </summary>
        public event Action<Obstacle> dataRecieved = null;

        private object _lock = new object();

      
        public void Start()
        {

        }

        public void Update()
        {

        }

        /// <summary>
        /// クライアント待ち受け開始
        /// </summary>
        public void StartAccepting(int port)
        {
            _lister = new TcpListener(IPAddress.Any, port);
            Debug.Log("StartAccepting");
            StartCoroutine(RunAccepting());
        }

        private IEnumerator RunAccepting()
        {
            _lister.Start();

            while (true)
            {
                yield return null;
                if (_lister.Pending())
                {
                    _client = _lister.AcceptTcpClient();

                    Debug.Log("connected: " + ((IPEndPoint)_client.Client.RemoteEndPoint).Address);
                    var stream = _client.GetStream();
                    //stream.ReadTimeout = 100;
                    stream.WriteTimeout = 100;
                    _reader = new StreamReader(stream);
                    _writer = new StreamWriter(stream) { NewLine = "\r\n", AutoFlush = true };

                    StartCoroutine(RunReceiving(_client));
                    break;
                }
            }
        }

        private IEnumerator RunReceiving(TcpClient clinet)
        {



            // プレイヤーデータを送信します
            Player player = new Player(GameState.Game, 1, 1, 100);
            Debug.Log("befor write");
            WriteToClient(JsonUtility.ToJson(player));
            Debug.Log("after write");

            Debug.Log("OpenStream");

            while (true)
            {
                yield return null;

                string line = _reader.ReadLine();

                if (line != null && line.Length != 0)
                {
                    Debug.Log("received: " + line);

                    OnDataRecieved(JsonUtility.FromJson<Obstacle>(line));
                }
                else
                {
                    Debug.Log("empty");
                }
            }

        }

        /// <summary>
        /// クライアントへ文字列送信
        /// </summary>
        /// <param name="line"></param>
        public void WriteToClient(string line)
        {
            lock (_lock)
            {
                _writer.WriteLine(line);
            }
        }

        private void OnDataRecieved(Obstacle obs)
        {
            Action<Obstacle> tmp = dataRecieved;

            if (tmp != null)
            {
                tmp(obs);
            }
        }
    }
}
