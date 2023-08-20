using System;
using System.Threading;
using System.Collections.Generic;

namespace MultiThreadedMergeSort
{
    class MultiThreadedMergeSort
    {
        //main mergesort function
        public Int64[] mergeSort(Int64[] array, int minElements) {

            //the length of our array
            int length = array.Length;
            List<Thread> threads = new List<Thread>();

            //this loop is to setup every thread and give each his beginning index and last index
            for (int i = 0; i < length; i += minElements)
            {
                int beg = i;
                int remain = length - i;
                int end;
                // to adapt the last thread
                if (remain < minElements)
                {
                   end  = i + (remain - 1);
                }
                else
                {
                    end = i + (minElements - 1);
                }
                Thread t = new Thread(() => mergeSort(array, beg, end, minElements));
                threads.Add(t);
            }

            foreach (Thread t in threads){
                try
                {
                    t.Start();
                    //without join the program will not work will, because in order to start mergeing, all the threads should end there work
                    t.Join();
                }
                catch(Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }

            //this loop is to merge all thread results and sort them toughter like any regual mergesort
            //from this point we are working with just one thread, it's too hard to do merge with more than one thread
            for (int i = 0; i < length; i+= minElements)
            {
                int mid = i;
                if (mid != 0)
                {
                    mid = i - 1;
                }
                int remain = length - i;
                int end = remain;
                if (remain < minElements)
                {
                    end = i + (remain - 1);
                }
                else
                {
                    end = i + (minElements - 1);
                }
                merge(array, 0, mid, end);
            }
            return array;
        }

        //regual mergesort to sort the results
        public static void mergeSort(Int64[] array, int begin, int end, int minElemnts)
        {
            if (begin < end)
            {
                int mid = (begin + end) / 2;
                mergeSort(array, begin, mid, minElemnts);
                mergeSort(array, mid + 1, end, minElemnts);
                merge(array, begin, mid, end);
            }
        }

        //regular merge
        public static void merge(Int64[] array, int begin, int mid, int end)
        {
            Int64[] temp = new long[end - begin + 1];
            int i = begin;
            int j = mid + 1;
            int k = 0;

            //Add elements from first half or second half based on lower to higher elements,
            //do until one of the list is finished and no more comparisons could be made
            while (i <=mid && j <= end)
            {
                if(array[i] <= array[j])
                {
                    temp[k] = array[i];
                    i += 1;
                }
                else
                {
                    temp[k] = array[j];
                    j += 1;
                }
                k += 1;
            }
            //add to temp array from first half if second half finished earlier
            while (i <= mid)
            {
                temp[k] = array[i];
                i += 1;
                k += 1;
            }
            //add to temp array from second half if first half finished earlier
            while (j <= end)
            {
                temp[k] = array[j];
                j += 1;
                k += 1;
            }
            //copy
            for (i = begin, k = 0; i <= end; i++, k++)
            {
                array[i] = temp[k];
            }
        }

    }
 
}
