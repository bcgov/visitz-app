using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Security;
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

	[ObservableProperty]
	public IReadOnlyList<CameraInfo> cameras;

	[ObservableProperty]
	public CameraInfo selectedCamera;

	int selectedCameraIndex;

	public override async void Create()
	{
		base.Create();

		attachmentFiler = new(AttachmentFiler.PicturesPath, CaseloadItem);

		await cameraProvider.RefreshAvailableCameras(Token);
		Cameras = cameraProvider.AvailableCameras;

		if (Cameras.Count > 0)
			SelectedCamera = Cameras[0];
	}

	[RelayCommand]
	public void SelectNextCamera()
	{
		if (Cameras.Count > 0)
			SelectedCamera = Cameras[NextCameraIndex()];
	}

	int NextCameraIndex()
	{
		selectedCameraIndex++;
		return selectedCameraIndex %= Cameras.Count;
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
