using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Graphics.Platform;
using Realms;
using Visitz.Storage;
using Visitz.Views.BaseClasses;
using VisitzModel.Models;
using VisitzModel.Models.Drafts;
using VisitzModel.Storage.Filesystem;
using IImage = Microsoft.Maui.Graphics.IImage;

namespace Visitz.Views.Entity.Attachments;

internal partial class TakePhotoViewModel(ICameraProvider cameraProvider) : VisitzViewModel, ICaseloadItemHolder
{
	public static readonly string PictureFiletype = "jpg";
	public static readonly string PictureFilenamePrepend = "Pic";

	[ObservableProperty]
	CancellationToken token = CancellationToken.None;

	Realm AttachmentsRealm { get; set; }

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

		AttachmentsRealm = await VisitzRealms.GetAttachmentDraftsRealmAsync();
		attachmentFiler = await VisitzFiles.GetAsync(AttachmentFiler.PicturesPath, CaseloadItem);

		await cameraProvider.RefreshAvailableCameras(Token);
		Cameras = cameraProvider.AvailableCameras;

		if (Cameras.Count > 0)
			SelectedCamera = Cameras[0];
	}

	public override void Destroy()
	{
		base.Destroy();

		AttachmentsRealm.Dispose();
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
		byte[] thumbnailBytes = await MakeThumbnail(stream).AsBytesAsync();
		string fullpath = await attachmentFiler.SaveFileAsync(stream, PictureFilenamePrepend, PictureFiletype);

		var draft = AttachmentDraft.Make(fullpath, thumbnailBytes);
		draft.InitWith(CaseloadItem);

		try
		{
			await AttachmentsRealm.WriteAsync(() => AttachmentsRealm.Add(draft));
		}
		catch
		{
			if (File.Exists(fullpath))
				File.Delete(fullpath);

			throw;
		}
	}

	static IImage MakeThumbnail(Stream stream, bool disposeStream = false)
	{
		stream.Seek(0, SeekOrigin.Begin);
		return PlatformImage.FromStream(stream).Downsize(200, disposeStream);
	}
}
