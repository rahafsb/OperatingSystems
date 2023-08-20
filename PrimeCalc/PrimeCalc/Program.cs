using System;
using System.Threading;
using System.IO;
using System.Text;


namespace PrimeCalc    //<N> <M> <nThreads>
{
    class Program
    {

        static void Main(string[] args)
        {
            Thread[] th_array = new Thread[1];

                if(Int64.Parse(args[2]) <= 0)
            {
                Console.WriteLine("Number of threads must be larger than 0.");
                    
            }

            if (Int64.Parse(args[1]) < Int64.Parse(args[0]))
            {
                Console.WriteLine("Invalid range!");
                    

            }

            if (Int64.Parse(args[2]) > 0)
            {
                th_array = new Thread[Int64.Parse(args[2])];
            }

            if (Int64.Parse(args[2]) > 0 && Int64.Parse(args[1]) > Int64.Parse(args[0]))
            {

                long prev = Int64.Parse(args[0]);
                long max = Int64.Parse(args[1]);
                long set = (Int64.Parse(args[1]) - Int64.Parse(args[0])) / Int64.Parse(args[2]);
                for (int i = 1; i <= Int64.Parse(args[2]); i++)
                {
                    long local_1 = prev;
                    long local_2 = local_1 + set;
                    prev = local_2 + 1;
                    if(local_2 > max) { local_2 = max; }
                    String name_ = i + "";
                    th_array[i-1] = new Thread(() =>  func_1(local_1, local_2, name_));
                    th_array[i-1].Name = name_;

                }
              
                for (int j = 0; j < Int64.Parse(args[2]); j++)
                {
                    th_array[j].Start();
                }
                   
            }
        }
       

        public static void  func_1(long s, long e, String id)//(long s, long e)
        {
           
            for (long i = s; i <= e; i++)
            {
                Boolean b = isPrime(i);
                if (b)
                {

                    Console.WriteLine("Thread[" + id + "]:" + i + "");
                   

                }
              
            }


        }
        public static bool isPrime(long num)
        {
            if(num == 0 || num == 1)
            {
                return false;
            }
            if(num == 2)
            {
                return true;
            }
            for (long i = 2; i < num; i++)
                if (num % i == 0)
                    return false;
            return true;
        }
    }
}