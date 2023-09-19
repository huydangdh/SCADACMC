using SCADACMC.Control;
using System.Collections.Generic;
using System.Linq;

namespace SCADACMC.Class
{
    public class CMCManager
    {
        public List<CMCControl> CMCs = new List<CMCControl>();
        private static CMCManager m_cmcmanger;

        public void AddGW28(CMCControl model)
        {
            if (!CMCs.Contains(model))
            {
                CMCs.Add(model);
            }
        }

        public CMCControl GetCMCByIP(string ip)
        {
            CMCControl cmc = null;
            if (!string.IsNullOrEmpty(ip) && ip[0] == '1')
            {
                cmc = this.CMCs.Where(x => x.IP == ip).First();
                if (cmc == null)
                {
                    throw new System.Exception("CMC_IP: " + ip + " IS NULL");
                }
            } // cmc Alarm
            else
            {
                var tempStr = string.Empty;
                if (ip.Substring(0, 3) == "B31")
                {
                    tempStr = ip.Substring(0, 6) + "W1";
                }
                else if (ip.Length == 8)
                {
                    tempStr = ip.Substring(0, 6) + "Q1";
                }

                if (HSCADAFLAG.IsUsePrgAlarmME)
                {
                    cmc = this.CMCs.Where(x => HSCADAFLAG.G_AlarmPrgMEIp.Contains(x.IP)).FirstOrDefault();
                    if (cmc == null)
                    {
                        throw new System.Exception("CMC_IP ALARMS: " + ip + " IS NULL");
                    }
                }
            }
            return cmc;
        }

        public static CMCManager GetInstance()
        {
            if (m_cmcmanger == null)
            {
                m_cmcmanger = new CMCManager();
                return m_cmcmanger;
            }
            else
            {
                return m_cmcmanger;
            }
        }


    }
}
