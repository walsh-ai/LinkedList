using System.Text;
using System.Transactions;

/// <summary>
/// This class implements ICollection so that it is iterable and
/// to implement useful operations requied such as add, remove, Contains,
/// GetEnumerator, Sort 
/// </summary>
/// <typeparam name="T"></typeparam>
public class SinglyLinkedList<T>:ICollection<T>
{
    #region private fields
    // Track the first and last node and
    // list length
    protected ListNode<T>? firstNode;
    protected ListNode<T>? lastNode;
    protected int count; 
    #endregion 
    
    #region ICollection properties 
    public bool IsReadOnly { get; }
    #endregion

    /// <summary>
    /// Property to mask get first node in the list
    /// </summary>
    public ListNode<T>? FirstNode {
        get { return firstNode; }
    }

    /// <summary>
    /// Property to mask get last node in the list 
    /// </summary>
    public ListNode<T>? LastNode {
        get {return lastNode; }
    }

    /// <summary>
    /// Property to hold the count of items in the list 
    /// </summary>
    public int Count 
    {
        get { return count; }
    }

    /// <summary>
    /// Indexer with square brackets accessor notation 
    /// to iterate the list and fetch the nth item 
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public T this[int index]
    {
        get 
        {
            // Exception where index is less than 0 (first node)
            if (index < 0)
                throw new ArgumentOutOfRangeException(); 

            if (IsEmpty) 
                throw new ApplicationException("Cannot index empty list."); 
            
            // Begin iteration from first node with reference object 
            // Traverse to the next node until index or null reference 
            ListNode<T> currentNode = firstNode;
            for (int i = 0; i < index; i++) {
                if (currentNode.Next == null) 
                    throw new ArgumentOutOfRangeException(); 
                currentNode = currentNode.Next; 
            }
            return currentNode.Item; 
        }
    }

    /// <summary>
    /// Property to determine if the list is empty 
    /// </summary>
    public bool IsEmpty
    {
        get
        {
            // Do not allow other operations on the object while the 
            // list is read (avoid race condition)
            lock (this) 
            {
                return firstNode == null; 
            }
        }
    }

    /// <summary>
    /// Constructor for empty list
    /// </summary>
    public SinglyLinkedList() {
        count = 0; 
        IsReadOnly = false; 
    }

    /// <summary>
    /// Override ToString to print entire list 
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        if (IsEmpty) 
            return string.Empty; 
        // Retrieve contents of list items as string 
        StringBuilder returnString = new StringBuilder();
        // Use iterator over list nodes 
        foreach (T item in this) {
            if (returnString.Length > 0) {
                returnString.Append("->");
            }
            returnString.Append(item);
        }
        return returnString.ToString(); 
    }

    #region Implement members required by ICollections 
    /// <summary>
    /// ICollections Add masks InsertAtFront method
    /// </summary>
    /// <param name="item"></param>
    public void Add(T item) {
        InsertAtBack(item); 
    }

    /// <summary>
    /// Clear all items from the list by setting first/last pointers
    /// to null 
    /// </summary>
    public void Clear() {
        firstNode = lastNode = null;
        count = 0; 
    }

    /// <summary>
    /// Thread safe
    /// Copies all elements of the list into an array starting at given 
    /// index 
    /// </summary>
    /// <param name="array"></param>
    /// <param name="index"></param>
    public void CopyTo(T[] array, int index) {
        if (IsEmpty) 
            return; 
        
        if (array == null)
            throw new ArgumentNullException("Destination array reference for CopyTo() is null."); 

        if (index < 0) 
            throw new IndexOutOfRangeException("Array index for CopyTo() is less than zero"); 
        
        if (array.Rank > 1) 
            throw new ArgumentException("Array for CopyTo() must be uni-dimensional.");
        
        // Number of elements in List is greater than array space from index
        if ((index + count) >= (array.Length)) 
            throw new ArgumentException("Array is not large enough to store collection elements from index."); 

        ListNode<T> current = firstNode;
        while (current.Next != null) {
            array.SetValue(current.Item, index++); 
        } 
    }
    #endregion 

    #region Implement operations on list 
        /// <summary>
        /// Thread safe method to add a node at the front of list 
        /// </summary>
        /// <param name="item"></param>
        public virtual void InsertAtFront(T item) {
            lock(this) {
                if (IsEmpty) 
                    firstNode = lastNode = new ListNode<T>(item); 
                else 
                    firstNode = new ListNode<T>(item, firstNode); 
                count++; 
            }
        }

        /// <summary>
        /// Thread safe method to add a node at the back of the list
        /// </summary>
        /// <param name="item"></param>
        public virtual void InsertAtBack(T item) {
            // Lock object for race conditions 
            lock(this) {
                if (IsEmpty) 
                    firstNode = lastNode = new ListNode<T>(item); 
                else 
                    // Create new list object and set references 
                    lastNode = lastNode.Next = new ListNode<T>(item); 
                count++; 
            }
        }

        /// <summary>
        /// Remove and return object from the from of list 
        /// Should this return null? In order to make the code explicit 
        /// this method will return an exception when removing from an empty list 
        /// </summary>
        public virtual T RemoveFromFront() {
            lock(this) {
                if (IsEmpty) 
                    throw new ApplicationException("Cannot remove from empty list.");
                
                T data = firstNode.Item; 
                
                if (firstNode == lastNode) 
                    firstNode = lastNode = null; 
                else 
                    firstNode = firstNode.Next; 
                
                count--; 
                return data; 
            }
        }

        /// <summary>
        /// Thread safe method to remove from the back of the list 
        /// </summary>
        /// <returns></returns>
        /// <exception cref="ApplicationException"></exception>
        public virtual T RemoveFromBack() {
            lock(this) {
                if (IsEmpty) 
                    throw new ApplicationException("Cannot remove from empty list."); 
                
                T data = lastNode.Item;

                if (firstNode == lastNode) 
                    firstNode = lastNode = null; 
                else {
                    // Set last node and preceeding reference to null 
                    ListNode<T> current = firstNode; 
                    while (current.Next != lastNode)
                        current = current.Next;
                    lastNode = current; 
                    current.Next = null; 
                }
                
                count--;
                return data; 
            }
        }

        /// <summary>
        /// Insert to the list at any valid index (front, back or middle)
        /// </summary>
        /// <param name="index"></param>
        /// <param name="item"></param>
        /// <exception cref="ApplicationException"></exception>
        public virtual void InsertAt(int index, T item) {
            lock(this) {
                if (index > count || index < 0) 
                    throw new ApplicationException($"Insert at index {index} out of list range."); 
                
                if (IsEmpty) 
                    InsertAtFront(item); 
                if (index == (count - 1))
                    InsertAtBack(item); 
                else {
                    int pos = 1; 
                    ListNode<T> current = firstNode; 
                    while (pos++ < index) 
                        current = current.Next; 
                    
                    current.Next = new ListNode<T>(item, current.Next); 
                }
                count++; 
            }
        }

        /// <summary>
        /// Thread safe method to remove at any valid list index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        /// <exception cref="ApplicationException"></exception>
        public virtual T RemoveAt(int index) {
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
                    ListNode<T> current = firstNode; 
                    while (pos++ < index) 
                        current = current.Next; 
                    
                    // The subsequent node is delted 
                    data = current.Next.Item; 
                    current.Next = current.Next.Next; 
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
        public virtual bool Remove(T item) {
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

                ListNode<T> current = firstNode;
                while (current.Next != null) {
                    if (current.Next.Item.ToString().Equals(item.ToString())) {
                        current.Next = current.Next.Next; 
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
        /// Thread safe method 
        /// Update item in list 
        /// </summary>
        /// <param name="oldItem"></param>
        /// <param name="newItem"></param>
        /// <returns></returns>
        /// <exception cref="ApplicationException"></exception>
        public bool Update(T oldItem, T newItem) {
            lock (this) {
                if (IsEmpty) 
                    throw new ApplicationException("Cannot update empty list"); 
                
                // Search for list node holding oldItem 
                ListNode<T> current = firstNode; 
                while (current.Next != null) {
                    if (current.Item.ToString().Equals(oldItem.ToString())) {
                        current.Item = newItem; 
                        return true;
                    }

                    current = current.Next; 
                }

                return false;
            }
        }

        /// <summary>
        /// Thread safe
        /// Check the if the list contains a specified item (linear search)
        /// </summary>
        /// <param name="item">The item to match on</param>
        /// <returns></returns>
        public bool Contains(T item) {
            lock (this) {
                ListNode<T> current = firstNode; 
                while (current != null) {
                    if (current.Item.ToString().Equals(item.ToString())) 
                        return true; 
                    current = current.Next; 
                }

                return false; 
            }
        }

        /// <summary>
        /// Thread safe
        /// Reverse the linked list element order 
        /// </summary>
        public virtual void Reverse() {
            lock (this) {
                if (IsEmpty || firstNode.Next == null) 
                    return; 
                
                lastNode = firstNode; 

                ListNode<T> prev = firstNode; 
                ListNode<T> current = firstNode.Next; 

                do {
                    ListNode<T> temp = current.Next; 

                    // Swap 
                    current.Next = prev; 
                    prev = current;
                    current = temp; 
                } while (current.Next != null);

                // Make first node equal to 'current' to complete reversal 
                firstNode = current; 
            }
        }

        /// <summary>
        /// Determine whether list contains a cycle 
        /// Loop Detection:
        /// Given a circular list, return the node at the beginning of the cycle 
        /// Definition: Circular linked list is a list in which a node's next pointer
        /// refers to an earlier node, creating a cycle 
        /// a -> b -> c -> d -> e -> f -> c
        /// Loop starts with c 
        /// Use a slow pointer and a fast pointer that makes two steps for every one slow step
        /// If at any iteration the two pointers overlap, the cycle is confirmed 
        /// </summary>
        /// <returns></returns>
        public bool HasCycle() {
            lock(this) {
                if (IsEmpty) 
                    return false; 
                
                ListNode<T> slow = firstNode; 
                ListNode<T> fast = firstNode; 

                while (fast.Next != null && fast.Next.Next != null) {
                    slow = slow.Next; 
                    fast = fast.Next.Next; 

                    if (slow == fast) 
                        return true; 
                }
                
                return false; 
            }
        }

        /// <summary>
        /// Remove duplicate elements from the sorted
        /// list
        /// ** TO DO : Implement sorting the list 
        /// </summary>
        public virtual void RemoveSortedDuplicates() {
            lock (this) {
                ListNode<T> current = firstNode; 
                ListNode<T> lastUnique = firstNode; 

                // Iterate list storing the last unique instance seen
                // Ignore duplicates (occur in order) 
                // When next unique value seen, set as next skipping duplicates 
                while (current.Next != null) {
                    current = current.Next; 

                    if (!(current.Item.ToString().Equals(lastUnique.Item.ToString()))) {
                        lastUnique.Next = current; 
                        lastUnique = current; 
                    }
                }
            }
        }

        /// <summary>
        /// Find the middle node 
        /// </summary>
        /// <returns></returns>
        public ListNode<T> FindMiddle() {
            lock (this) {
                ListNode<T> slow = firstNode; 
                ListNode<T> fast = firstNode; 

                while (fast.Next != null && fast.Next.Next != null) {
                    slow = slow.Next; 
                    fast = fast.Next.Next; 

                    // Check for cycle 
                    if (slow == fast) 
                        return null;

                    // Check for end condition 
                    // Fast has reached end 
                    if (fast == null || fast.Next == null) {
                        return slow; 
                    }
                }

                // If the loop is never entered, 
                //      return firstNode 
                return firstNode;
            }
        }
    #endregion 

    #region IEnumerable<T> Members
    /// <summary>
    /// GetEnumerator to suport foreach over list 
    /// Traverse the list and yield the current value 
    /// </summary>
    /// <returns></returns>
    public IEnumerator<T> GetEnumerator() {
        ListNode<T> current = firstNode; 
        while (current != null) {
            yield return current.Item; 
            current = current.Next; 
        }
    }

    System.Collections.IEnumerator
    System.Collections.IEnumerable.GetEnumerator() {
        return GetEnumerator(); 
    }
    #endregion
}