using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using TodoApp.Models;

namespace TodoApp.Windows
{
    /// <summary>
    /// Interaction logic for AddTodo.xaml
    /// </summary>
    public partial class AddTodo : Window
    {
        public DateTime Date { get; set; }
        public string TodoName { get; set; }
        public string Description { get; set; }
        public AddTodo()
        {
            Date = DateTime.Now;
            InitializeComponent();
            this.DataContext = this;
        }

        private void DateChanged(object sender, SelectionChangedEventArgs e)
        {
            DatePicker picker = (DatePicker)sender;
            Date = (picker.SelectedDate != null) ? (DateTime)picker.SelectedDate : DateTime.Now;
        }

        private void AddTodoToList(object sender, RoutedEventArgs e)
        {
            var window = System.Windows.Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();
            window.AddTodo(new Todo(Date, TodoName, Description));
            this.Close();
        }

        private void CloseWindow(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void DragWindow(object sender, MouseButtonEventArgs e)
        {
            if(e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
        }
    }
}
