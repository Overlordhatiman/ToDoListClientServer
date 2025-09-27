using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToDoListClientApp.Models
{
    public class ToDoItemModel
    {
        public int Id { get; set; }
        public string? Description { get; set; }
        public bool IsCompleted { get; set; }
    }
}