using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using XDate.BackEnd.Data;
using XDate.BackEnd.Dtos;
using XDate.BackEnd.Helpers;
using XDate.BackEnd.Models;

namespace XDate.BackEnd.Controllers
{
    [Authorize]
    [Route("api/users/{userId}/photos")]
    [ApiController]
    public class PhotosController : ControllerBase
    {
        private readonly IDatingRepository _repo;
        private readonly IMapper _mapper;
        private readonly Cloudinary _cloudinary;
        private readonly IOptions<CloudinarySettings> _cloudinaryConfig;
        public PhotosController(IDatingRepository repo, IMapper mapper, IOptions<CloudinarySettings> cloudinaryConfig)
        {
            _cloudinaryConfig = cloudinaryConfig;
            _mapper = mapper;
            _repo = repo;

            Account account = new Account(
                _cloudinaryConfig.Value.CloudName,
                _cloudinaryConfig.Value.ApiKey,
                _cloudinaryConfig.Value.ApiSecret);

            _cloudinary = new Cloudinary(account);
        }

        [HttpGet("{id}", Name = "GetPhoto")]
        public async Task<IActionResult> GetPhoto(int id)
        {
            var photo = await _repo.GetPhoto(id);
            var photoForReturn = _mapper.Map<PhotoForReturnDto>(photo);
            return Ok(photoForReturn);
        }

        [HttpPost]
        public async Task<IActionResult> AddUserPhoto(int userId, [FromForm]PhotoForCreationDto photoForCreationDto)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            var user = await _repo.GetUser(userId);

            var file = photoForCreationDto.File;

            var uploadResult = new ImageUploadResult();

            if (file.Length > 0)
            {
                using (var stream = file.OpenReadStream())
                {
                    var uploadParams = new ImageUploadParams()
                    {
                        File = new FileDescription(file.Name, stream),
                        Transformation = new Transformation().Width(500).Height(500).Crop("fill").Gravity("face")
                    };
                    uploadResult = _cloudinary.Upload(uploadParams);
                }
            }

            photoForCreationDto.PublicId = uploadResult.PublicId;
            photoForCreationDto.Url = uploadResult.Uri.ToString();

            var photo = _mapper.Map<Photo>(photoForCreationDto);

            if (!user.Photos.Any(i => i.IsMain))
                photo.IsMain = true;

            user.Photos.Add(photo);

            if (await _repo.SaveAll())
            {
                var photoToReturn = _mapper.Map<PhotoForReturnDto>(photo);
                return CreatedAtRoute("GetPhoto", new { id = photo.Id }, photoToReturn);
            }

            return BadRequest("Failed to load");
        }

        [HttpPost("{id}/setMain")]
        public async Task<IActionResult> SetMainPhoto(int userId, int id)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            var userFromRepo = await _repo.GetUser(userId);

            if (!userFromRepo.Photos.Any(p => p.Id == id))
                return Unauthorized();

            var currentPhoto = await _repo.GetCurrentPhoto(userId);
            var photo = await _repo.GetPhoto(id);

            if (photo.IsMain)
                BadRequest("Photo is already the main");

            currentPhoto.IsMain = false;
            photo.IsMain = true;

            if (await _repo.SaveAll())
                return NoContent();

            return BadRequest("Cannot set the photo to main");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePhoto(int userId, int id)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            var userFromRepo = await _repo.GetUser(userId);

            if(!userFromRepo.Photos.Any(p=>p.Id==id))
                return Unauthorized();

            var photo = await _repo.GetPhoto(id);

            if(photo.IsMain)
                return BadRequest("Cannot delete the main photo");
            
            if(photo.PublicId != null) 
            {
                var deleteParams=new DeletionParams(photo.PublicId);
                var response = _cloudinary.Destroy(deleteParams);

                if(response.Result=="ok")
                {
                    _repo.Delete(photo);
                }
            }

            if(photo.PublicId==null)
            {
                _repo.Delete(photo);
            }
            if(await _repo.SaveAll())
                return Ok();

            return BadRequest("Cannot delet this photo");
        }
    }
}