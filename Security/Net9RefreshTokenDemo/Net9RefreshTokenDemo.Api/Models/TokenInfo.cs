using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Net9RefreshTokenDemo.Api.Models;

public class TokenInfo
{
    public int Id { get; set; }

    [Required]
    [MaxLength(30)]
    public string Username { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string RefreshToken { get; set; } = string.Empty;

    [Required]
    public DateTime ExpiredAt { get; set; }
    
}