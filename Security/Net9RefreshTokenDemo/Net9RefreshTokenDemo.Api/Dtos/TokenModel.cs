using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Net9RefreshTokenDemo.Api.Dtos;

public class TokenModel
{
[Required]
public string AccessToken { get; set; } = string.Empty;

[Required]
public string RefreshToken { get; set; } = string.Empty;
}