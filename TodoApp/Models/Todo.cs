using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TodoApp.Models
{
    public enum TodoStatus
    {
        TODO = 0,
        DOING = 1,
        DONE = 2
    }

    public class Todo
    {
        public DateTime EndDate { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public TodoStatus Status { get; set; }

        public Guid Id { get; }

        public Todo(DateTime endDate, string name, string description)
        {
            EndDate = endDate;
            Name = name;
            Description = description;
            Status = TodoStatus.TODO;
            Id = new Guid();
        }
    }
}
