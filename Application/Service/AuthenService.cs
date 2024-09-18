﻿using Application.Commons;
using Application.IRepository;
using Application.IService;
using Application.ServiceResponse;
using Application.Utils;
using Application.ViewModels.UserDTO;
using AutoMapper;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Service
{
    public class AuthenService : IAuthenService
    {
        private readonly AppConfiguration _config;
        private readonly IUserRepo _userRepo;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        public AuthenService(AppConfiguration configuration, IUserRepo _user, IMapper mapper, IUnitOfWork unitOfWork)
        {
            _config = configuration;
            _userRepo = _user;
            _mapper = mapper;
            _unitOfWork = unitOfWork;

        }

        public async Task<ServiceResponse<RegisterDTO>> RegisterAsync(RegisterDTO userObject)
        {
            var response = new ServiceResponse<RegisterDTO>();
            try
            {
                var existEmail = await _unitOfWork.UserRepository.CheckEmailAddressExisted(userObject.Email);
                if (existEmail)
                {
                    response.Success = false;
                    response.Message = "Email is already existed";
                    return response;
                }

                var userAccountRegister = _mapper.Map<User>(userObject);
                userAccountRegister.Password = HashPass.HashPassWithSHA256(userObject.Password);
                //Create Token
                userAccountRegister.ConfirmationToken = Guid.NewGuid().ToString();

                userAccountRegister.Status = 1;
                userAccountRegister.RoleName = "Customer";
                await _unitOfWork.UserRepository.AddAsync(userAccountRegister);

                var confirmationLink =
                    $"https://cyberducky-gtbsaceffbhthhc5.eastus-01.azurewebsites.net//confirm?token={userAccountRegister.ConfirmationToken}";

                //SendMail
                var emailSend = await SendMail.SendConfirmationEmail(userObject.Email, confirmationLink);                
                if (!emailSend)
                {
                    response.Success = false;
                    response.Message = "Error when send mail";
                    return response;
                }
                var accountRegistedDTO = _mapper.Map<RegisterDTO>(userAccountRegister);
                response.Success = true;
                response.Data = accountRegistedDTO;
                response.Message = "Register successfully.";
            }
            catch (DbException e)
            {
                response.Success = false;
                response.Message = "Database error occurred.";
                response.ErrorMessages = new List<string> { e.Message };
            }
            catch (Exception e)
            {
                response.Success = false;
                response.Message = "Error";
                response.ErrorMessages = new List<string> { e.Message };
            }
            return response;
        }
    }
}
