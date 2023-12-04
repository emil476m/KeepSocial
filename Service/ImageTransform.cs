using ImageMagick;

namespace Service;

public sealed class ImageTransform(Stream stream) : IDisposable
{
    private readonly MagickImage _image = new(stream);
    private Stream? _outStream;

    public ImageTransform Resize(int width, int height)
    {
        _image.Resize(new MagickGeometry(width,height));
        return this;
    }

    public ImageTransform FixOrientation()
    {
        _image.AutoOrient();
        return this;
    }

    public ImageTransform RemoveMetadata()
    {
        var exifProfile = _image.GetExifProfile();
        var iptcProfile = _image.GetIptcProfile();
        var xmpProfile = _image.GetXmpProfile();
        
        if(exifProfile != null) _image.RemoveProfile(exifProfile);
        if(iptcProfile != null) _image.RemoveProfile(iptcProfile);
        if(xmpProfile != null) _image.RemoveProfile(xmpProfile);
        return this;
    }

    public ImageTransform Jpeg()
    {
        _image.Format = MagickFormat.Jpeg;
        return this;
    }

    public Stream ToStream()
    {
        _outStream = new MemoryStream();
        _image.Write(_outStream);
        _outStream.Position = 0;
        return _outStream;
    }

    public void Dispose()
    {
        _image.Dispose();
        stream.Dispose();
        _outStream?.Dispose();
    }
}