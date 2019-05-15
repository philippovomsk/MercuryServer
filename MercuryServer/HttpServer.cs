using log4net;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace MercuryServer
{
    class HttpServer
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private Thread serverThread;

        private TcpListener listener;

        private int port;

        public int Port
        {
            get { return port; }
            private set { }
        }

        public HttpServer(int port)
        {
            this.port = port;
            log.Debug("Запуск http сервера на " + port + " порту");
            serverThread = new Thread(this.Listen);
            serverThread.Start();
        }

        public void Stop()
        {
            log.Debug("Остановка http сервера");
            serverThread.Abort();
            listener.Stop();
        }

        private void Listen()
        {
            listener = new TcpListener(IPAddress.Any, Port);
            listener.Start();

            while (true)
            {
                try
                {
                    TcpClient client = listener.AcceptTcpClient();
                    Thread Thread = new Thread(new ParameterizedThreadStart(ClientThread));
                    Thread.Start(client);
                }
                catch (Exception ex)
                {
                    log.Error("Ошибка http сервера", ex);
                }
            }
        }

        private static void ClientThread(Object StateInfo)
        {
            new HttpWorker((TcpClient) StateInfo);
        }


        private void Process(HttpListenerContext context)
        {
            string path = context.Request.Url.AbsolutePath;

            log.Debug("Получен http запрос " + path);
            try
            {
                byte[] bytes = Encoding.UTF8.GetBytes("Запрос " + path);

                context.Response.ContentType = "text/plain";
                context.Response.ContentLength64 = bytes.Length;
                context.Response.AddHeader("Date", DateTime.Now.ToString("r"));

                context.Response.OutputStream.Write(bytes, 0, bytes.Length);

                context.Response.StatusCode = (int)HttpStatusCode.OK;
                context.Response.OutputStream.Flush();
            }
            catch (Exception ex)
            {
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            }

            context.Response.OutputStream.Close();
        }
    }
}
