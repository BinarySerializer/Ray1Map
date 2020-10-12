using System;
using System.Diagnostics;
using System.IO;
using Debug = UnityEngine.Debug;

namespace R1Engine
{
    public static class ProcessHelpers
    {
        public static string GetStringAsPathArg(string filePath) => $"\"{filePath.Replace('/', '\\')}\"";

        public static void RunProcess(string filePath, string[] args, string workingDir = null, bool waitForExit = true, bool logInfo = true)
        {
            // Create the process and dispose when finished
            using (var p = new Process())
            {
                // Set the start info
                p.StartInfo = new ProcessStartInfo(filePath, String.Join(" ", args))
                {
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    WorkingDirectory = workingDir ?? Path.GetDirectoryName(filePath)
                };

                if (logInfo)
                    Debug.Log($"Starting process {p.StartInfo.FileName} with arguments: {p.StartInfo.Arguments}");

                p.Start();

                if (waitForExit)
                {
                    p.WaitForExit();

                    if (logInfo)
                        Debug.Log($"Process output: {p.StandardOutput.ReadToEnd()}");
                }
            }
        }
    }
}