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
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using GithubReleaseDownloader.Entities;
using Newtonsoft.Json;

#pragma warning disable CS8618

namespace GithubReleaseDownloader
{
    /// <summary>
    /// Manages downloading GitHub releases.
    /// </summary>
    public class ReleaseManager
    {
        private static readonly object Lock = new object();
        private static ReleaseManager? _instance;
        private static HttpClient _httpClient;

        private ReleaseManager()
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri("https://api.github.com");
            _httpClient.DefaultRequestHeaders.TryAddWithoutValidation(
                "User-Agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");
        }

        /// <summary>
        /// Gets the singleton instance of the <see cref="ReleaseManager"/> class.
        /// </summary>
        public static ReleaseManager Instance
        {
            get
            {
                if (_instance is null)
                {
                    lock (Lock)
                    {
                        return _instance ??= new ReleaseManager();
                    }
                }
                return _instance;
            }
        }

        /// <summary>
        /// Gets the release with the specified tag from the specified repository.
        /// </summary>
        /// <param name="owner">The owner of the repository.</param>
        /// <param name="repository">The name of the repository.</param>
        /// <param name="tag">The tag of the release.</param>
        /// <returns>The <see cref="Release"/> object if found; otherwise, <c>null</c>.</returns>
        public Release? GetWithTag(string owner, string repository, string tag)
        {
            return Task.Run(async () => await GetWithTagAsync(owner, repository, tag)).Result;
        }

        /// <summary>
        /// Asynchronously gets the release with the specified tag from the specified repository.
        /// </summary>
        /// <param name="owner">The owner of the repository.</param>
        /// <param name="repository">The name of the repository.</param>
        /// <param name="tag">The tag of the release.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the <see cref="Release"/> object if found; otherwise, <c>null</c>.</returns>
        public async Task<Release?> GetWithTagAsync(string owner, string repository, string tag)
        {
            var response = await _httpClient.GetAsync(
                $"/repos/{owner}/{repository}/releases/tags/{tag}",
                HttpCompletionOption.ResponseContentRead);
            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<Release>(content);
        }
        
        /// <summary>
        /// Gets the latest release from the specified repository.
        /// </summary>
        /// <param name="owner">The owner of the repository.</param>
        /// <param name="repository">The name of the repository.</param>
        /// <returns>The <see cref="Release"/> object if found; otherwise, <c>null</c>.</returns>
        public Release? GetLatest(string owner, string repository)
        {
            return Task.Run(async () => await GetLatestAsync(owner, repository)).Result;
        }

        /// <summary>
        /// Asynchronously gets the latest release from the specified repository.
        /// </summary>
        /// <param name="owner">The owner of the repository.</param>
        /// <param name="repository">The name of the repository.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the <see cref="Release"/> object if found; otherwise, <c>null</c>.</returns>
        public async Task<Release?> GetLatestAsync(string owner, string repository)
        {
            var response = await _httpClient.GetAsync(
                $"/repos/{owner}/{repository}/releases/latest",
                HttpCompletionOption.ResponseContentRead);
            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<Release>(content);
        }
        
        /// <summary>
        /// Gets all releases from the specified repository.
        /// </summary>
        /// <param name="owner">The owner of the repository.</param>
        /// <param name="repository">The name of the repository.</param>
        /// <returns>An enumerable collection of <see cref="Release"/> objects if found; otherwise, an empty collection.</returns>
        public IEnumerable<Release>? GetAll(string owner, string repository)
        {
            return Task.Run(async () => await GetAllAsync(owner, repository)).Result;
        }

        /// <summary>
        /// Asynchronously gets all releases from the specified repository.
        /// </summary>
        /// <param name="owner">The owner of the repository.</param>
        /// <param name="repository">The name of the repository.</param>
        /// <returns>A task that represents the asynchronousoperation. The task result contains an enumerable collection of <see cref="Release"/> objects if found; otherwise, <c>null</c>.</returns>
        public async Task<IEnumerable<Release>?> GetAllAsync(string owner, string repository)
        {
            var response = await _httpClient.GetAsync(
                $"/repos/{owner}/{repository}/releases",
                HttpCompletionOption.ResponseContentRead);
            var content = await response.Content.ReadAsStringAsync();
            
            var releases = JsonConvert.DeserializeObject<IEnumerable<Release>>(content);

            if (releases is null)
            {
                return Enumerable.Empty<Release>();
            }
            
            return releases;
        }
    }
}