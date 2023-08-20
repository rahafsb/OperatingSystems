using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Security.Cryptography;
using System.Threading;

namespace TinyMemFS
{
    class TinyMemFS
    {
        private int total;
        private Int64 nexLine;
        private Dictionary<String, String> dict;
        private List<string> keys;
        private List<Mutex> locks;
        private string s_path;

        public TinyMemFS()
        {

            this.nexLine = 0;
            dict = new Dictionary<string, string>();// (nameOfFile,FirstRow-LastRow) in TinyMefs for the file
            locks = new List<Mutex>();
            keys = new List<string>();
            s_path = @"TinyMefs3.txt";
            total = 0;



        }
        public TinyMemFS(String str)
        {

            nexLine = 0;
            dict = new Dictionary<string, string>();
            s_path = @str;
            keys = new List<string>();
            total = 0;
            locks = new List<Mutex>();
        }
        public bool add(String fileName, String fileToAdd)
        {/* if it doesn't exist*/
            // fileName - The name of the file to be added to the file system
            // fileToAdd - The file path on the computer that we add to the system
            // return false if operation failed for any reason
            // Example:
            // add("name1.pdf", "C:\\Users\\user\Desktop\\report.pdf")
            // note that fileToAdd isn't the same as the fileName
            if (dict.ContainsKey(fileName))
            {
                return false;
            }
            if (!File.Exists(fileToAdd)) { return false; }
            FileInfo fi = new FileInfo(fileToAdd);// 
            String[] lines = File.ReadAllLines(fileToAdd);
            Int64 startIdx = this.nexLine;
            Int64 lastIdx = this.nexLine - 1;

            for (Int64 i = 0; i < lines.Length; i++)
            {
                File.AppendAllText(this.s_path, lines[i] + "\n");
                lastIdx++;
            }

            this.nexLine = lastIdx + 1; // "report.pdf,idx1,idx2,630KB,Friday, ‎May ‎13, ‎2022, ‏‎12:16:32 PM",
            this.dict.Add(fileName, "" + startIdx + "," + lastIdx + "," + (fi.Length / 1024) + "," + fi.CreationTime + "," + fileToAdd);// length/1024?
            this.keys.Add("-");
            this.nexLine = lastIdx + 1;
            total++;
            Mutex mtx = new Mutex();
            locks.Add(mtx);
            return true;
        }
        public int get_idx(string name)
        {
            int ret = -1;
            foreach (string item in dict.Keys)
            {
                ret++;
                if (item == name)
                {
                    return ret;
                }
            }
            return -1;
        }
        public bool remove(String fileName)
        {
            // fileName - remove fileName from the system
            // this operation releases all allocated memory for this file
            // return false if operation failed for any reason
            // Example:
            // remove("name1.pdf")
            if (!dict.ContainsKey(fileName)) { return false; }
            //// delte the data from the file 
            /*  int lck = get_idx(fileName);
              if (lck != -1)
              {
                  locks.RemoveAt(lck);

              }*/
            string all = dict[fileName];
            string[] word = all.Split(",");
            Int64 first = Int64.Parse(word[0]);
            Int64 last = Int64.Parse(word[1]);
            string[] lines = File.ReadAllLines(s_path);
            Int64 new_size = lines.Length - (last - first) - 1;
            string[] new_lines = new string[new_size];
            this.nexLine = this.nexLine - (last - first) - 1;
            for (Int64 i = 0; i < new_size; i++)
            {
                if (i < first)
                {
                    new_lines[i] = lines[i];
                }
                else
                {
                    new_lines[i] = lines[i + (last - first) + 1];
                }
            }
            File.WriteAllLines(this.s_path, new_lines);
            bool found = false;
            List<string> strs = new List<string>();
            Int64 counter = -1;
            Int64 at = 0;
            foreach (string item in dict.Keys)
            {
                counter++;
                if (found)
                {
                    strs.Add(item);
                }
                if (item.Equals(fileName))
                {
                    found = true;
                    at = counter;
                }
            }
            dict.Remove(fileName);
            for (int j = 0; j < strs.Count; j++)
            {

                string word_ = dict[strs[j]];
                string[] ch = word_.Split(",");
                dict[strs[j]] = "" + (int.Parse(ch[0]) - (last - first) - 1) + "," + (int.Parse(ch[1]) - (last - first) - 1) + "," + ch[2] + "," + ch[3] + "," + ch[4];

            }
            if (keys.Count > at)
            {
                keys.RemoveAt((int)at);
                locks.RemoveAt((int)at);
            }
            total--;
            return true;
        }
        public List<String> listFiles()
        {
            // The function returns a list of strings with the file information in the system
            // Each string holds details of one file as following: "fileName,size,creation time"
            // Example:{
            // "report.pdf,630KB,Friday, ‎May ‎13, ‎2022, ‏‎12:16:32 PM",
            // "table1.csv,220KB,Monday, ‎February ‎14, ‎2022, ‏‎8:38:24 PM" }
            // You can use any format for the creation time and date
            List<String> files = new List<string>();
            for (int lck = 0; lck < locks.Count; lck++)
            {
                locks[lck].WaitOne();
            }
            foreach (string item in dict.Keys)
            {
                string word = dict[item];
                string[] words = word.Split(",");
                string ad = "" + item + "," + words[2] + " KB," + words[3];
                files.Add(ad);
            }
            for (int lck = 0; lck < locks.Count; lck++)
            {
                locks[lck].ReleaseMutex();
            }
            return files;
        }
        public bool save(String fileName, String fileToAdd)
        {
            // this function saves file from the TinyMemFS file system into a file in the physical disk
            // fileName - file name from TinyMemFS to save in the computer
            // fileToAdd - The file path to be saved on the computer
            // return false if operation failed for any reason
            // Example:
            // save("name1.pdf", "C:\\tmp\\fileName.pdf")
            int lck = get_idx(fileName);
            if (lck != -1)
            {
                locks[lck].WaitOne();
            }
            if (!dict.ContainsKey(fileName)) { return false; }
            if (!File.Exists(fileToAdd)) { return false; }
            string line = dict[fileName];
            string[] idx = line.Split(",");
            Int64 first = Int64.Parse(idx[0]);
            Int64 last = Int64.Parse(idx[1]);
            string[] text = File.ReadAllLines(s_path);
            for (Int64 i = first; i <= last; i++)
            {
                File.AppendAllText(fileToAdd, text[i] + "\n");

            }
            locks[lck].ReleaseMutex();
            return true;
        }
        public bool encrypt(String key)
        {
            // key - Encryption key to encrypt the contents of all files in the system 
            // You can use an encryption algorithm of your choice
            // return false if operation failed for any reason
            // Example:
            // encrypt("myFSpassword")

            if (key.Length != 16) { return false; }
            if (keys.Contains(key)) { return false; }
            int locked = -1;
            for (int i = 0; i < total; i++)
            {
                if (keys[i] == "-")
                {
                    locks[i].WaitOne();
                }
                else
                {
                    locked++;
                }

            }
            if (locked == total) { return false; }

            int counter = -1;
            foreach (string item in dict.Keys)
            {
                counter++;
                if (keys[counter] == "-") // change everything from now on 
                {
                    keys[counter] = key;
                    var password = Encoding.UTF8.GetBytes(key);
                    AESEncryption change = new AESEncryption(password);//password
                    string data = dict[item];
                    string ret = string.Empty;
                    string[] idx = data.Split(",");
                    string to_cypher = File.ReadAllText(idx[4]);
                    if (string.IsNullOrEmpty(to_cypher)) { return false; }
                    else
                    {
                        ret = change.my_enrypt(to_cypher);
                        File.WriteAllText(idx[4], ret);
                    }
                }
            }
            for (int i = 0; i < total; i++)
            {
                if (keys[i] == key)
                {
                    locks[i].ReleaseMutex();
                }

            }
            return true;
        }

        public bool decrypt(String key)
        {
            // fileName - Decryption key to decrypt the contents of all files in the system 
            // return false if operation failed for any reason
            // Example:
            // decrypt("myFSpassword")
            if (key.Length != 16) { return false; }
            if (!keys.Contains(key)) { return false; }
            List<int> index_in = new List<int>();
            for (int i = 0; i < keys.Count; i++)
            {
                if (keys[i] == key)
                {
                    index_in.Add(i);
                }
            }
            for (int j = 0; j < index_in.Count; j++)
            {
                locks[index_in[j]].WaitOne();
            }
            int counter2 = -1;
            foreach (string item in dict.Keys)
            {
                counter2++;
                if (index_in.Contains(counter2))
                {
                    var password = Encoding.UTF8.GetBytes(key);
                    AESEncryption change = new AESEncryption(password);//password
                    string data = dict[item];
                    string[] idx = data.Split(",");
                    string to_cypher = File.ReadAllText(idx[4]);
                    if (string.IsNullOrEmpty(to_cypher)) { return false; }
                    else
                    {
                        string ret = change.my_decrypt(to_cypher);
                        File.WriteAllText(idx[4], ret);
                    }

                }
            }
            for (int j = 0; j < index_in.Count; j++)
            {
                keys[index_in[j]] = "-";
                locks[index_in[j]].ReleaseMutex();
            }


            return true;
        }

    }
    class AESEncryption
    {
        private AesCryptoServiceProvider provide_r;
        public AESEncryption(byte[] password)//
        {
            provide_r = new AesCryptoServiceProvider();
            provide_r.BlockSize = 128;
            provide_r.KeySize = 128;
            // provide_r.GenerateIV();
            provide_r.IV = password;//16
            provide_r.Key = password;
            provide_r.Mode = CipherMode.CBC;
            provide_r.Padding = PaddingMode.PKCS7;
        }

        public string my_enrypt(string txt)
        {
            ICryptoTransform tra = provide_r.CreateEncryptor();
            byte[] cyphred = tra.TransformFinalBlock(ASCIIEncoding.ASCII.GetBytes(txt), 0, txt.Length);
            string ret = Convert.ToBase64String(cyphred);
            return ret;
        }
        public string my_decrypt(string syphred)
        {
            ICryptoTransform tra = provide_r.CreateDecryptor();
            byte[] enc = Convert.FromBase64String(syphred);
            byte[] dec = tra.TransformFinalBlock(enc, 0, enc.Length);
            string ret = ASCIIEncoding.ASCII.GetString(dec);
            return ret;
        }

    }
}
