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

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using BigHelp.Http;
using GithubReleaseDownloader.Entities;

#pragma warning disable CS8618

namespace GithubReleaseDownloader
{
    public class AssetDownloader
    {
        private static readonly object Lock = new object();
        private static AssetDownloader? _instance = new AssetDownloader();
        private static HttpClient _httpClient;

        private AssetDownloader()
        {
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.TryAddWithoutValidation(
                "User-Agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");
        }

        /// <summary>
        /// Gets the singleton instance of the AssetDownloader class.
        /// </summary>
        public static AssetDownloader Instance
        {
            get
            {
                if (_instance is null)
                {
                    lock (Lock)
                    {
                        return _instance ??= new AssetDownloader();
                    }
                }
                return _instance;
            }
        }

        /// <summary>
        /// Downloads all assets from a GitHub release.
        /// </summary>
        /// <param name="release">The release containing the assets to download.</param>
        /// <param name="savePath">The path where the assets will be saved.</param>
        /// <param name="progressChanged">An optional callback to track the download progress.</param>
        /// <returns>The download results as a collection of DownloadInfo objects.</returns>
        public IEnumerable<DownloadInfo> DownloadAllAssets(Release release, string savePath, Action<DownloadInfo>? progressChanged = null!)
        {
            return Task.Run(async () => await DownloadAllAssetsAsync(release, savePath, progressChanged)).Result;
        }

        /// <summary>
        /// Asynchronously downloads all assets from a GitHub release.
        /// </summary>
        /// <param name="release">The release containing the assets to download.</param>
        /// <param name="savePath">The path where the assets will be saved.</param>
        /// <param name="progressChanged">An optional callback to track the download progress.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the download results as a collection of DownloadInfo objects.</returns>
        public async Task<IEnumerable<DownloadInfo>> DownloadAllAssetsAsync(Release release, string savePath, Action<DownloadInfo>? progressChanged = null!)
        {
            List<DownloadInfo> downloadResults = new List<DownloadInfo>();

            foreach (var asset in release.Assets)
            {
                var downloadProgress = new DownloadInfo
                {
                    Name = asset.Name,
                    Path = savePath + asset.Name
                };

                await _httpClient.DownloadFileAsync(
                    asset.DownloadUrl,
                    savePath,
                    reportCallback: new Progress<HttpDownloadProgress>(
                        progress => 
                        { 
                            downloadProgress.DownloadPercent = progress.PercentDownloaded; 
                            downloadProgress.DownloadedSize = progress.TotalDownloaded; 
                            if (progressChanged != null) progressChanged(downloadProgress); 
                        }));

                downloadResults.Add(downloadProgress);
            }

            return downloadResults;
        }

        /// <summary>
        /// Downloads specific assets from a GitHub release.
        /// </summary>
        /// <param name="assets">The assets to download.</param>
        /// <param name="savePath">The path where the assets will be saved.</param>
        /// <param name="progressChanged">An optional callback to track the download progress.</param>
        /// <returns>The download results as a collection of DownloadInfo objects.</returns>
        public IEnumerable<DownloadInfo> DownloadAssets(IEnumerable<ReleaseAsset> assets,
            string savePath, Action<DownloadInfo>? progressChanged = null!)
        {
            return Task.Run(async () => await DownloadAssetsAsync(assets, savePath, progressChanged)).Result;
        }

        /// <summary>
        /// Asynchronously downloads specific assets from a GitHub release.
        /// </summary>
        /// <param name="assets">The assets to download.</param>
        /// <param name="savePath">The path where the assets will be saved.</param>
        /// <param name="progressChanged">An optional callback to track the download progress.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the download results as a collection of DownloadInfo objects.</returns>
        public async Task<IEnumerable<DownloadInfo>> DownloadAssetsAsync(IEnumerable<ReleaseAsset> assets, string savePath, Action<DownloadInfo>? progressChanged = null!)
        {
            List<DownloadInfo> downloadResults = new List<DownloadInfo>();

            foreach (var asset in assets)
            {
                var downloadProgress = new DownloadInfo
                {
                    Name = asset.Name,
                    Path = savePath + asset.Name
                };

                await _httpClient.DownloadFileAsync(
                    asset.DownloadUrl,
                    savePath,
                    reportCallback: new Progress<HttpDownloadProgress>(
                        progress => 
                        { 
                            downloadProgress.DownloadPercent = progress.PercentDownloaded; 
                            downloadProgress.DownloadedSize = progress.TotalDownloaded;
                        if (progressChanged != null) progressChanged(downloadProgress);
                    }));

                downloadResults.Add(downloadProgress);
            }

            return downloadResults;
        }

        /// <summary>
        /// Downloads a single asset from a GitHub release.
        /// </summary>
        /// <param name="asset">The asset to download.</param>
        /// <param name="savePath">The path where the asset will be saved.</param>
        /// <param name="fileName">The name of the file.</param>
        /// <param name="progressChanged">An optional callback to track the download progress.</param>
        /// <returns>The download result as a DownloadInfo object.</returns>
        public DownloadInfo DownloadAsset(ReleaseAsset asset, string savePath, string fileName = null!, Action<DownloadInfo>? progressChanged = null!)
        {
            return Task.Run(async () => await DownloadAssetAsync(asset, savePath, fileName, progressChanged)).Result;
        }

        /// <summary>
        /// Asynchronously downloads a single asset from a GitHub release.
        /// </summary>
        /// <param name="asset">The asset to download.</param>
        /// <param name="savePath">The path where the asset will be saved.</param>
        /// <param name="fileName">The name of the file.</param>
        /// <param name="progressChanged">An optional callback to track the download progress.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the download result as a DownloadInfo object.</returns>
        public async Task<DownloadInfo> DownloadAssetAsync(ReleaseAsset asset, string savePath, string fileName = null!, Action<DownloadInfo>? progressChanged = null!)
        {
            var downloadInfo = new DownloadInfo
            {
                Name = asset.Name,
                Path = savePath + asset.Name
            };

            await _httpClient.DownloadFileAsync(
                asset.DownloadUrl,
                savePath,
                fileName,
                new Progress<HttpDownloadProgress>(
                progress =>
                {
                    downloadInfo.DownloadPercent = progress.PercentDownloaded;
                    downloadInfo.DownloadedSize = progress.TotalDownloaded;
                    if (progressChanged != null) progressChanged(downloadInfo);
                }));

            return downloadInfo;
        }

    }
}