<img src=".github/resources/images/github-release-downloader.png" style="width: 100%;" />

<h2 align="center">GithubReleaseDownloader</h2>

<p style="text-align: justify">
If you just want to download assets with very minimal code, we got you!
All you need to know is the public repository name and owner, and you can download release
assets easily. No need to setup api keys and clients.
</p>


## Simple console demo


I think it's easier to show how it works, here is a simple console demo
to download assets from the latest release of `fossa-client-desktop`

```csharp
using GithubReleaseDownloader;

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
        AssetDownloader.Instance .DownloadAllAssets(release, downloadPath);
    }

}
```

And done! That easy. If you need to monitor the progress, we also got you covered. just use the `progressChanged`
callback.

```csharp
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
```

## Available Methods


### :star: Release Manager

| **Method**            | **Description**                                     | **Parameters**                                  |
|-----------------------|-----------------------------------------------------|-------------------------------------------------|
| **GetWithTag()**      | Gets a release from the repo with the specified tag | string: owner<br/>string: repo<br/> string: tag |
| **GetWithTagAsync()** | Asynchronous overload of `GetWithTag()`             | string: owner<br/>string: repo<br/> string: tag |
| **GetLatest()**       | Gets the latest release from the repo               | string: owner<br/>string: repo<br/>             |
| **GetLatestAsync()**  | Asynchronous overload of `GetLatest()`              | string: owner<br/>string: repo<br/>             |
| **GetAll()**          | Gets all releases from the repo                     | string: owner<br/>string: repo<br/>             |
| **GetAllAsync()**     | Asynchronous overload of `GetAll()`                 | string: owner<br/>string: repo<br/>             |


### :arrow_down: Asset Downloader

Only the parameters with an asterisk `*` are required, the rest are optional

| **Method**                 | **Description**                                | **Parameters**                                                                                                              |
|----------------------------|------------------------------------------------|-----------------------------------------------------------------------------------------------------------------------------|
| `DownloadAllAssets()`      | Downloads all release assets                   | `Release`: release*<br/>`string`: savePath*<br/>`Action<DownloadInfo>` progressChanged                                      |
| `DownloadAllAssetsAsync()` | Asynchronous overload of `DownloadAllAssets()` | `Release`: release*<br/>`string`: savePath*<br/>`Action<DownloadInfo>` progressChanged                                      |
| `DownloadAssets()`         | Downloads selected release assets              | `IEnumerable<ReleaseAsset>` assets*<br/>`string` savePath*<br/>`Action<DownloadInfo>` progressChanged                       |
| `DownloadAssetsAsync()`    | Asynchronous overload of `DownloadAssets()`    | `IEnumerable<ReleaseAsset>` assets*<br/>`string` savePath*<br/>`Action<DownloadInfo>` progressChanged                       |
| `DownloadAsset()`          | Downloads a single release asset               | `IEnumerable<ReleaseAsset>` assets*<br/>`string` savePath*<br/>`string` fileName<br/>`Action<DownloadInfo>` progressChanged |
| `DownloadAssetAsync()`     | Asynchronous overload of `DownloadAsset()`     | `IEnumerable<ReleaseAsset>` assets*<br/>`string` savePath*<br/>`string` fileName<br/>`Action<DownloadInfo>` progressChanged |

## Support

I do this freely to help the community :heart:, but supporting this project means I can afford more
time to spend in these projects. Who knows, we might be able to create
something even more awesome! Click on the sponsor button in this repository to show some love for this project :heart:


## Special Message
This project was developed with the help of jetbrains products, thank you [JetBrains](https://www.jetbrains.com/) for supporting this project by providing licences to the JetBrains Suite!

<a href="https://www.jetbrains.com/community/opensource/#support">
<img width="200px" src="https://resources.jetbrains.com/storage/products/company/brand/logos/jb_beam.png" alt="JetBrains Logo (Main) logo.">
</a>