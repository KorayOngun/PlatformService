using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PlatformService.Data;
using PlatformService.Dtos;
using PlatformService.Models;

namespace PlatformService.Controller;

[Route("api/[controller]")]
[ApiController]
public class PlatformController : ControllerBase
{
    private readonly IPlatformRepo _repository;
    private readonly IMapper _mapper;

    public PlatformController(IPlatformRepo repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    [HttpGet("[action]")]
    public ActionResult<IEnumerable<PlatformReadDto>> Get()
    {
        var data = _repository.GetAllPlatforms();
        var response = _mapper.Map<IEnumerable<Platform>, IEnumerable<PlatformReadDto>>(data);
        return Ok(response);
    }

    [HttpGet("[action]{id}")]
    public ActionResult<IEnumerable<PlatformReadDto>> GetById(int id)
    {
        var data = _repository.GetPlatformById(id);
        var response = _mapper.Map<Platform, PlatformReadDto>(data);
        return Ok(response);
    }


    [HttpPost]
    public ActionResult<PlatformReadDto> CreatePlatfrom(PlatformCreateDto platformCreateDto)
    {
        var platformModel = _mapper.Map<Platform>(platformCreateDto);
        _repository.CreatePlatform(platformModel);
        _repository.SaveChanges();
        var platformReadDto = _mapper.Map<PlatformReadDto>(platformModel);
        return CreatedAtAction(actionName: nameof(GetById), routeValues: new { Id = platformReadDto.Id }, value: platformReadDto);
    }
}


