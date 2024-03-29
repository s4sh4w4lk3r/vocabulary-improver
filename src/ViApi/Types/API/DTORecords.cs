﻿namespace ViApi.Types.API;

public record class ApiUserDto(string Username, string Email, string Firstname, string Password);
public record class JwtTokenDto(string JwtToken);
public record class WordDto(string SourceWord, string TargetWord);