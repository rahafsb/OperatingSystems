using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;

namespace SpreadsheetApp
{
    class SharableSpreadSheet
    {
        private int row;
        private int col;
        private int nUsers;

        private List<List<String>> mat;
        private List<ReaderWriterLock> rowLocker;

        private static SemaphoreSlim sema;
        private static Mutex mutex1;
        private static Mutex mutex2;

        public SharableSpreadSheet(int nRows, int nCols, int nUsers = -1)
        {
            // nUsers used for setConcurrentSearchLimit, -1 mean no limit.
            // construct a nRows*nCols spreadsheet
            if (nRows <= 0 || nCols <= 0 || nUsers == 0 || nUsers <= -2)
            {
                throw new Exception("Invalid input");
            }
            this.row = nRows;
            this.col = nCols;

            mutex1 = new Mutex();
            mutex2 = new Mutex();
            if (nUsers == -1)
            {
                setConcurrentSearchLimit(20);
            }
            else
            {
                this.nUsers = nUsers;
            }
            sema = new SemaphoreSlim(this.nUsers);

            rowLocker = new List<ReaderWriterLock>();
            for (int i = 0; i < this.row; i++)
            {
                rowLocker.Add(new ReaderWriterLock());
            }

            mat = new List<List<string>>();
            for (int i = 0; i < row; i++)
            {
                List<String> lst = new List<string>();
                for (int j = 0; j < col; j++)
                {
                    lst.Add("");
                }
                mat.Add(lst);
            }
        }
        public String getCell(int row, int col) //read lock 3la kol lsater
        {
            if (row < 0 || col < 0 || row > this.row || col > this.col)
            {
                throw new Exception("Invalid input");
            }
            sema.Wait();
            // return the string at [row,col]
            rowLocker[row].AcquireReaderLock(1000);
            String s = mat[row][col];
            rowLocker[row].ReleaseReaderLock();
            sema.Release();

            // Console.WriteLine(s); ///////////////////////////////// am7a am7a darore
            return s;
        }
        public void setCell(int row, int col, String str) // write lock 3la kol lsater
        {
            if (row < 0 || col < 0 || row > this.row || col > this.col)
            {
                throw new Exception("Invalid input");
            }
            // set the string at [row,col]
            rowLocker[row].AcquireWriterLock(1000);
            mat[row][col] = str;
            rowLocker[row].ReleaseWriterLock();
        }
        public Tuple<int, int> searchString(String str) //read lock 3la kol l2astor
        {
            sema.Wait();
            int row = -1, col = -1;
            // return first cell indexes that contains the string (search from first row to the last row)
            for (int i = 0; i < this.row; i++)
            {
                rowLocker[i].AcquireReaderLock(1000);
            }
            int hold = 0;
            for (int i = 0; i < this.row; i++)
            {
                for (int j = 0; j < this.col; j++)
                {
                    if (mat[i][j] == str && hold == 0)
                    {
                        row = i;
                        col = j;
                        hold++;
                    }
                }
                rowLocker[i].ReleaseReaderLock();
            }
            sema.Release();
            if (row == -1 || col == -1)
            {
                throw new Exception("not found");
            }

            Tuple<int, int> t = new Tuple<int, int>(row, col);
            //    Console.WriteLine(t.ToString());  ////////////////////////////////////////////////////////// am7a
            return t;
        }
        public void exchangeRows(int row1, int row2) //write lock 3la lsatren
        {
            if (row1 < 0 || row2 < 0 || row1 > this.row || row2 > this.row)
            {
                throw new Exception("Invalid input");
            }
            // exchange the content of row1 and row2
            bool taken = false;
            rowLocker[row1].AcquireWriterLock(1000);
            try
            {
                Monitor.TryEnter(rowLocker[row2], 100, ref taken);
                if (taken)
                {

                    String ret = "";
                    for (int i = 0; i < col; i++)
                    {
                        ret = mat[row1][i];
                        mat[row1][i] = mat[row2][i];
                        mat[row2][i] = ret;
                    }
                }


            }

            finally
            {
                rowLocker[row1].ReleaseWriterLock();
                if (taken)
                {
                    Monitor.Exit(rowLocker[row2]);

                }
            }
        }
        public void exchangeCols(int col1, int col2) //wwrite lock 3la kol l2astor, mnser kol mn5ales sater n3ml unlock tawale
        {
            if (col1 < 0 || col2 < 0 || col1 > this.col || col2 > this.col)
            {
                throw new Exception("Invalid input");
            }
            // exchange the content of col1 and col2
            for (int i = 0; i < row; i++)
            {
                rowLocker[i].AcquireWriterLock(1000);
            }

            String ret = "";
            for (int i = 0; i < row; i++)
            {
                ret = mat[i][col1];
                mat[i][col1] = mat[i][col2];
                mat[i][col2] = ret;
                rowLocker[i].ReleaseWriterLock();
            }
        }
        public int searchInRow(int row, String str) //read lock 3la lsater
        {
            // perform search in specific row
            if (row < 0 || row > this.row)
            {
                throw new Exception("Invalid input");
            }
            sema.Wait();
            int col = -1;

            rowLocker[row].AcquireReaderLock(1000);
            for (int i = 0; i < this.col; i++)
            {
                if (mat[row][i] == str)
                {
                    col = i;
                    break;
                }
            }
            rowLocker[row].ReleaseReaderLock();
            sema.Release();
            if (col == -1)
            {
                throw new Exception("not found");
            }
            //  Console.WriteLine(col); //////////////////////////
            return col;
        }
        public int searchInCol(int col, String str)  //read lock 3la kol l2astor
        {
            // perform search in specific col
            if (col < 0 || col > this.col)
            {
                throw new Exception("Invalid input");
            }
            sema.Wait();
            for (int i = 0; i < this.row; i++)
            {
                rowLocker[i].AcquireReaderLock(1000);
            }
            int row = -1;

            int hold = 0;
            for (int i = 0; i < this.row; i++)
            {
                if (mat[i][col] == str && hold == 0)
                {
                    row = i;
                    hold = 1;
                }
                rowLocker[i].ReleaseReaderLock();
            }
            sema.Release();
            if (row == -1)
            {
                throw new Exception("not found");
            }
            // Console.WriteLine(row); //////////////////////////
            return row;
        }
        public Tuple<int, int> searchInRange(int col1, int col2, int row1, int row2, String str) //read lock 3la l2astor mn row 1 to row 2
        {
            if (row1 < 0 || row2 < 0 || col1 < 0 || col2 < 0 || row1 >= this.row ||
                row2 >= this.row || col1 >= this.col || col2 >= this.col)
            {
                throw new Exception("Invalid input");
            }
            sema.Wait();
            for (int i = row1; i <= row2; i++)
            {
                rowLocker[i].AcquireReaderLock(1000);
            }

            int row, col;
            row = -1;
            col = -1;

            // perform search within spesific range: [row1:row2,col1:col2]
            //includes col1,col2,row1,row2
            int hold = 0;
            for (int i = row1; i <= row2; i++)
            {
                for (int j = col1; j <= col2; j++)
                {
                    if (mat[i][j] == str && hold == 0)
                    {
                        row = i;
                        col = j;
                        hold++;
                    }
                }
                rowLocker[i].ReleaseReaderLock();
            }

            sema.Release();
            if (row == -1 || col == -1)
            {
                throw new Exception("not found");
            }
            Tuple<int, int> t = new Tuple<int, int>(row, col);
            //   Console.WriteLine(t.ToString()); ////////////////////////////////////
            return t;
        }
        public void addRow(int row1) //2lo mutex 3shan l getsize
        {
            if (row1 < 0 || row1 > this.row)
            {
                throw new Exception("Invalid input");
            }
            mutex1.WaitOne();
            //add a row after row1
            for (int i = row1 + 1; i < row; i++)
            {
                rowLocker[i].AcquireWriterLock(1000);
            }
            rowLocker.Add(new ReaderWriterLock());
            rowLocker[this.row].AcquireWriterLock(1000);
            List<String> new_l = new List<String>();
            for (int i = 0; i < this.col; i++)
            {
                new_l.Add("");
            }
            mat.Insert(row1 + 1, new_l);
            this.row++;
            for (int i = row1 + 1; i < this.row; i++)
            {
                rowLocker[i].ReleaseWriterLock();
            }
            mutex1.ReleaseMutex();
        }
        public void addCol(int col1) //2lo mutex 3shan lgetsize
        {
            //add a column after col1
            if (col1 < 0 || col1 > this.col)
            {
                throw new Exception("Invalid input");
            }
            mutex2.WaitOne();
            //add a row after row1
            for (int i = 0; i < row; i++)
            {
                rowLocker[i].AcquireWriterLock(1000);
            }
            this.col++;
            for (int i = 0; i < this.row; i++)
            {
                mat[i].Insert(col1 + 1, "");
            }
            for (int i = 0; i < this.row; i++)
            {
                rowLocker[i].ReleaseWriterLock();
            }
            mutex2.ReleaseMutex();
        }
        public Tuple<int, int>[] findAll(String str, bool caseSensitive) // mmno3 ykon fe ketabe
        {
            sema.Wait();
            // perform search and return all relevant cells according to caseSensitive param
            for (int i = 0; i < row; i++)
            {
                rowLocker[i].AcquireReaderLock(1000);
            }
            List<int> ret = new List<int>();
            int counter = 0;
            if (caseSensitive)
            {
                for (int i = 0; i < this.row; i++)
                {
                    for (int j = 0; j < this.col; j++)
                    {
                        if (mat[i][j] == str)
                        {
                            counter++;
                            ret.Add(i);
                            ret.Add(j);
                        }
                    }
                    rowLocker[i].ReleaseReaderLock();
                }
            }
            else
            {
                for (int i = 0; i < this.row; i++)
                {
                    for (int j = 0; j < this.col; j++)
                    {
                        if (mat[i][j].ToLower() == str.ToLower())
                        {
                            counter++;
                            ret.Add(i);
                            ret.Add(j);
                        }
                    }
                    rowLocker[i].ReleaseReaderLock();
                }
            }
            sema.Release();
            if (counter == 0)
            {
                throw new Exception("not found");
            }
            Tuple<int, int>[] t_list = new Tuple<int, int>[counter];
            int hold = 0;
            for (int i = 0; i < counter; i++)
            {
                Tuple<int, int> t = new Tuple<int, int>(ret[hold], ret[hold + 1]);
                t_list[i] = t;
                hold += 2;
            }

            /*  for (int i = 0; i < t_list.Length; i++) ////////////////////////////////////////////
              {
                  Console.WriteLine(t_list[i].ToString());
              }
            */

            return t_list;
        }
        public void setAll(String oldStr, String newStr, bool caseSensitive) //mmno3 ykon 3na read shele
        {
            // replace all oldStr cells with the newStr str according to caseSensitive param
            for (int i = 0; i < row; i++)
            {
                rowLocker[i].AcquireWriterLock(1000);
            }
            if (caseSensitive)
            {
                for (int i = 0; i < this.row; i++)
                {
                    for (int j = 0; j < this.col; j++)
                    {
                        if (mat[i][j] == oldStr)
                        {
                            mat[i][j] = newStr;
                        }
                    }
                    rowLocker[i].ReleaseWriterLock();
                }
            }
            else
            {
                for (int i = 0; i < this.row; i++)
                {
                    for (int j = 0; j < this.col; j++)
                    {
                        if (mat[i][j].ToLower() == oldStr.ToLower())
                        {
                            mat[i][j] = newStr;
                        }
                    }
                    rowLocker[i].ReleaseWriterLock();
                }
            }
        }
        public Tuple<int, int> getSize() //lmutexem ltnen t3on addrow o addcol mn3mlhn Writeone
        {
            mutex1.WaitOne();
            mutex2.WaitOne();
            int nRows = this.row;
            int nCols = this.col;

            Tuple<int, int> t = new Tuple<int, int>(nRows, nCols);
            // return the size of the spreadsheet in nRows, nCols
            mutex1.ReleaseMutex();
            mutex2.ReleaseMutex();
            //  Console.WriteLine(t.ToString()); ////////////////////////////////////////
            return t;
        }
        public void setConcurrentSearchLimit(int nUsers)
        {
            // this function aims to limit the number of users that can perform the search operations concurrently.
            // The default is no limit. When the function is called, the max number of concurrent search operations is set to nUsers.
            // In this case additional search operations will wait for existing search to finish.
            // This function is used just in the creation
            this.nUsers = nUsers;
        }

        public void save(String fileName) //read lock 3la kool l2astor
        {
            // save the spreadsheet to a file fileName.
            // you can decide the format you save the data. There are several options.
            for (int i = 0; i < this.row; i++)
            {
                rowLocker[i].AcquireReaderLock(1000);
            }
            using (StreamWriter OutputFile = new StreamWriter(fileName))
            {
                OutputFile.WriteLine(this.row);
                OutputFile.WriteLine(this.col);
                for (int i = 0; i < this.row; i++)
                {
                    for (int j = 0; j < this.col; j++)
                    {
                        OutputFile.WriteLine(mat[i][j]);
                    }
                }
            }
            for (int i = 0; i < this.row; i++)
            {
                rowLocker[i].ReleaseReaderLock();
            }

        }
        public void load(String fileName)  //write lock 3la kol l2astor
        {
            // load the spreadsheet from fileName
            // replace the data and size of the current spreadsheet with the loaded data
            mutex1.WaitOne();
            mutex2.WaitOne();
            for (int i = 0; i < this.row; i++)
            {
                rowLocker[i].AcquireWriterLock(1000);
            }
            String[] lines = File.ReadAllLines(fileName);
            int remove = this.row - int.Parse(lines[0]);
            int len = this.row;
            this.row = int.Parse(lines[0]);
            this.col = int.Parse(lines[1]);
            if (remove > 0)
            {
                for (int i = 0; i < remove; i++)
                {
                    rowLocker.Remove(rowLocker[len - i - 1]);
                }
            }
            else if (remove < 0)
            {
                for (int i = 0; i < remove * -1; i++)
                {
                    rowLocker.Add(new ReaderWriterLock());
                    rowLocker[len + i].AcquireWriterLock(1000);
                }
            }
            this.mat = new List<List<string>>();
            for (int i = 0; i < this.row; i++)
            {
                this.rowLocker.Add(new ReaderWriterLock());
            }
            int counter = 2;
            for (int i = 0; i < row; i++)
            {
                List<String> lst = new List<string>();
                for (int j = 0; j < col; j++)
                {
                    lst.Add(lines[counter]);
                    counter += 1;
                }
                mat.Add(lst);
            }

            for (int i = 0; i < this.row; i++)
            {
                rowLocker[i].ReleaseWriterLock();
            }

            mutex1.ReleaseMutex();
            mutex2.ReleaseMutex();
        }
    }
}
