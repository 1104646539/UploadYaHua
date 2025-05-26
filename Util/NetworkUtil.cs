using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using uploadyahua.Model;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Serilog;
using uploadyahua.ViewModel;
using Newtonsoft.Json;

namespace uploadyahua.Util
{
    public class NetworkUtil
    {
        private Socket _serverSocket;
        private OnConnectStateListener _listener;
        // 用于存储所有连接的客户端 Socket
        private readonly List<Socket> _connectedClients = new List<Socket>();
        public bool IsOpen;
        public async Task StartWebSocketServer(string ip, string port, OnConnectStateListener listener)
        {
            _listener = listener;
            IPAddress ipAddress = IPAddress.Parse(ip);
            int portNumber = int.Parse(port);

            _serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                _serverSocket.Bind(new IPEndPoint(ipAddress, portNumber));
                _serverSocket.Listen(5);
                IsOpen = true;
                _listener.onConnectOpenSuccess();

                while (true)
                {
                    Socket clientSocket = await Task.Factory.FromAsync(_serverSocket.BeginAccept, _serverSocket.EndAccept, null);
                    // 记录新连接的客户端
                    _connectedClients.Add(clientSocket);
                    _listener.onClientAdd($"新客户端连接，当前连接数: {_connectedClients.Count}");
                    HandleClientConnection(clientSocket);
                }
            }
            catch (Exception ex)
            {
                _listener.onConnectOpenFailed(ex.Message);
                IsOpen = false;
            }
        }
        public void Close() {
            if (_serverSocket!=null)
            {
                _serverSocket.Dispose();
                _serverSocket.Close();
                IsOpen = false;
            }
        }
        private async Task HandleClientConnection(Socket clientSocket)
        {
            try
            {
                var buffer = new byte[1024 * 40];
                var receivedData = new List<byte>();

                while (true)
                {
                    int bytesRead = await clientSocket.ReceiveAsync(new ArraySegment<byte>(buffer), SocketFlags.None);
                    if (bytesRead == 0)
                    {
                        break;
                    }

                    // 将新接收到的数据添加到 receivedData 中
                    receivedData.AddRange(new ArraySegment<byte>(buffer, 0, bytesRead));

                    // 查找换行符的位置
                    int newlineIndex = -1;
                    for (int i = 0; i < receivedData.Count; i++)
                    {
                        if (receivedData[i] == (byte)'\n')
                        {
                            newlineIndex = i;
                            break;
                        }
                    }

                    if (newlineIndex != -1)
                    {
                        // 提取包含换行符之前的数据
                        byte[] messageBytes = receivedData.Take(newlineIndex).ToArray();
                        var message = SystemGlobal.Encoding.GetString(messageBytes);
                        Log.Information("Received message: " + message);

                        try
                        {
                            // 尝试将消息解析为 TestResult 类
                            TestResult testResult = JsonConvert.DeserializeObject<TestResult>(message);
                            _listener.onNewMsg(testResult);
                        }
                        catch (Exception ex)
                        {
                            Log.Error($"解析消息为 TestResult 类时出错: {ex.Message}");
                        }

                        // 移除已处理的数据
                        receivedData.RemoveRange(0, newlineIndex + 1);
                    }
                }
            }
            catch (Exception ex)
            {
                _listener.onConnectOpenFailed(ex.Message);
            }
            finally
            {
                // 移除断开连接的客户端
                _connectedClients.Remove(clientSocket);
                clientSocket.Close();
                _listener.onConnectDisConenct($"Socket 连接关闭，当前连接数: {_connectedClients.Count}");
            }
        }

        // 判断是否有客户端连接
        public bool HasClientsConnected()
        {
            return _connectedClients.Count > 0;
        }
    }
    public interface OnConnectStateListener
    {
        void onConnectOpenSuccess();
        void onConnectOpenFailed(string error);
        void onClientAdd(string msg);
        void onConnectDisConenct(string error);
        void onNewMsg(TestResult testResult);
    }
}
