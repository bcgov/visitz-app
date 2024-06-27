using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.ComponentModel;
using Visitz.Views.BaseClasses;
using VisitzModel.Models;
using VisitzModel.Storage.Filesystem;

namespace Visitz.Views.Entity.Attachments;

internal partial class TakePhotoViewModel(ICameraProvider cameraProvider) : VisitzViewModel, ICaseloadItemHolder
{
	public static readonly string PictureFiletype = "jpg";
	public static readonly string PictureFilenamePrepend = "Pic";

	[ObservableProperty]
	CancellationToken token = CancellationToken.None;

	AttachmentFiler attachmentFiler;

	public CaseloadItem CaseloadItem { get; set; }

	public override async void Create()
	{
		base.Create();

		attachmentFiler = new(AttachmentFiler.PicturesPath, CaseloadItem);

		await cameraProvider.RefreshAvailableCameras(Token);
	}

	/// <summary>
	/// Caches a stream to the file system as an image and returns its filepath.
	/// </summary>
	/// <param name="stream"></param>
	/// <returns>Filepath of the cached picture.</returns>
	public async Task<string> CachePicture(Stream stream)
	{
		using (stream)
			return await attachmentFiler.CacheFile(stream, PictureFilenamePrepend, PictureFiletype);
	}
}
