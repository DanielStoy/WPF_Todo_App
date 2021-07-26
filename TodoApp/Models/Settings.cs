using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TodoApp.Models
{
    public class Settings
    {
        public bool StartUp { get; set; }

        public bool StartInBackground { get; set; }

        public bool ShowNotifications { get; set; }

        public Settings()
        {
            StartUp = false;
            StartInBackground = false;
            ShowNotifications = false;
        }

        public Settings(bool startUp, bool startInBackground, bool showNotifications)
        {
            StartUp = startUp;
            StartInBackground = startInBackground;
            ShowNotifications = showNotifications;
        }
    }
}
