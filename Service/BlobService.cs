using Azure.Storage.Blobs;

namespace Service;

public class BlobService
{
    private readonly BlobServiceClient _client;

    public BlobService(BlobServiceClient client)
    {
        _client = client;
    }
    
    public string Save(string containerName, Stream stream, string? url)
    {
        // Last part of url or a unique string (GUID)
        var name = url != null ? new Uri(url).LocalPath.Split("/").Last() : Guid.NewGuid().ToString();
        // Get object to add with container
        var container = _client.GetBlobContainerClient(containerName);
        // Get object to interact with the blob
        var blob = container.GetBlobClient(name);
        // Couldn't find a replace method, so delete if it exists
        if (blob.Exists().Value) blob.Delete();
        // Now upload the file stream for avatar image
        blob.Upload(stream);
        // Return the GUID part of the blob URI
        return blob.Uri.GetLeftPart(UriPartial.Path);
    }
}