using System.Runtime.Intrinsics.Arm;
using System.Text;
using System.Transactions;

/// <summary>
/// DoublyLinkedList Implementation where next and previous node are 
/// tracked 
/// This class implements SinglyLinkedList<T> : ICollection<T>> so that it is iterable and
/// to implement useful operations requied such as add, remove, Contains,
/// GetEnumerator, Sort 
/// </summary>
/// <typeparam name="T">Generic type of list node item</typeparam>
public class DoublyLinkedList<T> : SinglyLinkedList<T> {
    #region private field/property
    // Inherited fields from SinglyLinkedList 
    // Hide ListNode<T> properties replacing with properties that 
    //      cast fields as DoubleListNode<T> 
    private new DoubleListNode<T>? FirstNode {
        get { return (DoubleListNode<T>?)firstNode; }
        set { firstNode = value; }
    }

    private new DoubleListNode<T>? LastNode {
        get { return (DoubleListNode<T>?)lastNode; }
        set { lastNode = value; }
    }
    #endregion 

    #region ICollection fields
    // Inherited from SinglyLinkedList (IsReadOnly)
    #endregion 

    #region constructors 
    public DoublyLinkedList() 
        : base() {
        }
    #endregion 

    #region private methods
    #endregion 

    #region public methods
    /// <summary>
    /// Thread safe method to add a node at the front of list 
    /// </summary>
    /// <param name="item"></param>
    public override void InsertAtFront(T item) {
        lock(this) {
            if (IsEmpty) 
                 FirstNode = LastNode = new DoubleListNode<T>(item); 
            else 
                firstNode = new DoubleListNode<T>(item, FirstNode, null); 
            count++; 
        }
    }

    /// <summary>
    /// Thread safe method to add node at the back of the list 
    /// </summary>
    /// <param name="item"></param>
    public override void InsertAtBack(T item) {
        lock (this) {
            if (IsEmpty) 
                FirstNode = LastNode = new DoubleListNode<T>(item); 
            else 
                LastNode = new DoubleListNode<T>(item, null, LastNode); 
            count++; 
        }
    }

    /// <summary>
    /// Remove and return object from the from of list 
    /// Should this return null? In order to make the code explicit 
    /// this method will return an exception when removing from an empty list 
    /// </summary>
    public override T RemoveFromFront() {
        lock(this) {
            if (IsEmpty) 
                throw new ApplicationException("Cannot remove from empty list.");
            
            T data = FirstNode.Item; 
            
            if (FirstNode == LastNode) 
                FirstNode = LastNode = null; 
            else 
                FirstNode = FirstNode.Next; 
                FirstNode.Prev = null; 
            
            count--; 
            return data; 
        }
    }

    /// <summary>
    /// Thread safe method to remove from the back of the list 
    /// </summary>
    /// <returns></returns>
    /// <exception cref="ApplicationException"></exception>
    public override T RemoveFromBack() {
        lock(this) {
            if (IsEmpty) 
                throw new ApplicationException("Cannot remove from empty list."); 
            
            T data = LastNode.Item;

            if (FirstNode == LastNode) 
                FirstNode = LastNode = null; 
            else {
                // Make last node equal to its previous
                LastNode = LastNode.Prev; 
                LastNode.Next = null; 
            }
            
            count--;
            return data; 
        }
    }

    /// <summary>
    /// Insert list node at the given index 
    /// Overriden from SinglyLinkedList to use prev node pointer 
    /// </summary>
    /// <param name="index">Index to insert at</param>
    /// <param name="item">List node item to insert</param>
    public override void InsertAt(int index, T item) {
        lock (this) {
            if (index >= count || index < 0)
                throw new ArgumentOutOfRangeException($"Cannot insert at index {index} of list of size {count}"); 
            
            if (IsEmpty)
                if (index == 0) {
                    InsertAtFront(item); 
                    return; 
                } else 
                    throw new ArgumentOutOfRangeException($"Cannot insert at index {index} of empty list."); 
            
            if (index == (count - 1)) {
                InsertAtBack(item); 
                return; 
            }

            // Iterate up to index and insert list node 
            int pos = 0; 
            DoubleListNode<T>? current = FirstNode; 
            while (pos++ < index) 
                current = current.Next; 
            
            // Create new node and insert between current and current.Next 
            DoubleListNode<T> node = new DoubleListNode<T>(item, current.Next, current); 
            node.Next.Prev = node; 
            node.Prev.Next = node; 
        }
    }

    /// <summary>
    /// Thread safe method to remove at any valid list index
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    /// <exception cref="ApplicationException"></exception>
    public override T RemoveAt(int index) {
        lock (this) {
            if (index > count || index < 0) 
                throw new ApplicationException($"Index {index} out of list range.");
            
            T data; 
            if (index == 0) 
                return RemoveFromFront(); 
            else if (index == (count - 1))
                return RemoveFromBack(); 
            else {
                int pos = 1; 
                DoubleListNode<T> current = FirstNode; 
                while (pos++ < index) 
                    current = current.Next; 
                
                // The subsequent node is delted 
                data = current.Next.Item; 
                current.Next = current.Next.Next; 
                current.Next.Prev = current; 
            }

            count--;
            return data; 
        }
    }

    /// <summary>
    /// Thread safe method on list object
    /// Removes the list node with specified item if exists 
    /// </summary>
    /// <param name="item"></param>
    /// <returns>Boolean: True if item found and removed, false otherwise.</returns>
    /// <exception cref="ApplicationException"></exception>
    public override bool Remove(T item) {
        lock (this) {
            if (IsEmpty) 
                throw new ApplicationException("Cannot remove from empty list");
            
            if (firstNode.Item.ToString().Equals(item.ToString())) {
                RemoveFromFront();
                return true;
            } 
            if (lastNode.Item.ToString().Equals(item.ToString())) {
                RemoveFromBack(); 
                return true; 
            }

            DoubleListNode<T> current = FirstNode;
            while (current.Next != null) {
                if (current.Next.Item.ToString().Equals(item.ToString())) {
                    current.Next = current.Next.Next; 
                    current.Next.Prev = current; 
                    count--; 

                    // If current is now the last node
                    if (current.Next == null) 
                        lastNode = current;
                    
                    return true; 
                }

                current = current.Next; 
            }

            return false; 
        }
    }

    /// <summary>
    /// Thread safe
    /// Reverse the linked list element order 
    /// </summary>
    public override void Reverse() {
        lock (this) {
            if (IsEmpty || FirstNode.Next == null) 
                return; 
            
            DoubleListNode<T> current, temp; 
            
            LastNode = FirstNode;
            current = FirstNode.Next;  
            do {
                temp = current.Next; 
                current.Next = current.Prev;
                current.Prev = temp;

                current = temp; 
            } while (current.Next != null);

            // Make first node equal to 'current' to complete reversal 
            firstNode = current; 
        }
    }
    #endregion

    #region IColleciton methodsdd
    #endregion 

    #region IEnumerable 
    #endregion 
}