using Octokit;

namespace WhoHolds.Pipeline.Tests.TestInfrastructure;

public static class ReleaseAssetExtensions
{
    extension(ReleaseAsset)
    {
        public static ReleaseAsset Create(
            string name,
            int id = 1,
            int size = 0,
            string contentType = "application/octet-stream"
        ) =>
            new(
                url: "",
                id: id,
                nodeId: "",
                name: name,
                label: "",
                state: "uploaded",
                contentType: contentType,
                size: size,
                downloadCount: 0,
                createdAt: DateTimeOffset.UnixEpoch,
                updatedAt: DateTimeOffset.UnixEpoch,
                browserDownloadUrl: "",
                uploader: null!
            );
    }
}
