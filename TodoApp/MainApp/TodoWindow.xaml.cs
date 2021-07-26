using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using TodoApp.Models;
using TodoApp.Windows;

namespace TodoApp
{
    public partial class MainWindow : Window
    {
        public ObservableCollection<Todo> TodoTodos { get; set; }
        public ObservableCollection<Todo> DoingTodos { get; set; }
        public ObservableCollection<Todo> DoneTodos { get; set; }

        public string selectedItem { get; set; }

        public MainWindow()
        {
            SortTodos();
            InitializeComponent();
            this.DataContext = this;
        }

        private void SortTodos()
        {
            List<Todo> todos = Models.Application.Instance.allTodos;
            TodoTodos = new ObservableCollection<Todo>();
            DoingTodos = new ObservableCollection<Todo>();
            DoneTodos = new ObservableCollection<Todo>();

            foreach(Todo todo in todos)
            {
                switch (todo.Status)
                {
                    case TodoStatus.TODO:
                        TodoTodos.Add(todo);
                        break;
                    case TodoStatus.DOING:
                        DoingTodos.Add(todo);
                        break;
                    case TodoStatus.DONE:
                        DoneTodos.Add(todo);
                        break;
                }
            }
        }

        private void Todo_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Border b = (Border)sender;
            Todo newTodo = (Todo)b.DataContext;
            DataObject data = new DataObject(newTodo);
            DragDrop.DoDragDrop(b, data, DragDropEffects.Move);
            selectedItem = newTodo.Name;
        }

        private void Todo_Drop(object sender, DragEventArgs e)
        {
            Todo moveTodo = (Todo)e.Data.GetData(typeof(Todo));
            StackPanel s = (StackPanel)sender;

            switch (moveTodo.Status)
            {
                case TodoStatus.TODO:
                    TodoTodos.Remove(moveTodo);
                    break;
                case TodoStatus.DOING:
                    DoingTodos.Remove(moveTodo);
                    break;
                case TodoStatus.DONE:
                    DoneTodos.Remove(moveTodo);
                    break;
            }

            switch(s.Name)
            {
                case "TodoArea":
                    moveTodo.Status = TodoStatus.TODO;
                    TodoTodos.Add(moveTodo);
                    break;
                case "DoingArea":
                    moveTodo.Status = TodoStatus.DOING;
                    DoingTodos.Add(moveTodo);
                    break;
                case "DoneArea":
                    moveTodo.Status = TodoStatus.DONE;
                    DoneTodos.Add(moveTodo);
                    break;
            }
        }

        private void AddTodoWindow(object sender, RoutedEventArgs e)
        {
            AddTodo p = new AddTodo();
            p.Show();
        }

        private void ShowSettings(object sender, RoutedEventArgs e)
        {
            SettingsWindow settings = new SettingsWindow();
            settings.Show();
        }

        private void RemoveTodo(object sender, RoutedEventArgs e)
        {
            Todo removeTodo = (Todo)(sender as Button).DataContext;
            switch (removeTodo.Status)
            {
                case TodoStatus.TODO:
                    TodoTodos.Remove(removeTodo);
                    break;
                case TodoStatus.DOING:
                    DoingTodos.Remove(removeTodo);
                    break;
                case TodoStatus.DONE:
                    DoneTodos.Remove(removeTodo);
                    break;
            }
            Models.Application.Instance.allTodos.Remove(removeTodo);
        }

        public void AddTodo(Todo todo)
        {
            TodoTodos.Add(todo);
            Models.Application.Instance.allTodos.Add(todo);
        }

        private void MinimizeWindow(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void CloseWindow(object sender, RoutedEventArgs e)
        {
            if (!Models.Application.Instance.SettingsFile.StartInBackground)
            {
                this.Close();
            }
            else
            {
                this.Hide();
            }
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
