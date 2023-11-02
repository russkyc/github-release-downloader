// MIT License
// 
// Copyright (c) 2023 Russell Camo (Russkyc)
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using GithubReleaseDownloader;
using GithubReleaseDownloader.Entities;

namespace ConsoleApp;

public static class Program
{
    public static void Main()
    {
        
        // The owner and repo to download from, and target path
        var owner = "libremindsph";
        var repo = "fossa-client-desktop";
        var downloadPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

        // Get last release using .GetLatest(), can substitute with other available methods
        var release = ReleaseManager.Instance.GetLatest(owner, repo);
        
        if (release is null) return;
        
        // In this case, we download all assets
        AssetDownloader.Instance.DownloadAllAssets(release,downloadPath, progressChanged: RunOnProgressChanged);
    }

    // This is being executed when the progress changes
    private static void RunOnProgressChanged(DownloadInfo downloadInfo)
    {
        if (downloadInfo.DownloadPercent == 1.0)
        {
            Console.WriteLine($"Finished downloading: {downloadInfo.Name}");
            return;
        }

        if (downloadInfo.DownloadPercent == 0)
        {
            Console.WriteLine($"Start downloading: {downloadInfo.Name}");
            return;
        }
        
        Console.WriteLine($"Progress({downloadInfo.Name}): {downloadInfo.DownloadPercent:P}");
    }
}