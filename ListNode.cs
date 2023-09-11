/// <summary>
/// Class to represent linked list node 
/// </summary> 
/// <typeparam name="T"></typeparam>

public class ListNode<T> {
    // Reference to the next node 
    protected ListNode<T>? next; 
    // Item stored in list node
    protected T item; 

    /// <summary>
    /// Public property to get, set reference to next node 
    /// </summary>
    public ListNode<T>? Next { 
        get { return next; } 
        set { next = value; }
    }

    /// <summary>
    /// Public property to get, set node value 
    /// </summary>
    public T Item {
        get { return item; }
        set { item = value; }
    }

    /// <summary>
    /// Constructor with item init
    /// </summary>
    /// <param name="item"></param>
    public ListNode(T item) 
    : this(item, null) {
    }

    /// <summary>
    /// Constructor with item and the next node specified 
    /// </summary>
    public ListNode(T item, ListNode<T>? next) {
        this.item = item;
        this.next = next; 
    }

    /// <summary>
    /// Override ToString to print the node item value 
    /// </summary>
    public override string ToString()
    {
        if (item == null)
            return string.Empty; 
        return item.ToString(); 
    }
}