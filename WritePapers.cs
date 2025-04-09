using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
namespace Papers
{
    public class WritePapers
    {
        StreamWriter w;
        string file = "";
        public WritePapers(string file)
        {
            this.file = file;
        }

        public bool Write(string text)
        {
            bool request = false;
            try
            {
                w = new StreamWriter(this.file, true, Encoding.UTF8);
                w.WriteLine(text);
                w.Close();
                request = true;
            }
            catch (Exception ex)
            {
                request = false;
            }
            return request;
        }
    }
}
