using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Realms;
using Visitz.Extensions;
using Visitz.Storage;
using Visitz.Views.BaseClasses;
using VisitzModel.Models;
using VisitzModel.Storage.Filesystem;

namespace Visitz.Views.Entity.Attachments;

internal partial class TakePhotoViewModel(ICameraProvider cameraProvider) : VisitzViewModel, ICaseloadItemHolder
{
	public static readonly string PictureFiletype = "jpg";
	public static readonly string PictureFilenamePrepend = "Pic";

	Realm AttachmentsRealm { get; set; }

	AttachmentFiler attachmentFiler;

	public CaseloadItem CaseloadItem { get; set; }

	[ObservableProperty]
	public IReadOnlyList<CameraInfo> cameras;

	[ObservableProperty]
	public CameraInfo selectedCamera;

	int selectedCameraIndex;

	[ObservableProperty]
	public bool waitingToProcess = true;

	[ObservableProperty]
	public bool processing;

	public override async void Create()
	{
		base.Create();

		AttachmentsRealm = await VisitzRealms.GetAttachmentDraftsRealmAsync();
		attachmentFiler = await VisitzFiles.GetAsync(CaseloadItem);

		await SetupCameras();
	}

	public override void Destroy()
	{
		base.Destroy();

		AttachmentsRealm.Dispose();
	}

	private async Task SetupCameras()
	{
		await cameraProvider.RefreshAvailableCameras(CancellationToken.None);
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

	public async Task SavePicture(Stream stream)
	{
		WaitingToProcess = false;

		try
		{
			byte[] thumbnailBytes = await stream.MakeThumbnail(AttachmentsViewModel.ThumbnailSize).AsBytesAsync(ImageFormat.Jpeg);
			string filename = attachmentFiler.MakeFilename(PictureFilenamePrepend, PictureFiletype);

			await AttachmentDraft.SaveNew(attachmentFiler, AttachmentsRealm, filename, stream, thumbnailBytes);
		}
		finally
		{
			WaitingToProcess = true;
		}
	}

	partial void OnWaitingToProcessChanged(bool value)
	{
		Processing = !value;
	}
}
