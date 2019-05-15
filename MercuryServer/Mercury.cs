using log4net;
using MercuryCom;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Configuration;
using Newtonsoft.Json.Linq;

namespace MercuryServer
{
    public partial class Mercury : Form
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private MercuryServer mercury;

        private HttpServer server;

        public Mercury()
        {
            InitializeComponent();

            InitMercury();
            InitHttpServer();
        }

        private void InitHttpServer()
        {
            int httpPort = 8090;

            string httpPortParameter = ConfigurationManager.AppSettings["httpServerPort"];
            if (httpPortParameter != null)
            {
                if (!Int32.TryParse(httpPortParameter, out httpPort))
                {
                    httpPort = 8090;
                }
            }

            server = new HttpServer(httpPort);
            server.Mercury = mercury;
        }

        private void InitMercury()
        {
            mercury = new MercuryServer();

            string ofdIPParameter = ConfigurationManager.AppSettings["ofdIP"];
            if (ofdIPParameter != null)
            {
                mercury.OfdIP = ofdIPParameter;
            }

            string ofdPortParameter = ConfigurationManager.AppSettings["ofdPort"];
            int ofdPort = 0;
            if (ofdPortParameter != null && Int32.TryParse(ofdPortParameter, out ofdPort))
            {
                mercury.OfdPort = ofdPort;
            }

            string ofdTimerParameter = ConfigurationManager.AppSettings["ofdTimer"];
            int ofdTimer = 0;
            if (ofdTimerParameter != null && Int32.TryParse(ofdTimerParameter, out ofdTimer))
            {
                mercury.OfdTimer = ofdTimer;
            }

            string fnTimerParameter = ConfigurationManager.AppSettings["fnTimer"];
            int fnTimer = 0;
            if (fnTimerParameter != null && Int32.TryParse(fnTimerParameter, out fnTimer))
            {
                mercury.FnTimer = fnTimer;
            }

            string ofdDocIPParameter = ConfigurationManager.AppSettings["ofdDocIP"];
            if (ofdDocIPParameter != null)
            {
                mercury.OfdDocIP = ofdDocIPParameter;
            }

            string mercuryLogParameter = ConfigurationManager.AppSettings["mercuryLog"];
            bool mercuryLog = false;
            if (mercuryLogParameter != null && Boolean.TryParse(mercuryLogParameter, out mercuryLog))
            {
                mercury.MercuryLog = mercuryLog;
            }
        }

        private void Mercury_Load(object sender, EventArgs e)
        {
            BeginInvoke(new MethodInvoker(delegate
            {
                Hide();

                log.Debug("версия библиотеки Меркурия " + mercury.GetDriverVersion());
                connectFR();
            }
            ));
        }

        private void connectFR()
        {
            if (!mercury.connect())
            {
                if (!Visible)
                {
                    Visible = true;
                    TopMost = true;
                    TopMost = false;
                }

                List<string> state = new List<string>();
                state.Add("Не удалось подключится к фискальному регистратору!");
                state.Add("Ошибка: " + mercury.LastError);

                this.Invoke(new MethodInvoker(delegate ()
                {
                    frstatus.Lines = state.ToArray();
                }));
            }
            else
            {
                updateState();
            }
        }

        private void Mercury_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                Hide();
            }
        }

        private void notifyIcon_Click(object sender, EventArgs e)
        {
            if (e is MouseEventArgs && ((MouseEventArgs)e).Button == System.Windows.Forms.MouseButtons.Left)
            {
                Visible = !Visible;

                if (Visible)
                {
                    TopMost = true;
                    TopMost = false;

                    updateState();
                }
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            log.Debug("Выход из приложения");
            mercury.disconnect();
            server.Stop();
            Application.Exit();
        }

        private void buttonConnect_Click(object sender, EventArgs e)
        {
            connectFR();
        }

        private void updateState()
        {
            List<string> state = new List<string>();

            JObject status;
            if (!mercury.getCurrentStatus(out status))
            {
                state.Add("Не удалось получить состояние фискального регистратора!");
                state.Add("");
                state.Add("Ошибка: " + mercury.LastError);
            }
            else
            {
                state.Add("Смена " + ((int)status["SessionState"] == 1 ? "закрыта" : "открыта"));
                state.Add("");
                state.Add("Неотправленных документов " + (status["BacklogDocumentsCounter"] ?? 0));
                state.Add("Дата первого неотправленного " + (status["BacklogDocumentFirstDateTime"] ?? ""));
            }

            this.Invoke(new MethodInvoker(delegate ()
            {
                frstatus.Lines = state.ToArray();
            }));

        }

        private void buttonxreport_Click(object sender, EventArgs e)
        {
            if(!mercury.printXReport())
            {
                List<string> state = new List<string>();
                state.Add("Не удалось напечатать х-отчет!");
                state.Add("");
                state.Add("Ошибка: " + mercury.LastError);

                this.Invoke(new MethodInvoker(delegate ()
                {
                    frstatus.Lines = state.ToArray();
                }));
            }
            else
            {
                updateState();
            }
        }

        private void buttonzreport_Click(object sender, EventArgs e)
        {
            JObject parameters = new JObject();
            parameters["CashierName"] = "Администратор";
            JObject result;

            if (!mercury.closeShift(parameters, out result))
            {
                List<string> state = new List<string>();
                state.Add("Не удалось напечатать z-отчет!");
                state.Add("");
                state.Add("Ошибка: " + mercury.LastError);

                this.Invoke(new MethodInvoker(delegate ()
                {
                    frstatus.Lines = state.ToArray();
                }));
            }
            else
            {
                updateState();
            }

        }
    }
}
