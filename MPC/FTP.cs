using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;
using System.Net;

namespace MPC
{
    public class FTP
    {
        private const string FTP_URL = @"ftp://www.mossneukparishchurch.org.uk";

        private string Username;
        private string Password;


        public FTP(string Uname)
        {
            Username = Uname;
        }


        public bool Publish(string Source, int PageCount)
        {
            //Check Directory for files!
            if(!Directory.Exists(Source)) { Console.WriteLine(">> No Publish directory not found"); return false; }
 
            string[] Pages = Directory.GetFiles(Source);

            if (Pages.Length != PageCount) 
            { 
                Console.WriteLine(">> Expected {0} page(s) -> Found {1}", PageCount, Directory.GetFiles(Source).Length); 
                return false;
            }

            //We have files - so lets continue
            //Password for FTP
            Password = GetPassword();
            DateTime Start = DateTime.Now;
            int Passes = 0;

            //Check if we can connect to the FTP server
            if (HandleInitalRequest())
            {
                for (int i = 0; i < Pages.Length; ++i)
                {

                    string PageTitle = Pages[i].Replace(Source, "");

                    Console.Write("> (working) Uploading {0} ... ", PageTitle);
                    if (!HandleUpload(Pages[i], PageTitle))
                    {
                        Console.Write("\r> (FAILED) Uploading {0}        \n", PageTitle);
                    }
                    else
                    {
                        Console.Write("\r> (DONE) Uploaded {0}             \n", PageTitle);
                        Passes++;
                    }
                }

                DateTime Stop = DateTime.Now;
                TimeSpan TimeTaken = Stop - Start;
                int Fails = PageCount - Passes;

                Console.WriteLine("\n> Upload Complete! \n\nTotal Upload Tasks: {0}\nCompleted: {1}\nFailed: {2}\n\nTime Taken: {3}s\n", 
                                                                                 PageCount, Passes, Fails, TimeTaken.TotalSeconds);
            }



            //For each file - upload this to the FTP server

            //Make sure to close after each upload

            //Remember to clear password from memory as well!

            return true;
        }

        /// <summary>
        /// Gets password from User and masks the input on the console window
        /// </summary>
        /// <returns>Password entered by the user</returns>
        private string GetPassword()
        {
            Console.Write("FTP Password:");

            string UserPassword = "";
            string letter = Console.ReadKey(true).KeyChar.ToString();
            while (letter != "\r")
            {
                if (letter == "\b")
                {
                    if (UserPassword.Length != 0)
                    {
                        UserPassword = UserPassword.Substring(0, UserPassword.Length - 1);
                    }
                }
                else
                {
                    UserPassword += letter;
                }

                letter = Console.ReadKey(true).KeyChar.ToString();
            }

            Console.Write("\n");
            return UserPassword;
        }


        private bool HandleInitalRequest()
        {
            Console.WriteLine("> Attempting FTP connection...");

            try
            {
                //Create a base Request with URL and given credentials
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(FTP_URL);
                request.Credentials = new NetworkCredential(Username.Normalize(), Password.Normalize());
                request.Method = WebRequestMethods.Ftp.PrintWorkingDirectory;

                //Get the FTP response and close for resource leak
                FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                response.Close();

                response = null;

                //Read in response
                Console.WriteLine("> Connected to FTP server!");
                return true;
            }
            catch (Exception e) {

                //Catch invalid login details
                if (e.Message.Contains("(530)"))
                {
                    Console.WriteLine(">> FTP password incorrect!");
                }
                else
                {
                    Console.WriteLine(">> * FTP Error * \nMessage: {0} \n\nTrace{1}", e.Message, e.StackTrace);
                }

                return false;
            } 
        }

        private bool HandleUpload(string Filename, string Title)
        {
            try
            {
                //Create a base Request with URL and given credentials
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(FTP_URL + "/public_html/" + Title);
                request.Credentials = new NetworkCredential(Username.Normalize(), Password.Normalize());
                request.Method = WebRequestMethods.Ftp.UploadFile;

                // Copy the contents of the file to the request stream.
                StreamReader sourceStream = new StreamReader(Filename);
                byte[] fileContents = Encoding.UTF8.GetBytes(sourceStream.ReadToEnd());
                sourceStream.Close();
                request.ContentLength = fileContents.Length;

                Stream requestStream = request.GetRequestStream();
                requestStream.Write(fileContents, 0, fileContents.Length);
                requestStream.Close();

                FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                response.Close();

                response = null;
                return true;
            }
            #if DEBUG
            catch (Exception e) { Console.WriteLine("\n\n>> * FTP Error * \nMessage: {0} \n\nTrace{1}", e.Message, e.StackTrace); return false; } 
            #else
            catch (Exception e) { return false; }
            #endif
        }
    }
}
