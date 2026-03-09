using VisitzApi.Models.Attachments;
using VisitzModel.Models.Attachments;
using VisitzModel.Models.EntityTypes;

namespace VisitzModelTest.Models.Attachments;

public class AttachmentTests
{
    private static readonly List<AttachmentJson> attachmentJsons =
        [
         new()
         {
             ApplicationNo = "123",
             AttachmentId = "1",
             CaseId = "2",
             Categorie = "Case",
             Category = "sasdas",
             ClientFlag = "d",
             Comments = "ssss",
             CreatedByName = "bhgbh",
             CreatedDate = "12/10/2018 13:50:02",
             EndDate = "12/10/2018 13:50:02",
             FileAutoUpdFlg = "rr",
             FileDate = "12/10/2018 13:50:02",
             FileDeferFlg = "d",
             FileDockReqFlg = "d",
             FileDockStatFlg = "d",
             FileExt = "jpg",
             FileName = "test",
             FileSize = "12",
             FileSrcPath = "ddd",
             FileSrcType = "dcs",
             FinalFlag = "d",
             FormDescription = "dfdsfs",
             Id = "1",
             IncidentId = "1",
             IncidentNo = "1",
             Internal = "1",
             LastUpdatedDate = "12/10/2018 13:50:02",
             MemoId = "1",
             MemoNumber = "1",
             NoIntervention = "ss",
             PortalVisible = "true",
             ShowOnContact = "true",
             SrId = "1",
             Status = "test",
             SubCategory = "test",
             Template = "jdaijd",
             TemplateType = "dsfs",
             UpdatedByName = "test"
         },
        new()
         {
             ApplicationNo = "123",
             AttachmentId = "2",
             CaseId = "2",
             Categorie = "Case",
             Category = "sasdas",
             ClientFlag = "d",
             Comments = "ssss",
             CreatedByName = "bhgbh",
             CreatedDate = "12/10/2018 13:50:02",
             EndDate = "12/10/2018 13:50:02",
             FileAutoUpdFlg = "rr",
             FileDate = "12/10/2018 13:50:02",
             FileDeferFlg = "d",
             FileDockReqFlg = "d",
             FileDockStatFlg = "d",
             FileExt = "jpg",
             FileName = "test",
             FileSize = "12",
             FileSrcPath = "ddd",
             FileSrcType = "dcs",
             FinalFlag = "d",
             FormDescription = "dfdsfs",
             Id = "2",
             IncidentId = "1",
             IncidentNo = "1",
             Internal = "1",
             LastUpdatedDate = "12/10/2018 13:50:02",
             MemoId = "1",
             MemoNumber = "1",
             NoIntervention = "ss",
             PortalVisible = "true",
             ShowOnContact = "true",
             SrId = "1",
             Status = "test",
             SubCategory = "test",
             Template = "jdaijd",
             TemplateType = "dsfs",
             UpdatedByName = "test"
         },
        new()
         {
             ApplicationNo = "123",
             AttachmentId = "1",
             CaseId = "2",
             Categorie = "Case",
             Category = "sasdas",
             ClientFlag = "d",
             Comments = "ssss",
             CreatedByName = "bhgbh",
             CreatedDate = "12/10/2018 13:50:02",
             EndDate = "12/10/2018 13:50:02",
             FileAutoUpdFlg = "rr",
             FileDate = "12/10/2018 13:50:02",
             FileDeferFlg = "d",
             FileDockReqFlg = "d",
             FileDockStatFlg = "d",
             FileExt = "jpg",
             FileName = "test",
             FileSize = "12",
             FileSrcPath = "ddd",
             FileSrcType = "dcs",
             FinalFlag = "d",
             FormDescription = "dfdsfs",
             Id = "3",
             IncidentId = "1",
             IncidentNo = "1",
             Internal = "1",
             LastUpdatedDate = "12/10/2018 13:50:02",
             MemoId = "1",
             MemoNumber = "1",
             NoIntervention = "ss",
             PortalVisible = "true",
             ShowOnContact = "true",
             SrId = "1",
             Status = "test",
             SubCategory = "test",
             Template = "jdaijd",
             TemplateType = "dsfs",
             UpdatedByName = "test"
         },
        new()
         {
             ApplicationNo = "123",
             AttachmentId = "1",
             CaseId = "2",
             Categorie = "Case",
             Category = "sasdas",
             ClientFlag = "d",
             Comments = "ssss",
             CreatedByName = "bhgbh",
             CreatedDate = "12/10/2018 13:50:02",
             EndDate = "12/10/2018 13:50:02",
             FileAutoUpdFlg = "rr",
             FileDate = "12/10/2018 13:50:02",
             FileDeferFlg = "d",
             FileDockReqFlg = "d",
             FileDockStatFlg = "d",
             FileExt = "jpg",
             FileName = "test",
             FileSize = "12",
             FileSrcPath = "ddd",
             FileSrcType = "dcs",
             FinalFlag = "d",
             FormDescription = "dfdsfs",
             Id = "4",
             IncidentId = "1",
             IncidentNo = "1",
             Internal = "1",
             LastUpdatedDate = "12/10/2018 13:50:02",
             MemoId = "1",
             MemoNumber = "1",
             NoIntervention = "ss",
             PortalVisible = "true",
             ShowOnContact = "true",
             SrId = "1",
             Status = "test",
             SubCategory = "test",
             Template = "jdaijd",
             TemplateType = "dsfs",
             UpdatedByName = "test"
         }
        ];

    [Fact]
    public async Task SynchronizeAsync()
    {
        var realm = await TestingUtilities.MakeRealm<AttachmentTests>();
        List<AttachmentJson> attachments = attachmentJsons;
        var parentId = "12";

        await realm.Write(async () => await Attachment.SynchronizeAsync(
            realm,
            attachments,
            parentId,
            EntityType.Case));

        var allAttachments = realm
            .All<Attachment>()
            .Where(attachment =>
                attachment.RelatedEntityId == parentId).ToList();

        Assert.Equal(attachments.Count, allAttachments.Count);

        //Checking deletion of realm objects
        attachments.Clear();
        attachments.AddRange(
            new()
            {
                ApplicationNo = "123",
                AttachmentId = "10",
                CaseId = "2",
                Categorie = "Case",
                Category = "sasdas",
                ClientFlag = "d",
                Comments = "ssss",
                CreatedByName = "bhgbh",
                CreatedDate = "12/10/2018 13:50:02",
                EndDate = "12/10/2018 13:50:02",
                FileAutoUpdFlg = "rr",
                FileDate = "12/10/2018 13:50:02",
                FileDeferFlg = "d",
                FileDockReqFlg = "d",
                FileDockStatFlg = "d",
                FileExt = "jpg",
                FileName = "test",
                FileSize = "12",
                FileSrcPath = "ddd",
                FileSrcType = "dcs",
                FinalFlag = "d",
                FormDescription = "dfdsfs",
                Id = "10",
                IncidentId = "1",
                IncidentNo = "1",
                Internal = "1",
                LastUpdatedDate = "12/10/2018 13:50:02",
                MemoId = "1",
                MemoNumber = "1",
                NoIntervention = "ss",
                PortalVisible = "true",
                ShowOnContact = "true",
                SrId = "1",
                Status = "test",
                SubCategory = "test",
                Template = "jdaijd",
                TemplateType = "dsfs",
                UpdatedByName = "test"
            },
        new()
        {
            ApplicationNo = "123",
            AttachmentId = "12",
            CaseId = "2",
            Categorie = "Case",
            Category = "sasdas",
            ClientFlag = "d",
            Comments = "ssss",
            CreatedByName = "bhgbh",
            CreatedDate = "12/10/2018 13:50:02",
            EndDate = "12/10/2018 13:50:02",
            FileAutoUpdFlg = "rr",
            FileDate = "12/10/2018 13:50:02",
            FileDeferFlg = "d",
            FileDockReqFlg = "d",
            FileDockStatFlg = "d",
            FileExt = "jpg",
            FileName = "test",
            FileSize = "12",
            FileSrcPath = "ddd",
            FileSrcType = "dcs",
            FinalFlag = "d",
            FormDescription = "dfdsfs",
            Id = "12",
            IncidentId = "1",
            IncidentNo = "1",
            Internal = "1",
            LastUpdatedDate = "12/10/2018 13:50:02",
            MemoId = "1",
            MemoNumber = "1",
            NoIntervention = "ss",
            PortalVisible = "true",
            ShowOnContact = "true",
            SrId = "1",
            Status = "test",
            SubCategory = "test",
            Template = "jdaijd",
            TemplateType = "dsfs",
            UpdatedByName = "test"
        }
     );

        await realm.Write(async () => await Attachment.SynchronizeAsync(
                realm,
                attachments,
                parentId,
                EntityType.Case));

        allAttachments = realm
            .All<Attachment>()
            .ToList();

        Assert.Equal(2, allAttachments.Count);

    }
}
