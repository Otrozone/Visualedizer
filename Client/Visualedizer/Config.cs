using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ledqualizer
{
    internal class Config
    {
        private static Config instance;

        public string ipAddress { get; set; }
        public int ledCount { get; set; }
        public int delay { get; set; }
        public int port { get; set; } = 81;
        public float brightness { get; set; }

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
