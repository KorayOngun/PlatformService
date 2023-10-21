using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PlatformService.AsyncDataServices;
using PlatformService.Data;
using PlatformService.Dtos;
using PlatformService.Models;
using PlatformService.SyncDataServices.Http;

namespace PlatformService.Controller;

[Route("api/[controller]")]
[ApiController]
public class PlatformController : ControllerBase
{
    private readonly IPlatformRepo _repository;
    private readonly IMapper _mapper;
    private readonly ICommandDataClient _commandDataClient;
    private readonly IMessageBusClient _messageBusClient;
    public PlatformController(IPlatformRepo repository, IMapper mapper, ICommandDataClient commandDataClient, IMessageBusClient messageBusClient)
    {
        _repository = repository;
        _mapper = mapper;
        _commandDataClient = commandDataClient;
        _messageBusClient = messageBusClient;
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
    public async Task<ActionResult<PlatformReadDto>> CreatePlatfrom(PlatformCreateDto platformCreateDto)
    {
        var platformModel = _mapper.Map<Platform>(platformCreateDto);

        _repository.CreatePlatform(platformModel);
        _repository.SaveChanges();

        var platformReadDto = _mapper.Map<PlatformReadDto>(platformModel);


        // Send Sync Message
        // try
        // {
        //     await _commandDataClient.SendPlatformToCommand(platformReadDto);
        // }
        // catch (Exception e)
        // {
        //     Console.WriteLine($"--> Could not send synchronously {e.Message}");
        // }

        // Send Async Message
        try
        {
            var platformPublishedDto = _mapper.Map<PlatformPublishedDto>(platformReadDto);
            platformPublishedDto.Event = "Platform_Published";
            _messageBusClient.PublishNewPlatform(platformPublishedDto);
        }
        catch (Exception e)
        {
            Console.WriteLine($"--> Could not send Asynchronously {e.Message}");
        }

        return CreatedAtAction(actionName: nameof(GetById), routeValues: new { Id = platformReadDto.Id }, value: platformReadDto);
    }
}


