using System;
using System.Collections.Generic;

namespace TodoApi;

public partial class Item
{


    public int Id { get; set; }

    public string? Name { get; set; }

    public bool? IsComplete { get; set; }

    static int p = 100;

    public Item()
    {

    }
    public Item(string todo)
    {
        p++;
        this.Id = p;
        this.Name = todo;
        this.IsComplete = false;
    }
}
