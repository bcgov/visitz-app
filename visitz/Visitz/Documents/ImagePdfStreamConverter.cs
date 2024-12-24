using MetadataExtractor;
using MetadataExtractor.Formats.Exif;
using PdfSharp;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using SixLabors.ImageSharp.Metadata.Profiles.Exif;
using VisitzModel.Models;

namespace Visitz.Documents;

/// <summary>
/// <para>Takes a Stream with an Image for data and wraps it into a PDF document.</para>
/// <para>Centers the image, scales it to fit page bounds, and changes page orientation to match image ratio
/// (portrait/landscape).</para>
/// </summary>
/// <param name="contentTitle"></param>
internal class ImagePdfStreamConverter(string contentTitle) : IStreamConverter
// internal class ImagePdfStreamConverter(
// 	string contentTitle,
// 	DisplayOrientation orientation = DisplayOrientation.Unknown) : IStreamConverter
{
	string ContentTitle { get; set; } = contentTitle;

	public Task<Stream> ConvertAsync(Stream imageStream)
	{
		if (!imageStream.CanRead)
			throw new InvalidOperationException("Stream must be readable to convert.");

		if (!imageStream.CanSeek)
			throw new InvalidOperationException("Stream must be seekable to convert.");

		imageStream.Seek(0, SeekOrigin.Begin);

		using var document = MakeDocument();
		WrapImage(imageStream, document);

		MemoryStream outputStream = new();
		document.Save(outputStream);

		return Task.FromResult<Stream>(outputStream);
	}

	PdfDocument MakeDocument()
	{
		var outDocument = new PdfDocument();

		outDocument.Info.Title = ContentTitle;
		outDocument.Options.NoCompression = true;

		return outDocument;
	}

	void WrapImage(Stream imageStream, PdfDocument document)
	{
		var page = document.AddPage();
		using var gfx = XGraphics.FromPdfPage(page);
		using var image = XImage.FromStream(imageStream);

		double imageWidth = image.PixelWidth;
		double imageHeight = image.PixelHeight;

		var metadata = ImageMetadataReader.ReadMetadata(imageStream);
    	int orientation = GetImageOrientation(metadata);
		if (orientation == 6)
		{
			imageWidth = image.PixelHeight;
			imageHeight = image.PixelWidth;
		}

		double pageWidth = page.Width.Point;
		double pageHeight = page.Height.Point;

		TryScaleDimensions(ref imageWidth, ref imageHeight, pageWidth, pageHeight);

		double centeredX = pageWidth / 2 - imageWidth / 2;
		double centeredY = pageHeight / 2 - imageHeight / 2;

		// if (orientation == 6 && imageHeight > imageWidth)
		if (orientation == 6)
		{
			// gfx.RotateAtTransform(90, new XPoint(centeredX, centeredY));
			var x = centeredX + imageHeight / 2;
			var y = (centeredY + imageWidth) / 2;
			gfx.RotateAtTransform(90, new XPoint(x+150, imageHeight));
			centeredX = pageWidth / 2 - imageHeight / 2; // Adjust X position after rotation
			centeredY = pageHeight / 2 - imageWidth / 2;
		}

		gfx.DrawImage(image, centeredX, centeredY, imageWidth, imageHeight);
	}

	int GetImageOrientation(IReadOnlyList<MetadataExtractor.Directory> metadata)
	{
		foreach (var directory in metadata)
		{
			if (directory is ExifIfd0Directory exifIfd0Directory)
			{
				// Look for the orientation tag (key 0x0112)
				if (exifIfd0Directory.ContainsTag((int)ExifTag.Orientation))
				{
					return exifIfd0Directory.GetInt16((int)ExifTag.Orientation);
				}
			}
		}
		return 1; // Default orientation (no rotation needed)
	}

	static void TryScaleDimensions(ref double imageWidth, ref double imageHeight, double pageWidth, double pageHeight)
	{
		double scaleFactor = Math.Min(pageWidth / imageWidth, pageHeight / imageHeight);

		if (imageWidth > pageWidth)
		{
			imageWidth *= scaleFactor;
			imageHeight *= scaleFactor;
		}

		if (imageHeight > pageHeight)
		{
			imageWidth *= scaleFactor;
			imageHeight *= scaleFactor;
		}
		// if (scaleFactor < 1)
		// {
		// 	imageWidth *= scaleFactor;
		// 	imageHeight *= scaleFactor;
		// }
	}
}
