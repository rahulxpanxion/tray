using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace LzSetup
{
    class LZSetup
    {
        const string HomeDirectory = @"D:\LZSetup2\engineering\";
        static void Main(string[] args)
        {
            Directory.CreateDirectory(HomeDirectory);
            Directory.CreateDirectory(HomeDirectory + @"www\resources");

            var externalHtmlDirectory = HomeDirectory + @"site-external-html-src";
            var externalResourcesDirectory = HomeDirectory +  @"site-external-resources-src";

            var installGulpTask = ExecuteCommandAsync("npm install gulp -g");

            var cloneTasks = new List<Task>
            {
              ExecuteCommandAsync("git clone https://github.legalzoom.com/engineering/site-external-html-src.git"),
              ExecuteCommandAsync("git clone https://github.legalzoom.com/engineering/site-external-resources-src.git"),
              ExecuteCommandAsync("git clone https://github.legalzoom.com/engineering/site-external-LZWeb.git"),
              ExecuteCommandAsync("git clone https://github.legalzoom.com/engineering/site-external-LZBrain.git"),
              ExecuteCommandAsync("git clone https://github.legalzoom.com/engineering/site-external-ThirdPartyCheckout.git"),
              ExecuteCommandAsync("git clone https://github.legalzoom.com/engineering/site-external-CorporateCenter.git")
            };

            var gulpHtmlTasks = new List<Task>
            {
                 ExecuteCommandAsync("gulp build:site", externalHtmlDirectory),
                 ExecuteCommandAsync("gulp build:misc", externalHtmlDirectory),
                 ExecuteCommandAsync("gulp build:configs:local", externalHtmlDirectory),
                 ExecuteCommandAsync("gulp build:aabf", externalHtmlDirectory),
                 ExecuteCommandAsync("gulp build:aptm", externalHtmlDirectory),
            };

            var gulpResourcesTasks = new List<Task>
            {
                 ExecuteCommandAsync("gulp build:resources", externalResourcesDirectory),
                 ExecuteCommandAsync("gulp build: assets", externalResourcesDirectory),
                 ExecuteCommandAsync("gulp build: js", externalResourcesDirectory),
                 ExecuteCommandAsync("gulp build: css: LZ25", externalResourcesDirectory),
                 ExecuteCommandAsync("gulp build: js: LZ25", externalResourcesDirectory),
                 ExecuteCommandAsync("gulp build: portable: html", externalResourcesDirectory),
                 ExecuteCommandAsync("gulp build: aabf: scss", externalResourcesDirectory),
                 ExecuteCommandAsync("gulp push: sem", externalResourcesDirectory),
            };

            Task.Run(async () => {
                await installGulpTask;            
            }).Wait();

            Task.Run(async () => {
                await Task.WhenAll(cloneTasks.ToArray());
            }).Wait();

            Task.Run(async () => {
                await Task.WhenAll(gulpHtmlTasks.ToArray());
                await Task.WhenAll(gulpResourcesTasks);
            }).Wait();
        }

        public  static async Task ExecuteCommandAsync(object command,string workingDirectory = null)
        {
            try
            {
                // create the ProcessStartInfo using "cmd" as the program to be run,
                // and "/c " as the parameters.
                // Incidentally, /c tells cmd that we want it to execute the command that follows,
                // and then exit.
                var procStartInfo =
                    new System.Diagnostics.ProcessStartInfo("cmd", "/c " + command)
                    {

                        // The following commands are needed to redirect the standard output.
                        // This means that it will be redirected to the Process.StandardOutput StreamReader.
                        RedirectStandardOutput = true,
                        UseShellExecute = false,
                        // Do not create the black window.
                        CreateNoWindow = false,
                        WorkingDirectory = workingDirectory ?? HomeDirectory
                    };

                // Now we create a process, assign its ProcessStartInfo and start it
                using (var proc = new System.Diagnostics.Process { StartInfo = procStartInfo })
                {
                    proc.Start();
                    // Get the output into a string
                    var result = await proc.StandardOutput.ReadToEndAsync();
                    // Display the command output.

                    proc.WaitForExit();
                    Console.WriteLine(result);
                }             
            }
            catch (Exception objException)
            {
                Console.WriteLine(objException.Message);
            }
        }
    }
}