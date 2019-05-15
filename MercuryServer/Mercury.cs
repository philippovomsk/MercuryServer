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


namespace MercuryServer
{
    public partial class Mercury : Form
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private OfdFPDriver mercury;

        private HttpServer server;

        public Mercury()
        {
            InitializeComponent();

            mercury = new OfdFPDriver();

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
        }

        private void Mercury_Load(object sender, EventArgs e)
        {
            BeginInvoke(new MethodInvoker(delegate
            {
               Hide();

               log.Debug("версия библиотеки Меркурия " + mercury.GetVersion());
            }
            ));
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
                }
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            log.Debug("Выход из приложения");
            server.Stop(); 
            Application.Exit();
        }
    }
}
