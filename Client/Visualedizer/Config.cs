using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IniParser;
using IniParser.Model;

namespace Ledqualizer
{
    internal class Config
    {
        private Config instance;
        private const string IniFileName = "config.ini";
        private const string IniSectionSettings = "Settings";
        private const string IniSectionScreenCapture = "ScreenCapture";
        private const string IniSectionScreenCaptureOther = "ScreenCaptureOther";

        public string ipAddress { get; set; }
        public int ledCount { get; set; }
        public int delay { get; set; }
        public int port { get; set; } = 81;
        public float brightness { get; set; }
        public float normalizationLevel { get; set; }
        public int screenCaptureRow { get; set; }

        public int strobeTriggerX { get; set; }
        public int strobeTriggerY { get; set; }

        public int laserTriggerX { get; set; }
        public int laserTriggerY { get; set; }
        public int laserPatternX { get; set; }
        public int laserPatternY { get; set; }
        public int laserColorY { get; set; }
        public int laserColorX { get; set; }

        public void LoadFromIni()
        {
            if (File.Exists(IniFileName))
            {
                var parser = new FileIniDataParser();
                IniData data = parser.ReadFile("config.ini");

                ipAddress = data[IniSectionSettings]["ipAddress"];
                ledCount = int.Parse(data[IniSectionSettings]["ledCount"]);
                delay = int.Parse(data[IniSectionSettings]["delay"]);
                port = int.Parse(data[IniSectionSettings]["port"]);
                brightness = float.Parse(data[IniSectionSettings]["brightness"]);
                normalizationLevel = float.Parse(data[IniSectionSettings]["normalizationLevel"]);

                screenCaptureRow = int.Parse(data[IniSectionScreenCapture]["screenCaptureRow"]);

                strobeTriggerX = int.Parse(data[IniSectionScreenCaptureOther]["strobeTriggerX"]);
                strobeTriggerY = int.Parse(data[IniSectionScreenCaptureOther]["strobeTriggerY"]);

                laserTriggerX = int.Parse(data[IniSectionScreenCaptureOther]["laserTriggerX"]);
                laserTriggerY = int.Parse(data[IniSectionScreenCaptureOther]["laserTriggerY"]);
                laserPatternX = int.Parse(data[IniSectionScreenCaptureOther]["laserPatternX"]);
                laserPatternY = int.Parse(data[IniSectionScreenCaptureOther]["laserPatternY"]);
                laserColorX = int.Parse(data[IniSectionScreenCaptureOther]["laserColorX"]);
                laserColorY = int.Parse(data[IniSectionScreenCaptureOther]["laserColorY"]);
            }
        }

        public void SaveToIni()
        {
            var parser = new FileIniDataParser();

            IniData data;
            if (File.Exists(IniFileName))
            {
                data = parser.ReadFile(IniFileName);
            } 
            else
            {
                data = new IniData();
            }
                
            data[IniSectionSettings]["ipAddress"] = ipAddress;
            data[IniSectionSettings]["ledCount"] = ledCount.ToString();
            data[IniSectionSettings]["delay"] = delay.ToString();
            data[IniSectionSettings]["port"] = port.ToString();
            data[IniSectionSettings]["brightness"] = brightness.ToString();
            data[IniSectionSettings]["normalizationLevel"] = normalizationLevel.ToString();

            data[IniSectionScreenCapture]["screenCaptureRow"] = screenCaptureRow.ToString();

            data[IniSectionScreenCaptureOther]["strobeTriggerX"] = strobeTriggerX.ToString();
            data[IniSectionScreenCaptureOther]["strobeTriggerY"] = strobeTriggerY.ToString();

            data[IniSectionScreenCaptureOther]["laserTriggerX"] = laserTriggerX.ToString();
            data[IniSectionScreenCaptureOther]["laserTriggerY"] = laserTriggerY.ToString();
            data[IniSectionScreenCaptureOther]["laserPatternX"] = laserPatternX.ToString();
            data[IniSectionScreenCaptureOther]["laserPatternY"] = laserPatternY.ToString();
            data[IniSectionScreenCaptureOther]["laserColorX"] = laserColorX.ToString();
            data[IniSectionScreenCaptureOther]["laserColorY"] = laserColorY.ToString();

            parser.WriteFile(IniFileName, data);
        }

        /*public static Config GetInstance() 
        {
            if (instance == null)
            {
                instance = new Config();
            }
            return instance;
        }*/
    }


}
