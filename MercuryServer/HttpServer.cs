using log4net;
using Newtonsoft.Json.Linq;
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

        private MercuryServer mercury;

        public int Port
        {
            get { return port; }
            private set { }
        }

        public MercuryServer Mercury
        {
            get { return mercury; }
            set { mercury = value; }
        }

        public HttpServer(int port)
        {
            this.port = port;
            log.Debug("Запуск http сервера на " + port + " порту");
            serverThread = new Thread(this.Listen);
            serverThread.IsBackground = true;
            serverThread.Start();
        }

        public void Stop()
        {
            log.Debug("Остановка http сервера");
            //serverThread.Abort();
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
                    // обработка запросов в один поток
                    TcpClient client = listener.AcceptTcpClient();
                    ProcessRequest(client);
                }
                catch (Exception ex)
                {
                    if (!(ex is ThreadAbortException))
                    {
                        log.Error("Ошибка http сервера", ex);
                    }
                }
            }
        }

        public void ProcessRequest(TcpClient client)
        {
            NetworkStream stream = client.GetStream();

            StringBuilder request = new StringBuilder();
            byte[] buffer = new byte[1024];

            int numberOfBytesRead = 0;
            do
            {
                // 1с подтормаживает и не успевает прислать тело запроса, поэтому спим и удостоверяемся, что реально всё.
                if (numberOfBytesRead > 0)
                {
                    Thread.Sleep(50);
                    numberOfBytesRead = 0;
                    if(!stream.DataAvailable) { break;  }
                }

                numberOfBytesRead = stream.Read(buffer, 0, buffer.Length);
                request.AppendFormat("{0}", Encoding.UTF8.GetString(buffer, 0, numberOfBytesRead));
            }
            while (stream.DataAvailable || numberOfBytesRead > 0);


            log.Debug("Получен запрос: " + request.ToString());

            String requestString = request.ToString();
            int bodypos = requestString.IndexOf("\r\n\r\n");
            string body = "";
            if (bodypos > 0)
            {
                body = requestString.Substring(bodypos + 4).Trim();
            }

            // 1c начиная с 8.13 при пост запросе перед телом вставляет знак вопроса
            if (!body.StartsWith("{")) {
                body = body.Substring(1);
            }

            if (!requestString.StartsWith("POST"))
            {
                answerError(client);
                return;
            }

            JObject requestJson = new JObject();

            if (body.Trim().Length != 0)
            {
                requestJson = JObject.Parse(body);
            }

            int posHttp = requestString.IndexOf("HTTP");
            string requestType = requestString.Substring(4, posHttp - 4).Trim().ToLower();

            JObject resultJson = new JObject();

            switch(requestType)
            {
                case "/openshift":
                    if(!mercury.openShift(requestJson, out resultJson))
                    {
                        resultJson["error"] = mercury.LastError;
                    }
                    break;

                case "/closeshift":
                    if (!mercury.closeShift(requestJson, out resultJson))
                    {
                        resultJson["error"] = mercury.LastError;
                    }
                    break;

                case "/status":
                    if(!mercury.getCurrentStatus(out resultJson))
                    {
                        resultJson["error"] = mercury.LastError;
                    }
                    break;

                case "/printxreport":
                    if(!mercury.printXReport())
                    {
                        resultJson["error"] = mercury.LastError;
                    }
                    break;

                case "/printcheck":
                    if(!mercury.printCheck(requestJson, out resultJson))
                    {
                        resultJson["error"] = mercury.LastError;
                    }
                    break;

                default:
                    answerError(client);
                    return;
            }


            string result = resultJson.ToString();
            string resultCode = "200 OK";
            if (resultJson["error"] != null)
            {
                resultCode = "403 Forbidden";
            }

            string resultHtml = "HTTP/1.1 " + resultCode + "\nContent-type: text/html\nContent-Length:" + 
                Encoding.UTF8.GetBytes(result).Length + "\n\n" + result;

            byte[] resultBytes = Encoding.UTF8.GetBytes(resultHtml);
            client.GetStream().Write(resultBytes, 0, resultBytes.Length);
            client.Close();
        }

        private void answerError(TcpClient client)
        {
            string answerHtml = "<html><head><meta charset=\"UTF-8\"></head><body>" +
                "<h1>Что-то пошло не так! :)</h1><br>Это http-сервер для печати чеков на Меркурии 119-Ф (usb)<br>" +
                "<br>Cтраница сервера <a href=\"https://github.com/philippovomsk/MercuryServer\">" +
                "https://github.com/philippovomsk/MercuryServer</a></body></html>";


            string answer = "HTTP/1.1 400 Bad Request\nContent-type: text/html\nContent-Length:" + 
                Encoding.UTF8.GetBytes(answerHtml).Length + "\n\n" + answerHtml;
            byte[] answerBytes = Encoding.UTF8.GetBytes(answer);

            client.GetStream().Write(answerBytes, 0, answerBytes.Length);
            client.Close();
        }
    }
}
