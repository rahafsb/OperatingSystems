using System;
using System.Threading;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;


namespace MultiThreadedSearch
{
    class Program
    {
        static string use_text; //a static variable so every thread can access the text
        static int thread_counter = 0; //a static variable that let us know when all the threads are executed
        static int is_printed = 0; //a static variable to check wether we should print "not found"
        static void Main(string[] args)
        {
            string path = args[0];
            string word_to_search = "";
            for (int word = 1; word < args.Length - 2; word++) //discipline word_to_search
            {
                word_to_search += args[word];
                if (args.Length > 4 && word + 1 != args.Length - 2)
                {
                    word_to_search += " ";
                }
            }
            int num_of_threads = int.Parse(args[args.Length - 2]);
            int delay = int.Parse(args[args.Length - 1]);

            Thread[] n_th = new Thread[num_of_threads]; //out thread list
            string text = File.ReadAllText(path);
            use_text = text;
            int text_len = text.Length;
            int letter_per_thread = text_len / num_of_threads; //how much letters should every thread have

            int[] x_start = new int[num_of_threads]; //give every thread information about it's start line position in the text
            int[] y_start = new int[num_of_threads]; //give every thread information about it's start line index (y) position in the text
            int cur_x = 0; //current x position (which line)
            int cur_y = -1; //current y position (index in line)
            int counter = 0;
            x_start[0] = 0; //first thread start at [0, 0]
            y_start[0] = 0;
            int idx = 1;
            foreach (char c in use_text)
            {
                if (counter == letter_per_thread && idx < num_of_threads) //add to x_start and y_start when new thread appear
                {
                    counter = 0;
                    if (c == '\n') //if the new thread start with the letter new line '\n'
                    {
                        cur_x += 1;
                        cur_y = -1;
                        x_start[idx] = cur_x;
                        y_start[idx] = cur_y;
                    }
                    else //new thread start with a regular letter
                    {
                        cur_y += 1;
                        x_start[idx] = cur_x;
                        y_start[idx] = cur_y;
                    }
                    idx += 1;
                }
                else if (c == '\n') //not a new thread
                {
                    cur_y = -1;
                    cur_x += 1;
                }
                else //pass by a regular letter
                {
                    cur_y += 1;
                }
                counter += 1; //counter till we reach the next thread chunk
            }


            int start = 0; //give every thread the start index in text file
            int end = letter_per_thread - 1; //give every thread the end index in text file
            for (int i = 0; i < num_of_threads; i++)
            {
                int loc = i;
                int s = start;
                int e = end;
                if (i != (num_of_threads - 1)) //take every thread beside the last one
                {
                    n_th[i] = new Thread(() => print_legit(s, e, delay, word_to_search, x_start[loc], y_start[loc], text_len));
                    start += letter_per_thread;
                    end += letter_per_thread;
                }
                else //last thread (the length of the last thread is different from others depends)
                {
                    n_th[i] = new Thread(() => print_legit(s, text_len - 1, delay, word_to_search, x_start[loc], y_start[loc], text_len));
                }
            }
            for (int i = 0; i < num_of_threads; i++) //start the threads
            {
                n_th[i].Start();
            }
            Console.Write("");
            while (thread_counter != num_of_threads) { Console.Write(""); } //wait until all the threads finish
            if (is_printed == 0) { Console.WriteLine("not found"); } //check if not found should be printed

        }
        private static void print_legit(int start, int end, int delay, string word_to_search, int x_start, int y_start, int text_len) //search first letter and print it if legit
        {
            bool is_legit;
            for (int i = start; i <= end; i++) //search every letter in thread text ranfe
            {
                if (use_text[i] == '\n' && i == start) //if the first letter is '\n'
                {
                    y_start = 0;  //x_start already been added up there
                }
                else if (use_text[i] == '\n') //if 'n' but not the first letter
                {
                    x_start += 1;  //x_start isn't added
                    y_start = 0; //next element is in index 0
                }
                else if (word_to_search[0] == use_text[i]) //if we find a legit first letter
                {
                    is_legit = check_legit(i, delay, word_to_search, text_len); //check legit for the rest of the word
                    if (is_legit)
                    {
                        Console.WriteLine("[" + x_start + ", " + y_start + "]");
                        is_printed += 1;
                    }
                    y_start += 1;
                }
                else
                {
                    y_start += 1; //regular letter (not \n or the first letter in word_to_search)
                }
            }
            thread_counter += 1; //thread has finish this function
        }
        private static bool check_legit(int start, int delay, string word_to_search, int text_len) //check the rest of word_to_search starting from index start
        {
            int to_search = start + delay + 1; //index of letter 2
            for (int i = 1; i < word_to_search.Length; i++)
            {
                if (to_search > text_len - 1) //if we exceeded the text limit
                {
                    return false;
                }
                if (word_to_search[i] != use_text[to_search]) //if not the same in word_to_search
                {
                    return false;
                }
                to_search += delay + 1; //next letter index
            }
            return true;
        }

    }
}