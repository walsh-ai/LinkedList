public class DoubleListNode<T> : ListNode<T> {
    private DoubleListNode<T>? prev; 

    // Hide ListNode<T> Next property as DoubleListNode<T>
    public new DoubleListNode<T>? Next {
        get { return (DoubleListNode<T>?)next; }
        set { next = value; }
    }

    /// <summary>
    /// Public property to get, set reference to next node 
    /// </summary>
    public DoubleListNode<T>? Prev { 
        get { return prev; } 
        set { prev = value; }
    }

    /// <summary>
    /// Constructor taking only an item and setting null pointers 
    /// </summary>
    /// <param name="item"></param>
    public DoubleListNode(T item) 
        : this(item, null, null) {
        }

    /// <summary>
    /// DoubleListNode constructor, calls parent constructor ListNode 
    /// Initialises previous node pointer 
    /// </summary>
    /// <param name="item"></param>
    /// <param name="next"></param>
    /// <param name="prev"></param>
    public DoubleListNode(T item, DoubleListNode<T>? next, DoubleListNode<T>? prev) 
        : base(item, next) {
        this.prev = prev; 
    }
}