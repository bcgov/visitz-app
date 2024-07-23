using PdfSharp;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using VisitzModel.Models;

namespace Visitz.Documents;

/// <summary>
/// <para>Takes a Stream with an Image for data and wraps it into a PDF document.</para>
/// <para>Centers the image, scales it to fit page bounds, and changes page orientation to match image ratio
/// (portrait/landscape).</para>
/// </summary>
/// <param name="contentTitle"></param>
/// <param name="orientation"></param>
internal class ImagePdfStreamConverter(
	string contentTitle,
	DisplayOrientation orientation = DisplayOrientation.Unknown) : IStreamConverter
{
	string ContentTitle { get; set; } = contentTitle;

	DisplayOrientation Orientation { get; set; } = orientation;

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

		page.Orientation = GetPageOrientation(imageWidth, imageHeight);
		double pageWidth = page.Width.Point;
		double pageHeight = page.Height.Point;

		TryScaleDimensions(ref imageWidth, ref imageHeight, pageWidth, pageHeight);

		double centeredX = pageWidth / 2 - imageWidth / 2;
		double centeredY = pageHeight / 2 - imageHeight / 2;

		gfx.DrawImage(image, centeredX, centeredY, imageWidth, imageHeight);
	}

	PageOrientation GetPageOrientation(double width, double height)
	{
		if (Orientation == DisplayOrientation.Unknown)
			return width > height
				? PageOrientation.Landscape
				: PageOrientation.Portrait;
		else
			return Orientation == DisplayOrientation.Portrait
				? PageOrientation.Portrait
				: PageOrientation.Landscape;
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
	}
}
