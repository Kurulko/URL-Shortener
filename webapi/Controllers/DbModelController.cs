﻿using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using URL_ShortenerAPI.Data.Models;
using URL_ShortenerAPI.Data.Results;

namespace URL_ShortenerAPI.Controllers;

public abstract class DbModelController : APIController
{
    protected readonly IMapper mapper;
    public DbModelController(IMapper mapper)
        => this.mapper = mapper;

    protected ActionResult<T_DTO> HandleDTOServiceResult<T, T_DTO>(ServiceResult<T> serviceResult, string? notFoundMessage = null)
        where T : class, IDbModel
        where T_DTO : class
    {
        ServiceResult<T_DTO> serviceResultDTO;

        if (serviceResult.Success)
        {
            var modelDTO = mapper.Map<T_DTO>(serviceResult.Model);
            serviceResultDTO = ServiceResult<T_DTO>.Ok(modelDTO);
        }
        else
        {
            serviceResultDTO = ServiceResult<T_DTO>.Fail(serviceResult.ErrorMessage!);
        }

        return HandleServiceResult(serviceResultDTO, notFoundMessage);
    }
}