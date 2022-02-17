namespace PetDoctor.API.Options;

public class AuthenticationOptions
{
    public string? Authority { get; set; }
    public string? Audience { get; set; }
    public bool RequireHttps { get; set; }
}