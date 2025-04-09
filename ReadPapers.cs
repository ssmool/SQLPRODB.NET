using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Papers
{
    public class ReadPapers
    {
        private string file;
        StreamReader r;

        public ReadPapers()
        {
        }

        public string[] ReadConfigureFile(string file)
        {
            this.file = file;
            string[] request;
            r = new StreamReader(this.file);
            request = r.ReadToEnd().Split('&');
            r.Close();
            return request;
        }

        public bool FileExists(string file)
        {
            this.file = file;
            bool request = false;
            int count = 0;
            try
            {
                r = new StreamReader(this.file);
                count = r.ReadToEnd().Length;
                r.Close();
            }
            catch (Exception ex)
            {
                request = false;
            }

            if (count > 0)
                request = true;
            else
                request = false;
            return request;
        }
    }
}
