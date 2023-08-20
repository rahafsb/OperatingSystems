using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;


namespace ThreadedBinarySearchTree
{

    class Node
    {
        private Node father;
        private Node left;
        private Node right;
        private int data;

        public Node(int data_)
        {
            this.data = data_;
            this.father = null;
            this.right = null;
            this.left = null;

        }
        public void set_father(Node father_)
        {
            this.father = father_;
        }
        public void set_right(Node right_)
        {
            this.right = right_;
        }
        public void set_left(Node left_)
        {
            this.left = left_;
        }

        public int get_data()
        {
            return this.data;
        }
        public Node get_right()
        {
            return this.right;
        }
        public Node get_left()
        {
            return this.left;
        }
        public Node get_father()
        {
            return this.father;
        }
        public void set_data(int num)
        {
            this.data = num;
        }



    }
    class ThreadedBinarySearchTree
    {
        private Node root;
        static ReaderWriterLock mut = new ReaderWriterLock();
        static Mutex m = new Mutex();

        public ThreadedBinarySearchTree()
        {
            this.root = null;
        }
        public ThreadedBinarySearchTree(Node root_)
        {
            this.root = root_;
        }
        public bool search(int num)
        {
            mut.AcquireReaderLock(1000);
           
            Node at = this.root;
            while (at != null)
            {
                if (num == at.get_data())
                {
                    
                    mut.ReleaseReaderLock();
                    return true;

                }
                else if (num < at.get_data())
                {
                    at = at.get_left();
                }
                else
                {
                    at = at.get_right();
                }
            }
            mut.ReleaseReaderLock();
            return false;
        }

        public void add(int num)
        {
            // if search is false the add 
            if (!search(num)) //read 
            {
                
                mut.AcquireWriterLock(1000); // write
                Node add_ = new Node(num);
                if (root == null)
                {
                    this.root = add_;
                }
                else
                {
                    Node at = this.root;
                    Node here = at;
                    while (at != null)
                    {
                        if (num < at.get_data())
                        {
                            here = at;
                            at = at.get_left();
                        }
                        else if (num > at.get_data())
                        {
                            here = at;
                            at = at.get_right();
                        }
                    }
                    if (num > here.get_data())
                    {
                        here.set_right(add_);
                        add_.set_father(here);
                    }
                    else if (num < here.get_data())
                    {
                        here.set_left(add_);
                        add_.set_father(here);
                    }

                }
                
                mut.ReleaseWriterLock();
            }
        }

        public int minValue(Node root)
        {
            int minv = root.get_data();
            while (root.get_left() != null)
            {
                minv = root.get_left().get_data();
                root = root.get_left();
            }
            return minv;
        }
        public Node helper(Node at, int num)
        {
            if (at == null) { return null; }
            else if (num < at.get_data())
            {
                at.set_left(helper(at.get_left(), num));
            }
            else if (num > at.get_data())
            {
                at.set_right(helper(at.get_right(), num));
            }
            else
            {
                if (at.get_right() == null && at.get_left() == null)
                {
                    at = null;
                }
                else if (at.get_left() == null)
                {
                    Node tmp = at;
                    at = at.get_right();
                    at.set_father(tmp.get_father());
                    at.set_right(null);
                }
                else if (at.get_right() == null)
                {
                    Node tmp = at;
                    at = at.get_left();
                    at.set_father(tmp.get_father());
                    at.set_left(null);


                }
                else
                {
                    at.set_data(minValue(at));
                    at.set_right(helper(at.get_right(), minValue(at)));

                }
            }
            return at;

        }
        public void remove(int num)
        {
            
            mut.AcquireWriterLock(1000);
            
            this.root = helper(this.root, num);
            mut.ReleaseWriterLock();
            
        }
        public virtual void clear() // remove all items from tree
        {
            try
            {
                
                mut.AcquireWriterLock(1000);
             
                root = null;
            }
            finally
            {
                mut.ReleaseWriterLock();
            }

        }

        public void print() // inorder left me right
        {
            
            mut.AcquireReaderLock(1000);
            m.WaitOne();
            inorder(this.root);
            Console.WriteLine("");
            m.ReleaseMutex();
            mut.ReleaseReaderLock();

        }
        public void inorder(Node at)
        {
            if (at == null) { return; }
            inorder(at.get_left());
            Console.Write(" " + at.get_data());
            inorder(at.get_right());
        }
    }

}

