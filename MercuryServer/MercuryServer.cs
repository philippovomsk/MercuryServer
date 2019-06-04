using log4net;
using MercuryCom;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;

namespace MercuryServer
{
    class MercuryServer
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private OfdFPDriver mercury;

        private string mercuryDeviceId = "";

        private string lastError = "";

        private string ofdIP = "";

        private int ofdPort = 0;

        // Таймер ОФД (10..3600 с)
        private int ofdTimer = 0;

        // Таймер ФН (10..60 с)
        private int fnTimer = 0;

        private string ofdDocIP = "";

        private bool mercuryLog = false;

        public string OfdIP
        {
            get { return ofdIP; }
            set { ofdIP = value; }
        }

        public int OfdPort
        {
            get { return ofdPort; }
            set { ofdPort = value; }
        }

        public int OfdTimer
        {
            get { return ofdTimer; }
            set
            {
                if (value < 10 || value > 3600)
                {
                    ofdTimer = 0;
                }
                else
                {
                    ofdTimer = value;
                }
            }
        }

        public int FnTimer
        {
            get { return fnTimer; }
            set
            {
                if (value < 10 || value > 60)
                {
                    fnTimer = 0;
                }
                else
                {
                    fnTimer = value;
                }
            }
        }

        public string OfdDocIP
        {
            get { return ofdDocIP; }
            set { ofdDocIP = value; }
        }

        public string LastError
        {
            get { return lastError; }
        }

        public bool MercuryLog
        {
            get { return mercuryLog; }
            set { mercuryLog = value; }
        }

        public MercuryServer()
        {
            mercury = new OfdFPDriver();
        }

        public string GetDriverVersion()
        {
            return mercury.GetVersion();
        }

        public bool connect()
        {
            log.Debug("Подключение Меркурия");

            if (mercuryDeviceId.Length != 0)
            {
                log.Debug("Меркурий уже подключен");
                return true;
            }

            lastError = "";

            int numberOfDevices = 0;
            if (!mercury.GetUsbQuantity(out numberOfDevices))
            {
                processError();
                return false;
            }

            if (numberOfDevices < 1)
            {
                processError("Количество подключенных Меркуриев " + numberOfDevices);
                return false;
            }

            string deviceName = "";
            if (!mercury.GetUsbNameFromNumber(1, out deviceName))
            {
                processError();
                return false;
            }

            int portNumber = 0;
            if (!mercury.GetUsbPortNumberFromName(deviceName, out portNumber))
            {
                processError();
                return false;
            }

            log.Debug("Подключаем первое устройство: " + deviceName + " /" + portNumber);

            mercury.SetParameter("Protocol", 1); // Меркурий-119Ф(ОФД)
            mercury.SetParameter("Speed", 9600); // скорость для usb не важна
            mercury.SetParameter("Port", portNumber);
            if (OfdIP.Length > 0)
            {
                mercury.SetParameter("OfdIP", OfdIP);
            }
            if (OfdPort != 0)
            {
                mercury.SetParameter("OfdPort", OfdPort);
            }
            if (OfdTimer != 0)
            {
                mercury.SetParameter("OfdTimer", OfdTimer);
            }
            if (FnTimer != 0)
            {
                mercury.SetParameter("FnTimer", FnTimer);
            }
            if (ofdDocIP.Length > 0)
            {
                mercury.SetParameter("OfdDocIP", OfdTimer);
            }
            mercury.SetParameter("Log", MercuryLog);

            mercuryDeviceId = "";
            if (!mercury.Open(out mercuryDeviceId))
            {
                mercuryDeviceId = "";
                processError();
                return false;
            }

            return true;
        }

        public bool disconnect()
        {
            lastError = "";

            if (mercuryDeviceId.Length > 0)
            {
                log.Debug("Отключение Меркурия");
                if (!mercury.Close(mercuryDeviceId))
                {
                    processError();
                    return false;
                }
            }

            return true;
        }

        public bool printTextDocument(string doc)
        {
            log.Debug("Печать текстового документа " + doc);
            lastError = "";
            if (!mercury.PrintTextDocument(mercuryDeviceId, doc))
            {
                processError();
                return false;
            }

            return true;
        }

        public bool getCurrentStatus(out JObject status)
        {
            log.Debug("Запрос текущего состояния регистратора");

            lastError = "";
            status = new JObject();

            int checkNumber = 0;
            int sessionNumber = 0;
            int sessionState = 0;
            string statusParameters = "";
            if(!mercury.GetCurrentStatus(mercuryDeviceId, out checkNumber, out sessionNumber, out sessionState, out statusParameters))
            {
                processError();
                return false;
            }

            status["CheckNumber"] = checkNumber;
            status["SessionNumber"] = sessionNumber;
            status["SessionState"] = sessionState;

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(statusParameters);
            XmlNode node = doc.DocumentElement.SelectSingleNode("/StatusParameters/Parameters");
            status["BacklogDocumentsCounter"] = node.Attributes["BacklogDocumentsCounter"]?.InnerText ?? "0";
            status["BacklogDocumentFirstNumber"] = node.Attributes["BacklogDocumentFirstNumber"]?.InnerText ?? "";
            status["BacklogDocumentFirstDateTime"] = node.Attributes["BacklogDocumentFirstDateTime"]?.InnerText ?? "";

            log.Debug("Текущее состояние: " + status.ToString());

            return true;
        }

        public bool openShift(JObject parameters, out JObject result)
        {
            log.Debug("Открытие смены");

            lastError = "";
            result = new JObject();

            XDocument openShiftInDoc = new XDocument();
            XElement xinputParameters = new XElement("InputParameters");
            XElement xparameters = new XElement("Parameters");
            if ((string)parameters["CashierName"] != null) {
                xparameters.Add(new XAttribute("CashierName", (string)parameters["CashierName"]));
            }
            if ((string)parameters["CashierVATIN"] != null)
            {
                xparameters.Add(new XAttribute("CashierVATIN", (string)parameters["CashierVATIN"]));
            }
            xinputParameters.Add(xparameters);
            openShiftInDoc.Add(xinputParameters);

            string openShiftOut = "";
            int sessionNumber = 0;
            int documentNumber = 0;

            if(!mercury.OpenShift2(mercuryDeviceId, openShiftInDoc.ToString(), out openShiftOut, out sessionNumber, out documentNumber))
            {
                processError();
                return false;
            }

            result["SessionNumber"] = sessionNumber;
            result["DocumentNumber"] = documentNumber;

            XmlDocument openShiftOutDoc = new XmlDocument();
            openShiftOutDoc.LoadXml(openShiftOut);
            XmlNode node = openShiftOutDoc.DocumentElement.SelectSingleNode("/OutputParameters/Parameters");
            result["UrgentReplacementFN"] = node.Attributes["UrgentReplacementFN"]?.InnerText ?? "";
            result["MemoryOverflowFN"] = node.Attributes["MemoryOverflowFN"]?.InnerText ?? "";
            result["ResourcesExhaustionFN"] = node.Attributes["ResourcesExhaustionFN"]?.InnerText ?? "";
            result["OFDtimeout"] = node.Attributes["OFDtimeout"]?.InnerText ?? "";

            log.Debug("Смена открыта. Данные смены: " + result.ToString());

            return true;
        }

        public bool closeShift(JObject parameters, out JObject result)
        {
            log.Debug("Закрытие смены");

            lastError = "";
            result = new JObject();

            XDocument closeShiftInDoc = new XDocument();
            XElement xinputParameters = new XElement("InputParameters");
            XElement xparameters = new XElement("Parameters");
            if ((string)parameters["CashierName"] != null)
            {
                xparameters.Add(new XAttribute("CashierName", (string)parameters["CashierName"]));
            }
            if ((string)parameters["CashierVATIN"] != null)
            {
                xparameters.Add(new XAttribute("CashierVATIN", (string)parameters["CashierVATIN"]));
            }
            xinputParameters.Add(xparameters);
            closeShiftInDoc.Add(xinputParameters);

            string closeShiftOut = "";
            int sessionNumber = 0;
            int documentNumber = 0;

            if (!mercury.CloseShift2(mercuryDeviceId, closeShiftInDoc.ToString(), out closeShiftOut, out sessionNumber, out documentNumber))
            {
                processError();
                return false;
            }

            result["SessionNumber"] = sessionNumber;
            result["DocumentNumber"] = documentNumber;

            XmlDocument closeShiftOutDoc = new XmlDocument();
            closeShiftOutDoc.LoadXml(closeShiftOut);
            XmlNode node = closeShiftOutDoc.DocumentElement.SelectSingleNode("/OutputParameters/Parameters");
            result["NumberOfChecks"] = node.Attributes["NumberOfChecks"]?.InnerText ?? "";
            result["NumberOfDocuments"] = node.Attributes["NumberOfDocuments"]?.InnerText ?? "";
            result["BacklogDocumentsCounter"] = node.Attributes["BacklogDocumentsCounter"]?.InnerText ?? "";
            result["BacklogDocumentFirstNumber"] = node.Attributes["BacklogDocumentFirstNumber"]?.InnerText ?? "";
            result["BacklogDocumentFirstDateTime"] = node.Attributes["BacklogDocumentFirstDateTime"]?.InnerText ?? "";
            result["UrgentReplacementFN"] = node.Attributes["UrgentReplacementFN"]?.InnerText ?? "";
            result["MemoryOverflowFN"] = node.Attributes["MemoryOverflowFN"]?.InnerText ?? "";
            result["ResourcesExhaustionFN"] = node.Attributes["ResourcesExhaustionFN"]?.InnerText ?? "";
            result["ResourcesFN"] = node.Attributes["ResourcesFN"]?.InnerText ?? "";
            result["OFDtimeout"] = node.Attributes["OFDtimeout"]?.InnerText ?? "";

            log.Debug("Смена закрыта. Данные смены: " + result.ToString());

            return true;
        }

        public bool printXReport()
        {
            log.Debug("Печать X-отчета");

            lastError = "";
            if(!mercury.PrintXReport(mercuryDeviceId))
            {
                processError();
                return false;
            }

            return true;
        }

        public bool printCheck(JObject parameters, out JObject result)
        {
            log.Debug("Печать чека: " + parameters.ToString());

            if (!getCurrentStatus(out result))
            {
                processError();
                return false;
            }

            if((int) result["SessionState"] == 1)
            {
                if(!openShift(parameters, out result))
                {
                    processError();
                    return false;
                }
            }

            lastError = "";
            result = new JObject();

            string cashierName = (string)parameters["CashierName"] ?? "";
            string checkPackage = (string)parameters["CheckPackage"] ?? "";

            int checkNumber = 0;
            int sessionNumber = 0;
            string fiscalSign = "";
            string addressSiteInspections = "";

            if (!mercury.ProcessCheck(mercuryDeviceId, cashierName, true, checkPackage, out checkNumber, out sessionNumber, out fiscalSign, out addressSiteInspections))
            {
                string message = "";
                int errorCode = mercury.GetLastError(out message);

                if(errorCode == 278) // смена открыта больше 24 часов, закрываем и пробуем напечатать чек еще раз.
                {
                    JObject closeParameters = new JObject();
                    closeParameters["CashierName"] = cashierName;

                    Regex cashierVATINReg = new Regex("CashierVATIN=\"(.*?)\"");
                    var vatinnmatch = cashierVATINReg.Match(checkPackage);
                    if (vatinnmatch.Success)
                    {
                        closeParameters["CashierVATIN"] = vatinnmatch.Groups[1].ToString();
                    }

                    JObject closeResult = new JObject();

                    if (!closeShift(closeParameters, out closeResult))
                    {
                        processError();
                        return false;
                    }

                    return printCheck(parameters, out result);
                }

                processError();
                return false;
            }

            result["SessionNumber"] = sessionNumber;
            result["CheckNumber"] = checkNumber;
            result["FiscalSign"] = fiscalSign;
            result["AddressSiteInspections"] = addressSiteInspections;

            log.Debug("Чек напечатан");

            return true;
        }

        private void processError()
        {
            string message = "";
            int errorCode = mercury.GetLastError(out message);
            processError(message + " (" + errorCode + ")");
        }

        private void processError(string message)
        {
            lastError = message;
            log.Error(message);
        }

    }
}
