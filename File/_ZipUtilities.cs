using System;
using System.Collections.Generic;
using System.IO;

namespace RPA.Utils.File
{
    public static class ZipUtilities
    {    
        public static List<FileInfo> SplitZipFile(string zipFilePath, string outputBasePath, long maxPartSizeBytes)
        {
            List<FileInfo> resultingParts = new();
            byte[] buffer = new byte[8192]; // Standard 8KB buffer size for .NET I/O operations
            int partIndex = 1;
            
            using (FileStream input = new(zipFilePath, FileMode.Open, FileAccess.Read))
            {
                while (input.Position < input.Length)
                {
                    string partFileName = $"{outputBasePath}.{partIndex:D3}"; // 001, 002, etc.
                    using (FileStream output = new(partFileName, FileMode.Create, FileAccess.Write))
                    {
                        long bytesRemaining = maxPartSizeBytes;
                        int bytesRead;
    
                        while (bytesRemaining > 0 &&
                               (bytesRead = input.Read(buffer, 0, (int)Math.Min(buffer.Length, bytesRemaining))) > 0)
                        {
                            output.Write(buffer, 0, bytesRead);
                            bytesRemaining -= bytesRead;
                        }
                    }
                    
                    resultingParts.Add(new FileInfo(partFileName));
                    partIndex++;
                }
            }
            
            return resultingParts;
        }
    }
}