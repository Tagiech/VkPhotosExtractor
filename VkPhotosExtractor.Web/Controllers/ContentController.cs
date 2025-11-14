using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace VkPhotosExtractor.Web.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ContentController
{
    [HttpGet("sources")]
    public async Task<IActionResult> GetContentSources()
    {
        // returns list of available personal albums, including "tagged photos", 
        // list of owned groups and list on subscriptions to other groups
        throw new NotImplementedException();
    }

    [HttpGet("albums/{sourceId:int}")]
    public async Task<IActionResult> GetAlbums(int sourceId)
    {
        // returns list of albums from group
        throw new NotImplementedException();
    }

    [HttpGet("photos/{sourceId:int}")]
    public async Task<IActionResult> GetPhotos(int sourceId)
    {
        //returns photos from specified source (personal album, group album)
        throw new NotImplementedException();
    }

    [HttpPost("prepare-archive")]
    public async Task<IActionResult> PrepareArchive()
    {
        // download photos from vk, change their metadata, pack to archive
        // returns archiveId to check status and download later
        throw new NotImplementedException();
    }

    [HttpGet("download-archive/{archiveId}")]
    public async Task<IActionResult> DownloadArchive(string archiveId)
    {
        //check status of archive preparation, return download link when ready
        throw new NotImplementedException();
    }

    [HttpDelete("delete-archive/{archiveId}")]
    public async Task<IActionResult> DeleteArchive(string archiveId)
    {
        //delete prepared archive and related temporary data after download or on user request
        throw new NotImplementedException();
    }
}