using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TodoApp.Models
{
    class Application
    {
        private static Application instance = null;

        public Settings SettingsFile { get; set; }

        public List<Todo> allTodos { get; set; }

        Application()
        {
            allTodos = new List<Todo>();
        }

        public static Application Instance
        {
            get
            {
                if(instance == null)
                {
                    instance = new Application();
                }
                return instance;
            }
        }
    }
}
